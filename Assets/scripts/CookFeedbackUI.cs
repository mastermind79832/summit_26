using System.Collections;
using UnityEngine;
using TMPro;

public class CookFeedbackUI : MonoBehaviour
{
    public PanController pan;

    [Header("UI")]
    public TMP_Text feedbackText;
    public float showSeconds = 0.8f;

    Coroutine routine;

    void Start()
    {
        if (feedbackText) feedbackText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (pan != null) pan.OnCookResult += OnCookResult;
    }

    void OnDisable()
    {
        if (pan != null) pan.OnCookResult -= OnCookResult;
    }

    void OnCookResult(PanController.CookResult result)
    {
        if (feedbackText == null) return;

        string msg = result switch
        {
            PanController.CookResult.Undercooked => "UNDERCOOKED",
            PanController.CookResult.Perfect     => "PERFECT",
            PanController.CookResult.Burnt       => "BURNT",
            _ => ""
        };

        if (string.IsNullOrEmpty(msg)) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(msg));
    }

    IEnumerator ShowRoutine(string msg)
    {
        feedbackText.text = msg;
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(showSeconds); // works even if F1 pauses
        feedbackText.gameObject.SetActive(false);
    }
}
