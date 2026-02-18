using UnityEngine;

public class PlateController : MonoBehaviour
{
    [Header("Refs")]
    public ESP32JsonReader reader;
    public PanController pan;
    public PlateVisuals visuals;

    // Plate state
    public bool HasBottom { get; private set; }
    public bool HasPatty { get; private set; }
    public bool HasTop { get; private set; }
    public bool IsComplete => HasBottom && HasPatty && HasTop;

    // Event to notify GameManager when a burger is served
    public System.Action OnBurgerServed;

    void Start()
    {
        ResetPlate();
    }

    void Update()
    {
        if (reader == null || pan == null) return;
        if (!reader.PlatePressedEvent) return;

        // If holding a cooked item, place it
        if (pan.HeldCookedItem != ItemType.None)
        {
            TryPlaceHeldItem();
            return;
        }

        // If not holding anything, and plate is complete -> serve
        if (IsComplete)
        {
            ServeBurger();
        }
    }

    void TryPlaceHeldItem()
    {
        ItemType item = pan.HeldCookedItem;

        bool placed = false;

        switch (item)
        {
            case ItemType.BottomBun:
                if (!HasBottom) { HasBottom = true; placed = true; }
                break;

            case ItemType.Patty:
                if (!HasPatty) { HasPatty = true; placed = true; }
                break;

            case ItemType.TopBun:
                if (!HasTop) { HasTop = true; placed = true; }
                break;
        }

        if (placed)
        {
            visuals?.Place(item);
            pan.ClearHeldItem();
        }
        // else: duplicate attempt -> ignore (optional: play error beep later)
    }

    void ServeBurger()
    {
        OnBurgerServed?.Invoke();
        ResetPlate();
    }

    public void ResetPlate()
    {
        HasBottom = HasPatty = HasTop = false;
        visuals?.ClearAll();
    }
}
