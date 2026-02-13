using UnityEngine;
using System.IO.Ports;
using System.Threading;

public class ESP32JsonReader : MonoBehaviour
{
    [Header("Serial Settings")]
    public string portName = "COM5";   // Change this
    public int baudRate = 115200;

    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;

    private string latestJson = null;
    private ESP32Data parsedData;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 100;

        try
        {
            serialPort.Open();
            isRunning = true;

            readThread = new Thread(ReadSerial);
            readThread.Start();

            Debug.Log("ESP32 Connected");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Serial Error: " + e.Message);
        }
    }

    void ReadSerial()
    {
        while (isRunning)
        {
            try
            {
                string data = serialPort.ReadLine();
                latestJson = data;
            }
            catch { }
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(latestJson))
        {
            try
            {
                parsedData = JsonUtility.FromJson<ESP32Data>(latestJson);

                // 🔹 Example Usage
                Debug.Log("Weight: " + parsedData.weight);
                Debug.Log("Item ID: " + parsedData.id_Item);
                Debug.Log("Plate Button: " + parsedData.bt_plate);
                Debug.Log("Waste Button: " + parsedData.bt_waste);
                Debug.Log("Timestamp: " + parsedData.timestamp);

                // Example logic
                if (parsedData.bt_plate)
                {
                    Debug.Log("Plate detected");
                }

                if (parsedData.bt_waste)
                {
                    Debug.Log("Waste detected");
                }
            }
            catch
            {
                Debug.LogWarning("Invalid JSON: " + latestJson);
            }

            latestJson = null;
        }
    }

    void OnApplicationQuit()
    {
        isRunning = false;

        if (readThread != null)
            readThread.Join();

        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}