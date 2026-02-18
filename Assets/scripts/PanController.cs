using UnityEngine;

public class PanController : MonoBehaviour
{
    [Header("Refs")]
    public ESP32JsonReader reader;
    public GameTuningConfig config;

    [Header("Pressure Presence (Noise-proof)")]
    public float cookStartPressure = 0.15f;     // cooking starts above this
    public float removePressureThreshold = 0.08f; // considered "released" below this
    public float removeHoldSeconds = 0.25f;     // must stay low for this long to remove

    // Outputs for UI
    public float Pressure01 { get; private set; }
    public float Cook01 { get; private set; }
    public ItemType PanItem { get; private set; } = ItemType.None;
    public bool IsBurnt { get; private set; }

    // Output for plate
    public ItemType HeldCookedItem { get; private set; } = ItemType.None;

    public enum CookResult { None, Undercooked, Perfect, Burnt }
    public CookResult LastResult { get; private set; } = CookResult.None;
    public System.Action<CookResult> OnCookResult;

    enum State { Empty, ReadyOnPan, Cooking, Burnt }
    State state = State.Empty;

    public float CookProgress => cookProgress;
    public float PerfectMin => config != null ? (config.GetProfile(PanItem)?.perfectMin ?? 0f) : 0f;
    public float PerfectMax => config != null ? (config.GetProfile(PanItem)?.perfectMax ?? 0f) : 0f;
    public float BurnLimit  => config != null ? (config.GetProfile(PanItem)?.burnTimeLimit ?? 1f) : 1f;


    float smoothedWeight;
    float cookProgress;
    float lowPressureTimer;

    void OnEnable()
    {
        if (reader != null) reader.OnRfidDetected += HandleRfid;
    }

    void OnDisable()
    {
        if (reader != null) reader.OnRfidDetected -= HandleRfid;
    }

    void HandleRfid(string id)
    {
        if (config == null) return;
        if (HeldCookedItem != ItemType.None) return; // force player to plate first

        ItemType item = config.GetItemFromRfid(id);
        if (item == ItemType.None) return;

        // If pan is empty, place item immediately
        if (state == State.Empty)
        {
            PanItem = item;
            state = State.ReadyOnPan;
            IsBurnt = false;
            cookProgress = 0f;
            Cook01 = 0f;
            lowPressureTimer = 0f;
        }

        // Optional: if already has an item, ignore new RFID scans
        // (prevents spam scans changing item mid-pan)
    }

    public void ClearHeldItem() => HeldCookedItem = ItemType.None;

    public void HardReset()
    {
        HeldCookedItem = ItemType.None;
        ResetPanInternal();
        smoothedWeight = 0f;
        lowPressureTimer = 0f;
        LastResult = CookResult.None;
    }

    void Update()
    {
        if (reader == null || config == null) return;

        // Smooth the incoming weight (plus your zero-glitch filter in reader)
        float smooth = Mathf.Max(1f, config.weightSmoothing);
        smoothedWeight = Mathf.Lerp(smoothedWeight, reader.CurrentWeight, smooth * Time.deltaTime);
        float w = smoothedWeight;

        // Pressure bar scaling (use your existing config maxPressureWeight)
        Pressure01 = Mathf.Clamp01((w) / Mathf.Max(0.0001f, config.maxPressureWeight));
        // Note: since we’re not using presenceThreshold anymore, we map from 0..maxPressureWeight.

        // If no item on pan, nothing else to do
        if (state == State.Empty) { Cook01 = 0f; return; }

        // Removal detection: if pressure stays near-zero long enough
        if (w <= removePressureThreshold)
        {
            lowPressureTimer += Time.deltaTime;
            if (lowPressureTimer >= removeHoldSeconds)
            {
                ResolveOnRemoval();
                ResetPanInternal();
            }
            return;
        }
        else
        {
            lowPressureTimer = 0f;
        }

        // If burnt, keep showing burnt until removed
        if (state == State.Burnt) return;

        // Start cooking only when above cookStartPressure
        if (w < cookStartPressure)
        {
            // not cooking yet, but item stays on pan
            return;
        }

        // Now cooking
        state = State.Cooking;

        var profile = config.GetProfile(PanItem);
        if (profile == null) return;

        // Instant burn
        if (w >= config.burnPressureThreshold)
        {
            BurnNow(immediatePopup: true);
            return;
        }

        cookProgress += profile.baseCookSpeed * Pressure01 * Time.deltaTime;
        Cook01 = Mathf.Clamp01(cookProgress / Mathf.Max(0.0001f, profile.burnTimeLimit));

        if (cookProgress >= profile.burnTimeLimit)
            BurnNow(immediatePopup: true);
    }

    void ResolveOnRemoval()
    {
        if (PanItem == ItemType.None) return;

        if (state == State.Burnt || IsBurnt)
        {
            LastResult = CookResult.Burnt;
            OnCookResult?.Invoke(LastResult);
            return;
        }

        var profile = config.GetProfile(PanItem);
        if (profile == null) return;

        bool inPerfect = (cookProgress >= profile.perfectMin && cookProgress <= profile.perfectMax);

        if (inPerfect)
        {
            HeldCookedItem = PanItem;
            LastResult = CookResult.Perfect;
        }
        else
        {
            LastResult = CookResult.Undercooked;
        }

        OnCookResult?.Invoke(LastResult);
    }

    void BurnNow(bool immediatePopup)
    {
        state = State.Burnt;
        IsBurnt = true;
        Cook01 = 1f;

        if (immediatePopup)
        {
            LastResult = CookResult.Burnt;
            OnCookResult?.Invoke(LastResult);
        }
    }

    void ResetPanInternal()
    {
        state = State.Empty;
        PanItem = ItemType.None;
        IsBurnt = false;
        cookProgress = 0f;
        Cook01 = 0f;
        lowPressureTimer = 0f;
    }
}
