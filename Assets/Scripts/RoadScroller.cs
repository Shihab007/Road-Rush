using UnityEngine;

public class RoadScroller : MonoBehaviour
{
    public static RoadScroller Instance;

    [Header("Tiles (assign both in Inspector)")]
    public Transform tileA;
    public Transform tileB;

    [Header("Tuning")]
    [Tooltip("Increase this if you see a gap between tiles")]
    public float tileOverlap = 0.1f;

    private float tileHeight;
    private Transform[] tiles;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (tileA == null || tileB == null)
        {
            Debug.LogError("RoadScroller: Assign BOTH tileA and tileB in Inspector.");
            enabled = false;
            return;
        }

        // Use renderer bounds which accounts for scale
        SpriteRenderer sr = tileA.GetComponent<SpriteRenderer>();
        tileHeight = sr.bounds.size.y;

        tiles = new Transform[] { tileA, tileB };
        ResetTiles();
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        float moveAmount = GameManager.Instance.CurrentSpeed * Time.deltaTime;

        foreach (Transform tile in tiles)
        {
            tile.position += Vector3.down * moveAmount;

            float tileTop   = tile.position.y + (tileHeight * 0.5f);
            float camBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;

            if (tileTop < camBottom)
            {
                float highestY = GetHighestTileY();
                // Subtract overlap to close any gap between tiles
                tile.position = new Vector3(
                    tile.position.x,
                    highestY + tileHeight - tileOverlap,
                    tile.position.z
                );
            }
        }
    }

    float GetHighestTileY()
    {
        float highest = float.MinValue;
        foreach (Transform tile in tiles)
            if (tile.position.y > highest) highest = tile.position.y;
        return highest;
    }

    public void ResetTiles()
    {
        if (tiles == null) return;
        tileA.position = new Vector3(0f, 0f, 0f);
        tileB.position = new Vector3(0f, tileHeight - tileOverlap, 0f);
    }
}