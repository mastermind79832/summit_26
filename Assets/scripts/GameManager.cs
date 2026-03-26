using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Refs")]
    public PlateController plate;
    public CustomerManager customers;
    public PanController pan;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text scoreText;

    [Header("Game Over UI")]
    public GameObject gameOverRoot;     // assign GameOverPanel here

    [Header("Game Settings")]
    public float gameDurationSeconds = 120f;

    public bool IsRunning { get; private set; }
    public bool IsGameOver { get; private set; }
    public int Score { get; private set; }

    float timeLeft;

    void Awake()
    {
        if (plate != null)
            plate.OnBurgerServed += HandleBurgerServed;

        ShowGameOver(false);
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
            GameOver();
            return;
        }

        UpdateTimerUI();
    }

    // Hook to Start button
    public void StartGame()
    {
        IsRunning = true;
        IsGameOver = false;

        Score = 0;
        timeLeft = gameDurationSeconds;

        plate?.ResetPlate();
        pan?.HardReset();
        customers?.StartLoop();

        ShowGameOver(false);
        UpdateUI();
    }

    // Hook to End button (manual stop)
    public void EndGame()
    {
        IsRunning = false;

        customers?.StopLoop();
        // Not a "Game Over" unless you want it to be:
        // ShowGameOver(true);

        UpdateUI();
    }

    void GameOver()
    {
        IsRunning = false;
        IsGameOver = true;

        customers?.StopLoop();

        ShowGameOver(true);
        UpdateUI();
    }

    void HandleBurgerServed()
    {
        if (!IsRunning) return;

        Score++;
        UpdateScoreUI();
        customers?.OnServed();
    }

    void ShowGameOver(bool show)
    {
        if (gameOverRoot) gameOverRoot.SetActive(show);
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