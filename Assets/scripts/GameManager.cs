using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public PlateController plate;
    public CustomerManager customers;      // Step 6 will implement this
    public PanController pan;              // for resets / safety (optional)

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;

    [Header("Game Settings")]
    public float gameDurationSeconds = 120f;

    public bool IsRunning { get; private set; }
    public int Score { get; private set; }

    float timeLeft;

    void Awake()
    {
        if (plate != null)
            plate.OnBurgerServed += HandleBurgerServed;

        UpdateUI();
    }

    void OnDestroy()
    {
        if (plate != null)
            plate.OnBurgerServed -= HandleBurgerServed;
    }

    void Update()
    {
        if (!IsRunning) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            EndGame();
            return;
        }

        UpdateTimerUI();
    }

    // Hook this to Start button
    public void StartGame()
    {
        IsRunning = true;
        Score = 0;
        timeLeft = gameDurationSeconds;

        plate?.ResetPlate();
        pan?.HardReset(); // clears held item/pan state

        customers?.StartLoop(); // spawns customer and waits for serves

        UpdateUI();
    }

    // Hook this to End button
    public void EndGame()
    {
        IsRunning = false;

        customers?.StopLoop();

        UpdateUI();
    }

    void HandleBurgerServed()
    {
        if (!IsRunning) return;

        Score++;
        UpdateScoreUI();

        // Tell customer manager to leave and spawn next
        customers?.OnServed();
    }

    void UpdateUI()
    {
        UpdateTimerUI();
        UpdateScoreUI();
    }

    void UpdateTimerUI()
    {
        if (!timerText) return;
        int t = Mathf.CeilToInt(timeLeft);
        timerText.text = $"{t / 60:0}:{t % 60:00}";
    }

    void UpdateScoreUI()
    {
        if (!scoreText) return;
        scoreText.text = Score.ToString();
    }
}
