using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
    }

    public void PlaySong()
    {
        if (audioSource.clip == null)
        {
            Debug.LogWarning("AudioManager: No AudioClip assigned.");
            return;
        }

        audioSource.Stop();
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void StopSong()
    {
        audioSource.Stop();
    }
}