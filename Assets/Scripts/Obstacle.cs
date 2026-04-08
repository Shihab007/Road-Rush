using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float destroyY = -7f;

    void Update()
    {
        if (!GameManager.Instance.isPlaying) return;

        transform.position += Vector3.down * GameManager.Instance.CurrentSpeed * Time.deltaTime;

        if (transform.position.y < destroyY)
            Destroy(gameObject);
    }
}