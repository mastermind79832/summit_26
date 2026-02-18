using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { None, BottomBun, Patty, TopBun }

[Serializable]
public class RfidMapEntry
{
    public string id;      // RFID tag id string (e.g. "A1B2C3")
    public ItemType item;  // which item this id represents
}

[Serializable]
public class CookProfile
{
    public ItemType item;

    [Header("Perfect Window (seconds of cookProgress)")]
    public float perfectMin = 1.6f;
    public float perfectMax = 2.2f;

    [Header("Burn Limit (seconds of cookProgress)")]
    public float burnTimeLimit = 3.0f;

    [Header("Speed Multiplier")]
    public float baseCookSpeed = 1.0f; // progress per second at full pressure
}

[CreateAssetMenu(menuName = "BurgerStand/Game Tuning Config")]
public class GameTuningConfig : ScriptableObject
{
    [Header("RFID Mapping")]
    public List<RfidMapEntry> rfidMap = new();

    [Header("Pan Thresholds (Weight)")]
    public float presenceThreshold = 0.20f;      // item considered on pan above this
    public float maxPressureWeight = 3.00f;      // maps to Pressure01=1
    public float burnPressureThreshold = 2.70f;  // instant burn if weight >= this

    [Header("Weight Smoothing")]
    public float weightSmoothing = 10f;          // higher = less jitter

    [Header("Cook Profiles")]
    public List<CookProfile> cookProfiles = new();

    public ItemType GetItemFromRfid(string id)
    {
        if (string.IsNullOrEmpty(id)) return ItemType.None;

        for (int i = 0; i < rfidMap.Count; i++)
        {
            if (rfidMap[i] != null && rfidMap[i].id == id)
                return rfidMap[i].item;
        }

        return ItemType.None;
    }

    public CookProfile GetProfile(ItemType item)
    {
        for (int i = 0; i < cookProfiles.Count; i++)
        {
            if (cookProfiles[i] != null && cookProfiles[i].item == item)
                return cookProfiles[i];
        }

        return null;
    }
}
