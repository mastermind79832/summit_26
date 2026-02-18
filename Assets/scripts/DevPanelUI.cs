using UnityEngine;
using TMPro;

public class DevPanelUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject panelRoot;
    public ESP32JsonReader reader;
    public GameTuningConfig config;

    [Header("Debug Text")]
    public TMP_Text txtConnection;
    public TMP_Text txtRawJson;
    public TMP_Text txtParsed;
    
    [Header("RFID Debug")]
    public TMP_Text txtLastRfid;


    [Header("RFID Mapping Fields")]
    public TMP_InputField inBottomId;
    public TMP_InputField inPattyId;
    public TMP_InputField inTopId;

    [Header("Tuning - Global")]
    public TMP_InputField inPresenceThreshold;
    public TMP_InputField inMaxPressureWeight;
    public TMP_InputField inBurnPressureThreshold;
    public TMP_InputField inWeightSmoothing;

    [Header("Tuning - Bottom Bun")]
    public TMP_InputField inBottomPerfectMin;
    public TMP_InputField inBottomPerfectMax;
    public TMP_InputField inBottomBurnLimit;
    public TMP_InputField inBottomBaseSpeed;

    [Header("Tuning - Patty")]
    public TMP_InputField inPattyPerfectMin;
    public TMP_InputField inPattyPerfectMax;
    public TMP_InputField inPattyBurnLimit;
    public TMP_InputField inPattyBaseSpeed;

    [Header("Tuning - Top Bun")]
    public TMP_InputField inTopPerfectMin;
    public TMP_InputField inTopPerfectMax;
    public TMP_InputField inTopBurnLimit;
    public TMP_InputField inTopBaseSpeed;

    [Header("Options")]
    public KeyCode toggleKey = KeyCode.F1;
    public bool pauseGameWhileOpen = true;

    void Start()
    {
        if (panelRoot) panelRoot.SetActive(false);
        RefreshAllFields();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();

        if (panelRoot == null || !panelRoot.activeSelf) return;

        if (reader != null)
        {
            if (txtConnection)
                txtConnection.text = reader.IsConnected ? $"ESP32: OK ({reader.portName})" : $"ESP32: DISCONNECTED ({reader.portName})";

            if (txtRawJson)
                txtRawJson.text = string.IsNullOrEmpty(reader.LatestRawJson) ? "(no data yet)" : reader.LatestRawJson;

            if (txtParsed)
            {
                txtParsed.text =
                    $"Weight: {reader.CurrentWeight:F2}\n" +
                    $"ID: {reader.CurrentItemId}\n" +
                    $"Plate: {reader.CurrentPlate}\n" +
                    $"SelectedItem: {reader.SelectedItem}";
            }

            if (txtLastRfid)
            {
                txtLastRfid.text = string.IsNullOrEmpty(reader.LastDetectedItemId)
                    ? "Last RFID: (none)"
                    : $"Last RFID: {reader.LastDetectedItemId}";
            }

        }
    }

    public void Toggle()
    {
        if (!panelRoot) return;

        bool show = !panelRoot.activeSelf;
        panelRoot.SetActive(show);

        if (pauseGameWhileOpen)
            Time.timeScale = show ? 0f : 1f;

        if (show)
            RefreshAllFields();
    }

    // ---------- RFID Mapping ----------
    public void CaptureBottom()
    {
        if (reader == null) return;
        if (!string.IsNullOrEmpty(reader.LastDetectedItemId))
            inBottomId.text = reader.LastDetectedItemId;

    }

    public void CapturePatty()
    {
        if (reader == null) return;
        if (!string.IsNullOrEmpty(reader.LastDetectedItemId))
            inPattyId.text = reader.LastDetectedItemId;

    }

    public void CaptureTop()
    {
        if (reader == null) return;
        if (!string.IsNullOrEmpty(reader.LastDetectedItemId))
            inTopId.text = reader.LastDetectedItemId;

    }

    public void ApplyMapping()
    {
        if (config == null) return;

        SetIdFor(ItemType.BottomBun, inBottomId.text.Trim());
        SetIdFor(ItemType.Patty,     inPattyId.text.Trim());
        SetIdFor(ItemType.TopBun,    inTopId.text.Trim());
    }

    // ---------- Tuning Apply ----------
    public void ApplyTuning()
    {
        if (config == null) return;

        // Global
        TrySetFloat(inPresenceThreshold,    v => config.presenceThreshold = v);
        TrySetFloat(inMaxPressureWeight,    v => config.maxPressureWeight = v);
        TrySetFloat(inBurnPressureThreshold,v => config.burnPressureThreshold = v);
        TrySetFloat(inWeightSmoothing,      v => config.weightSmoothing = v);

        // Profiles
        ApplyProfile(ItemType.BottomBun, inBottomPerfectMin, inBottomPerfectMax, inBottomBurnLimit, inBottomBaseSpeed);
        ApplyProfile(ItemType.Patty,     inPattyPerfectMin,  inPattyPerfectMax,  inPattyBurnLimit,  inPattyBaseSpeed);
        ApplyProfile(ItemType.TopBun,    inTopPerfectMin,    inTopPerfectMax,    inTopBurnLimit,    inTopBaseSpeed);
    }

    void ApplyProfile(ItemType item, TMP_InputField pMin, TMP_InputField pMax, TMP_InputField burn, TMP_InputField speed)
    {
        var prof = config.GetProfile(item);
        if (prof == null) return;

        TrySetFloat(pMin,  v => prof.perfectMin = v);
        TrySetFloat(pMax,  v => prof.perfectMax = v);
        TrySetFloat(burn,  v => prof.burnTimeLimit = v);
        TrySetFloat(speed, v => prof.baseCookSpeed = v);
    }

    void TrySetFloat(TMP_InputField field, System.Action<float> setter)
    {
        if (field == null) return;
        if (float.TryParse(field.text, out float v))
            setter(v);
    }

    // ---------- Refresh ----------
    public void RefreshAllFields()
    {
        if (config == null) return;

        // RFID
        if (inBottomId) inBottomId.text = GetIdFor(ItemType.BottomBun);
        if (inPattyId)  inPattyId.text  = GetIdFor(ItemType.Patty);
        if (inTopId)    inTopId.text    = GetIdFor(ItemType.TopBun);

        // Global
        SetField(inPresenceThreshold,     config.presenceThreshold);
        SetField(inMaxPressureWeight,     config.maxPressureWeight);
        SetField(inBurnPressureThreshold, config.burnPressureThreshold);
        SetField(inWeightSmoothing,       config.weightSmoothing);

        // Profiles
        FillProfile(ItemType.BottomBun, inBottomPerfectMin, inBottomPerfectMax, inBottomBurnLimit, inBottomBaseSpeed);
        FillProfile(ItemType.Patty,     inPattyPerfectMin,  inPattyPerfectMax,  inPattyBurnLimit,  inPattyBaseSpeed);
        FillProfile(ItemType.TopBun,    inTopPerfectMin,    inTopPerfectMax,    inTopBurnLimit,    inTopBaseSpeed);
    }

    void FillProfile(ItemType item, TMP_InputField pMin, TMP_InputField pMax, TMP_InputField burn, TMP_InputField speed)
    {
        var prof = config.GetProfile(item);
        if (prof == null) return;

        SetField(pMin,  prof.perfectMin);
        SetField(pMax,  prof.perfectMax);
        SetField(burn,  prof.burnTimeLimit);
        SetField(speed, prof.baseCookSpeed);
    }

    void SetField(TMP_InputField field, float value)
    {
        if (field) field.text = value.ToString("0.###");
    }

    // ---------- Helpers for RFID ----------
    string GetIdFor(ItemType item)
    {
        for (int i = 0; i < config.rfidMap.Count; i++)
            if (config.rfidMap[i] != null && config.rfidMap[i].item == item)
                return config.rfidMap[i].id;

        return "";
    }

    void SetIdFor(ItemType item, string id)
    {
        for (int i = config.rfidMap.Count - 1; i >= 0; i--)
            if (config.rfidMap[i] != null && config.rfidMap[i].item == item)
                config.rfidMap.RemoveAt(i);

        if (!string.IsNullOrEmpty(id))
            config.rfidMap.Add(new RfidMapEntry { item = item, id = id });
    }

    // ---------- Tare ----------
    public void TareNow() => reader?.TareNow();

    public void Close()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(false);
        if (pauseGameWhileOpen) Time.timeScale = 1f;
    }
}
