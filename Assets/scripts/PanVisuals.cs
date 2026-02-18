using UnityEngine;

public class PanVisuals : MonoBehaviour
{
    [Header("Refs")]
    public PanController pan;
    public SpriteRenderer panRenderer;

    [Header("Cooked Sprites")]
    public Sprite bottomBunSprite;
    public Sprite pattySprite;
    public Sprite topBunSprite;

    [Header("Burnt Sprites")]
    public Sprite burntBottomBunSprite;
    public Sprite burntPattySprite;
    public Sprite burntTopBunSprite;

    void LateUpdate()
    {
        if (pan == null || panRenderer == null)
            return;

        if (pan.PanItem == ItemType.None)
        {
            panRenderer.enabled = false;
            return;
        }

        panRenderer.enabled = true;

        // If burnt → show corresponding burnt sprite
        if (pan.IsBurnt)
        {
            switch (pan.PanItem)
            {
                case ItemType.BottomBun:
                    panRenderer.sprite = burntBottomBunSprite;
                    break;

                case ItemType.Patty:
                    panRenderer.sprite = burntPattySprite;
                    break;

                case ItemType.TopBun:
                    panRenderer.sprite = burntTopBunSprite;
                    break;
            }

            return;
        }

        // Otherwise show normal sprite
        switch (pan.PanItem)
        {
            case ItemType.BottomBun:
                panRenderer.sprite = bottomBunSprite;
                break;

            case ItemType.Patty:
                panRenderer.sprite = pattySprite;
                break;

            case ItemType.TopBun:
                panRenderer.sprite = topBunSprite;
                break;
        }
    }
}
