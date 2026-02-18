using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ESP32JsonReader : MonoBehaviour
{
    [Header("Config")]
    public GameTuningConfig config;

    [Header("Serial Settings")]
    public string portName = "COM5";
    public int baudRate = 115200;

    [Header("Connection Watchdog")]
    public float disconnectAfterSeconds = 1.0f;

    [Header("Weight Filtering")]
    public bool enableZeroGlitchFilter = true;
    public float zeroGlitchHoldSeconds = 0.25f;   // how long we keep last good weight
    public float zeroGlitchMinGood = 0.05f;       // below this counts as "near zero"

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning;

    private readonly object _lock = new object();
    private string latestLine; // raw line from serial thread

    // Parsed outputs
    public float CurrentWeight { get; private set; }
    public string CurrentItemId { get; private set; } = "";
    public bool CurrentPlate { get; private set; }

    // Derived outputs
    public bool PlatePressedEvent { get; private set; } // true for 1 frame
    public ItemType SelectedItem { get; private set; } = ItemType.None;

    // Debug outputs
    public string LatestRawJson { get; private set; } = "";
    public float LastReceivedTime { get; private set; }
    public bool IsConnected => (Time.unscaledTime - LastReceivedTime) <= disconnectAfterSeconds;

    public string LastDetectedItemId { get; private set; } = "";

    public System.Action<string> OnRfidDetected;


    float lastGoodWeight;
    float lastGoodTime;



    // Tare (offset baseline)
    private float tareOffset;

    private bool prevPlate;

    void Start()
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (var p in ports)
        {
            Debug.Log("Available Port: " + p);
        }

        TryOpenPort();
    }

    void TryOpenPort()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 100;

        try
        {
            serialPort.Open();
            isRunning = true;

            readThread = new Thread(ReadSerial) { IsBackground = true };
            readThread.Start();

            Debug.Log($"ESP32 Connected on {portName} @ {baudRate}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial Error: " + e.Message);
            isRunning = false;
        }
    }

    void ReadSerial()
    {
        while (isRunning)
        {
            try
            {
                string line = serialPort.ReadLine();
                lock (_lock) latestLine = line;
            }
            catch
            {
                // ignore timeouts/disconnect noise for event build
            }
        }
    }

    void Update()
    {
        PlatePressedEvent = false;

        // Pull latest line safely
        string line = null;
        lock (_lock)
        {
            if (!string.IsNullOrEmpty(latestLine))
            {
                line = latestLine;
                latestLine = null;
            }
        }

        if (string.IsNullOrEmpty(line))
            return;

        LatestRawJson = line;
        LastReceivedTime = Time.unscaledTime;

        try
        {
            var parsed = JsonUtility.FromJson<ESP32Data>(line);

            // Weight with tare
            float rawWeight = parsed.weight;
            CurrentWeight = Mathf.Max(0f, rawWeight - tareOffset);

            if (enableZeroGlitchFilter)
            {
                float now = Time.unscaledTime;

                // If it's a "good" value, store it
                if (CurrentWeight > zeroGlitchMinGood)
                {
                    lastGoodWeight = CurrentWeight;
                    lastGoodTime = now;
                }
                else
                {
                    // If we recently had a good value, treat this zero as a glitch and hold last good
                    if ((now - lastGoodTime) <= zeroGlitchHoldSeconds)
                    {
                        CurrentWeight = lastGoodWeight;
                    }
                    // else: it's a real zero (no pressure / nothing on pan)
                }
            }


            // RFID (can be empty on removal)
            CurrentItemId = parsed.id_item ?? "";

            // If ESP32 sends ID only once, store it
            if (!string.IsNullOrEmpty(CurrentItemId))
            {
                LastDetectedItemId = CurrentItemId;
                OnRfidDetected?.Invoke(CurrentItemId);
            }

            // Button
            CurrentPlate = parsed.bt_plate;

            // Rising edge event
            PlatePressedEvent = (!prevPlate && CurrentPlate);
            prevPlate = CurrentPlate;

            // Selected item from config mapping
            if (config != null && !string.IsNullOrEmpty(CurrentItemId))
                SelectedItem = config.GetItemFromRfid(CurrentItemId);
            else
                SelectedItem = ItemType.None;
        }
        catch
        {
            // Keep LatestRawJson visible in F1 panel; ignore bad lines
        }
    }

    // Call this from F1 Dev Panel
    public void TareNow()
    {
        // Set current displayed weight to 0 baseline
        tareOffset += CurrentWeight;
    }

    void OnApplicationQuit()
    {
        isRunning = false;

        try { if (readThread != null) readThread.Join(200); } catch { }

        try
        {
            if (serialPort != null && serialPort.IsOpen)
                serialPort.Close();
        }
        catch { }
    }
}
