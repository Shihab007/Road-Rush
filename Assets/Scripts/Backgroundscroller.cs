using UnityEngine;
using DG.Tweening;

public class BackgroundScroller : MonoBehaviour
{
    [Header("Tiles")]
    public Transform tileA;
    public Transform tileB;

    [Header("Scroll Settings")]
    public float idleSpeed = 3f;
    public float gameSpeedMultiplier = 0.4f;

    [Header("Tuning")]
    public float tileOverlap = 0.5f;

    private float tileHeight;
    private float camBottom;
    private float camTop;
    private float currentSpeed;

    void Start()
    {
        if (tileA == null || tileB == null)
        {
            Debug.LogError("BackgroundScroller: Assign both tiles.");
            enabled = false;
            return;
        }

        Camera cam = Camera.main;
        camBottom = cam.transform.position.y - cam.orthographicSize;
        camTop    = cam.transform.position.y + cam.orthographicSize;

        tileHeight = tileA.GetComponent<SpriteRenderer>().bounds.size.y;

        // Cover full camera from start
        float startA = camBottom + (tileHeight * 0.5f);
        tileA.position = new Vector3(tileA.position.x, startA, tileA.position.z);
        tileB.position = new Vector3(tileB.position.x, startA + tileHeight - tileOverlap, tileB.position.z);

        currentSpeed = idleSpeed;
        StartScroll();
    }

    void Update()
    {
        float targetSpeed = (GameManager.Instance != null && GameManager.Instance.isPlaying)
            ? GameManager.Instance.CurrentSpeed * gameSpeedMultiplier
            : idleSpeed;

        if (Mathf.Abs(targetSpeed - currentSpeed) > 0.1f)
        {
            currentSpeed = targetSpeed;
            DOTween.Kill(gameObject);
            StartScroll();
        }
    }

    void StartScroll()
    {
        if (currentSpeed <= 0f) return;
        TweenTile(tileA);
        TweenTile(tileB);
    }

    void TweenTile(Transform tile)
    {
        if (currentSpeed <= 0f) return;

        // How far until this tile's top edge hits camera bottom
        float tileTop    = tile.position.y + (tileHeight * 0.5f);
        float distToExit = tileTop - camBottom;

        if (distToExit <= 0f)
        {
            // Already off screen — snap above camera top immediately
            Respawn(tile);
            TweenTile(tile);
            return;
        }

        float duration = distToExit / currentSpeed;

        tile.DOMoveY(tile.position.y - distToExit, duration)
            .SetEase(Ease.Linear)
            .SetId(gameObject)
            .OnComplete(() =>
            {
                // Tile top just hit camera bottom — snap instantly above camera top
                Respawn(tile);
                TweenTile(tile);
            });
    }

    void Respawn(Transform tile)
    {
        // Place tile so its bottom edge sits at camera top — seamless entry
        float spawnY = camTop + (tileHeight * 0.5f) - tileOverlap;
        tile.position = new Vector3(tile.position.x, spawnY, tile.position.z);
    }

    void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}