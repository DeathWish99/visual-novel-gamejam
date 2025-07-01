using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] private AudioClip[] soundEffects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(int index)
    {
        if (index >= 0 && index < musicTracks.Length)
        {
            musicSource.clip = musicTracks[index];
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < soundEffects.Length)
        {
            sfxSource.PlayOneShot(soundEffects[index]);
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
