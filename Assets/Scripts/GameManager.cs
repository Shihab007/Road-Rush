using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float baseSpeed = 5f;
    public float maxSpeed = 20f;
    public float speedIncreaseRate = 0.5f;
    public float crashTime = 60f;

    [Header("Crash Sequence")]
    public float crashDelay = 1.2f;

    [Header("State")]
    public bool isPlaying = false;
    public bool isCrashing = false;

    public float CurrentSpeed { get; private set; }
    public float ElapsedTime { get; private set; }
    public int Score { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartGame()
    {
        ResetState();
        isPlaying = true;

        AudioManager.Instance.PlaySong();
        UIManager.Instance.ShowHUD();
    }

    // Called by Restart button — no scene reload
    public void RestartGame()
    {
        ObstacleSpawner.Instance.ClearObstacles();
        RoadScroller.Instance.ResetTiles();
        PlayerCar.Instance.ResetPosition();
        CameraShake.Instance.ResetPosition();
        StartGame();
    }

    void ResetState()
    {
        CurrentSpeed = baseSpeed;
        ElapsedTime = 0f;
        Score = 0;
        isCrashing = false;
        isPlaying = false;
    }

    void Update()
    {
        if (!isPlaying) return;

        ElapsedTime += Time.deltaTime;
        Score += Mathf.RoundToInt(Time.deltaTime * CurrentSpeed * 10f);

        CurrentSpeed = Mathf.Min(baseSpeed + speedIncreaseRate * ElapsedTime, maxSpeed);
        ElapsedTime = Mathf.Min(ElapsedTime, crashTime);

        if (ElapsedTime >= crashTime)
            TriggerCrash();
    }

    public void TriggerCrash()
    {
        if (!isPlaying || isCrashing) return;

        isPlaying = false;
        isCrashing = true;
        CurrentSpeed = 0f;

        AudioManager.Instance.StopSong();
        StartCoroutine(CrashSequence());
    }

    IEnumerator CrashSequence()
    {
        PlayerCar.Instance.StartCrashFlash();
        CameraShake.Instance.Shake(0.4f, 0.18f);

        yield return new WaitForSeconds(crashDelay);

        isCrashing = false;
        UIManager.Instance.ShowGameOver();
    }
}