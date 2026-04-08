using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public GameObject hudPanel;
    public TMP_Text scoreText;
    public TMP_Text speedText;
    public TMP_Text timerText;

    [Header("Start Screen")]
    public GameObject startPanel;

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public CanvasGroup gameOverCanvasGroup;  // add CanvasGroup to gameOverPanel

    [Header("Fade Settings")]
    public float fadeInDuration = 0.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        startPanel.SetActive(true);

        if (gameOverCanvasGroup != null)
            gameOverCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        scoreText.text = "SCORE: " + GameManager.Instance.Score.ToString("D5");
        speedText.text = "SPEED: " + Mathf.RoundToInt(GameManager.Instance.CurrentSpeed * 10f) + "km/h";

        float remaining = Mathf.Max(0f, GameManager.Instance.crashTime - GameManager.Instance.ElapsedTime);
        int mins = Mathf.FloorToInt(remaining / 60f);
        int secs = Mathf.FloorToInt(remaining % 60f);
        timerText.text = string.Format("{0}:{1:00}", mins, secs);
    }

    public void ShowHUD()
    {
        startPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        finalScoreText.text = "SCORE: " + GameManager.Instance.Score.ToString("D5");
        gameOverPanel.SetActive(true);
        StartCoroutine(FadeInGameOver());
    }

    IEnumerator FadeInGameOver()
    {
        if (gameOverCanvasGroup == null) yield break;

        float elapsed = 0f;
        gameOverCanvasGroup.alpha = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            gameOverCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        gameOverCanvasGroup.alpha = 1f;
    }

    // Wired to Start button
    public void OnStartButton()
    {
        GameManager.Instance.StartGame();
    }

    // Wired to Restart button
    public void OnRestartButton()
    {
        GameManager.Instance.RestartGame();
    }
}