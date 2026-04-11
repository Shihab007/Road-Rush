using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Start Screen")]
    public GameObject startPanel;
    public TMP_Text   songStatusText;
    public GameObject playButton;

    [Header("HUD")]
    public GameObject hudPanel;
    public TMP_Text   scoreText;
    public TMP_Text   speedText;
    public TMP_Text   timerText;

    [Header("Game Over Screen")]
    public GameObject  gameOverPanel;
    public TMP_Text    finalScoreText;
    public CanvasGroup gameOverCanvasGroup;

    [Header("Fade")]
    public float fadeInDuration = 0.5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        ShowStartScreen();

        if (gameOverCanvasGroup != null)
            gameOverCanvasGroup.alpha = 0f;

        // If fallback clip already loaded, unlock play immediately
        if (AudioManager.Instance.SongReady)
            OnSongReady(AudioManager.Instance.SongDuration);
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        scoreText.text = "SCORE  " + GameManager.Instance.Score.ToString("D5");
        speedText.text = Mathf.RoundToInt(GameManager.Instance.CurrentSpeed * 10f) + " km/h";

        // Count UP
        float elapsed = GameManager.Instance.ElapsedTime;
        int mins = Mathf.FloorToInt(elapsed / 60f);
        int secs = Mathf.FloorToInt(elapsed % 60f);
        timerText.text = string.Format("{0}:{1:00}", mins, secs);
    }

    // ── Panel helpers ──────────────────────────────────────────

    void ShowStartScreen()
    {
        startPanel.SetActive(true);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (playButton != null) playButton.SetActive(false);
        if (songStatusText != null) songStatusText.text = "Choose a song to begin";
    }

    public void ShowHUD()
    {
        startPanel.SetActive(false);
        hudPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        finalScoreText.text = "SCORE  " + GameManager.Instance.Score.ToString("D5");
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

    // ── Song loading feedback ──────────────────────────────────

    public void OnSongLoading()
    {
        if (songStatusText != null) songStatusText.text = "Loading...";
        if (playButton != null)     playButton.SetActive(false);
    }

    public void OnSongReady(float duration)
    {
        int mins = Mathf.FloorToInt(duration / 60f);
        int secs = Mathf.FloorToInt(duration % 60f);

        if (songStatusText != null)
            songStatusText.text = string.Format("Ready  {0}:{1:00}", mins, secs);

        if (playButton != null)
            playButton.SetActive(true);
    }

    public void OnSongError()
    {
        if (songStatusText != null) songStatusText.text = "Failed to load. Try again.";
        if (playButton != null)     playButton.SetActive(false);
    }

    // ── Button callbacks ───────────────────────────────────────

    // "Choose Song" button
    public void OnChooseSongButton()
    {
        AudioManager.Instance.RequestFilePicker();
    }

    // "Play" button
    public void OnPlayButton()
    {
        if (!AudioManager.Instance.SongReady) return;
        GameManager.Instance.StartGame();
    }

    // "Play Again" button — back to start screen, song stays loaded
    public void OnPlayAgainButton()
    {
        ObstacleSpawner.Instance.ClearObstacles();
        RoadScroller.Instance.ResetTiles();
        PlayerCar.Instance.ResetPosition();
        CameraShake.Instance.ResetPosition();

        if (gameOverCanvasGroup != null)
            gameOverCanvasGroup.alpha = 0f;

        ShowStartScreen();

        // Song is still loaded — re-show the ready state
        if (AudioManager.Instance.SongReady)
            OnSongReady(AudioManager.Instance.SongDuration);
    }
}