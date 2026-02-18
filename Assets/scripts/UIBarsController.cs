using UnityEngine;
using UnityEngine.UI;

public class UIBarsController : MonoBehaviour
{
    public PanController pan;

    [Header("UI Images (Fill Amount)")]
    public Image cookFill;
    public Image pressureFill;

    void Update()
    {
        if (pan == null) return;

        if (cookFill) cookFill.fillAmount = pan.Cook01;
        if (pressureFill) pressureFill.fillAmount = pan.Pressure01;
    }
}
