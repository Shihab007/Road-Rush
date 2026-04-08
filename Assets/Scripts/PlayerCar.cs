using UnityEngine;
using System.Collections;

public class PlayerCar : MonoBehaviour
{
    public static PlayerCar Instance;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float leftLimit = -2.5f;
    public float rightLimit = 2.5f;

    [Header("Crash Flash")]
    public int flashCount = 5;
    public float flashInterval = 0.1f;

    private float inputDirection = 0f;
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        startPosition = transform.position;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        float keyInput = Input.GetAxisRaw("Horizontal");
        float direction = (keyInput != 0f) ? keyInput : inputDirection;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + direction * moveSpeed * Time.deltaTime, leftLimit, rightLimit);
        transform.position = pos;
    }

    public void SetInput(float direction) => inputDirection = direction;
    public void PressLeft() => inputDirection = -1f;
    public void PressRight() => inputDirection = 1f;
    public void Release() => inputDirection = 0f;

    // Called on soft reset
    public void ResetPosition()
    {
        transform.position = startPosition;
        inputDirection = 0f;

        // Restore original color
        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
            GameManager.Instance.TriggerCrash();
    }

    public void StartCrashFlash()
    {
        StartCoroutine(CrashFlash());
    }

    IEnumerator CrashFlash()
    {
        Color originalColor = Color.white;
        Color crashColor = Color.red;

        for (int i = 0; i < flashCount; i++)
        {
            if (spriteRenderer != null) spriteRenderer.color = crashColor;
            yield return new WaitForSeconds(flashInterval);
            if (spriteRenderer != null) spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(flashInterval);
        }

        if (spriteRenderer != null) spriteRenderer.color = crashColor;
    }
}