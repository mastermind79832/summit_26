using UnityEngine;
using UnityEngine.UI;

public class CookBarColorUI : MonoBehaviour
{
    public PanController pan;
    public Image cookFill;

    [Header("Gradient Colors")]
    public Color underColor = Color.yellow; // before perfect
    public Color perfectColor = Color.green;
    public Color overColor = Color.red;

    [Header("Soft Edge Width (seconds of cookProgress)")]
    public float blendWidth = 0.25f; // how smooth the transition is

    void Update()
    {
        if (pan == null || cookFill == null) return;

        if (pan.PanItem == ItemType.None)
            return;

        if (pan.IsBurnt)
        {
            cookFill.color = overColor;
            return;
        }

        float p = pan.CookProgress;
        float min = pan.PerfectMin;
        float max = pan.PerfectMax;

        cookFill.color = EvaluateColor(p, min, max, blendWidth);
    }

    Color EvaluateColor(float p, float min, float max, float w)
    {
        // Region A: far before perfect -> underColor
        if (p <= min - w)
            return underColor;

        // Region B: blend under -> perfect
        if (p < min + w)
        {
            float t = Mathf.InverseLerp(min - w, min + w, p);
            return Color.Lerp(underColor, perfectColor, t);
        }

        // Region C: solid perfect
        if (p <= max - w)
            return perfectColor;

        // Region D: blend perfect -> over
        if (p < max + w)
        {
            float t = Mathf.InverseLerp(max - w, max + w, p);
            return Color.Lerp(perfectColor, overColor, t);
        }

        // Region E: far after perfect -> overColor
        return overColor;
    }
}
