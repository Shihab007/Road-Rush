using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public static ObstacleSpawner Instance;

    [Header("Prefabs")]
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Settings")]
    public float spawnY       = 7f;
    public float minInterval  = 0.5f;
    public float startInterval = 2.5f;

    [Header("Lanes — X positions only, no center")]
    public float[] lanes = { -2.3f, 2.3f };

    private float timer = 0f;
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
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            Debug.LogWarning("ObstacleSpawner: No prefabs assigned.");
            return;
        }

        int laneIndex   = Random.Range(0, lanes.Length);
        int prefabIndex = Random.Range(0, obstaclePrefabs.Length);

        Vector3 spawnPos = new Vector3(lanes[laneIndex], spawnY, 0f);
        GameObject obs   = Instantiate(obstaclePrefabs[prefabIndex], spawnPos, Quaternion.identity);
        activeObstacles.Add(obs);
    }

    float GetInterval()
    {
        float t        = GameManager.Instance.ElapsedTime;
        float interval = startInterval - (t * 0.02f);
        return Mathf.Max(interval, minInterval);
    }

    public void ClearObstacles()
    {
        activeObstacles.RemoveAll(o => o == null);
        foreach (GameObject obs in activeObstacles)
            if (obs != null) Destroy(obs);
        activeObstacles.Clear();
        timer = 0f;
    }
}