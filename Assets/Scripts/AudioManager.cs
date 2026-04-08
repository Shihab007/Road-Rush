using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.InteropServices;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Song (loaded from file or fallback)")]
    public AudioClip fallbackClip;

    [Header("SFX")]
    public AudioClip engineSound;   // looping car rumble
    public AudioClip crashSound;    // one-shot on collision

    private AudioSource songSource;
    private AudioSource sfxSource;

    private float loadedDuration = 60f;

    public float SongDuration => loadedDuration;
    public bool  SongReady    { get; private set; } = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Song source — existing AudioSource on this GameObject
        songSource             = GetComponent<AudioSource>();
        songSource.loop        = false;
        songSource.playOnAwake = false;

        // SFX source — second AudioSource added in code
        sfxSource             = gameObject.AddComponent<AudioSource>();
        sfxSource.loop        = false;
        sfxSource.playOnAwake = false;

        if (fallbackClip != null)
        {
            songSource.clip = fallbackClip;
            loadedDuration  = fallbackClip.length;
            SongReady       = true;
        }
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void OpenAudioFilePicker();
#endif

    public void RequestFilePicker()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenAudioFilePicker();
#else
        if (fallbackClip != null)
        {
            songSource.clip = fallbackClip;
            loadedDuration  = fallbackClip.length;
            SongReady       = true;
            UIManager.Instance.OnSongReady(loadedDuration);
        }
        else
        {
            Debug.Log("AudioManager: Assign a fallback clip to test in Editor.");
        }
#endif
    }

    // Called by JS via SendMessage
    public void OnAudioFileLoaded(string payload)
    {
        if (payload.StartsWith("ERROR"))
        {
            UIManager.Instance.OnSongError();
            return;
        }

        string[] parts   = payload.Split('|');
        string   url     = parts[0];
        float    duration = float.Parse(parts[1],
                            System.Globalization.CultureInfo.InvariantCulture);

        loadedDuration = duration;
        StartCoroutine(LoadClip(url));
    }

    IEnumerator LoadClip(string url)
    {
        UIManager.Instance.OnSongLoading();

        using (UnityWebRequest www =
               UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                UIManager.Instance.OnSongError();
                yield break;
            }

            songSource.clip = DownloadHandlerAudioClip.GetContent(www);
            SongReady       = true;
            UIManager.Instance.OnSongReady(loadedDuration);
        }
    }

    // --- Song controls ---

    public void PlaySong()
    {
        if (songSource.clip == null) return;
        songSource.Stop();
        songSource.time = 0f;
        songSource.Play();

        PlayEngine();
    }

    public void StopSong()
    {
        songSource.Stop();
        StopEngine();
    }

    // --- Engine sound ---

    public void PlayEngine()
    {
        if (engineSound == null) return;
        sfxSource.clip = engineSound;
        sfxSource.loop = true;
        sfxSource.Play();
    }

    public void StopEngine()
    {
        if (sfxSource.isPlaying && sfxSource.loop)
            sfxSource.Stop();
    }

    // --- Crash sound ---

    public void PlayCrash()
    {
        if (crashSound == null) return;
        sfxSource.loop = false;
        sfxSource.PlayOneShot(crashSound);
    }
}