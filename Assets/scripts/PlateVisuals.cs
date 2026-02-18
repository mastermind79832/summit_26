using UnityEngine;

public class PlateVisuals : MonoBehaviour
{
    [Header("Plate Slot Renderers")]
    public SpriteRenderer bottomSlot;
    public SpriteRenderer pattySlot;
    public SpriteRenderer topSlot;

    [Header("Cooked Item Sprites")]
    public Sprite bottomBunSprite;
    public Sprite pattySprite;
    public Sprite topBunSprite;

    public void ClearAll()
    {
        Set(bottomSlot, null);
        Set(pattySlot, null);
        Set(topSlot, null);
    }

    public void Place(ItemType item)
    {
        switch (item)
        {
            case ItemType.BottomBun: Set(bottomSlot, bottomBunSprite); break;
            case ItemType.Patty:     Set(pattySlot, pattySprite); break;
            case ItemType.TopBun:    Set(topSlot, topBunSprite); break;
        }
    }

    void Set(SpriteRenderer r, Sprite s)
    {
        if (!r) return;
        r.sprite = s;
        r.enabled = (s != null);
    }
}
