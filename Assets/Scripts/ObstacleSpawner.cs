using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance;

    [Header("Prefab")]
    public GameObject obstaclePrefab;

    [Header("Spawn Settings")]
    public float spawnY = 7f;
    public float minInterval = 0.5f;
    public float startInterval = 2.5f;

    private float[] lanes = { -2.3f, 0f, 2.3f };
    private float timer = 0f;

    // Track all live obstacles for cleanup
    private List<GameObject> activeObstacles = new List<GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = GetInterval();
        }
    }

    void Spawn()
    {
        int laneIndex = Random.Range(0, lanes.Length);
        Vector3 spawnPos = new Vector3(lanes[laneIndex], spawnY, 0f);
        GameObject obs = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        activeObstacles.Add(obs);
    }

    float GetInterval()
    {
        float t = GameManager.Instance.ElapsedTime;
        float interval = startInterval - (t * 0.02f);
        return Mathf.Max(interval, minInterval);
    }

    // Called on soft reset — destroy all live obstacles
    public void ClearObstacles()
    {
        // Clean up destroyed entries first
        activeObstacles.RemoveAll(o => o == null);

        foreach (GameObject obs in activeObstacles)
            if (obs != null) Destroy(obs);

        activeObstacles.Clear();
        timer = 0f;
    }
}