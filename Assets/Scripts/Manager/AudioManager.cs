using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

namespace VisualNovel.GameJam.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Mixer Volume Parameters")]
        private const string MASTER_VOL = "MasterVolume";
        private const string MUSIC_VOL = "MusicVolume";
        private const string SFX_VOL = "SFXVolume";

        private const string PREF_MASTER = "Volume_Master";
        private const string PREF_MUSIC = "Volume_Music";
        private const string PREF_SFX = "Volume_SFX";

        [Header("Music Settings")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private float bgmFadeDuration = 1f;

        [Header("SFX Pool Settings")]
        [SerializeField] private GameObject sfxSourcePrefab;
        [SerializeField] private Transform sfxParent;
        [SerializeField] private int initialSFXPoolSize = 5;

        [Header("BGM List")]
        [SerializeField] private List<AudioEntry> bgmList = new();

        [Header("SFX List")]
        [SerializeField] private List<AudioEntry> sfxList = new();

        private readonly List<AudioSource> sfxPool = new();
        private Dictionary<string, AudioClip> bgmDict;
        private Dictionary<string, AudioClip> sfxDict;

        private AudioClip currentBGM;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitSFXPool();
            InitAudioDictionaries();

            LoadVolumeSettings();
        }

        #region Init
        private void InitSFXPool()
        {
            for (int i = 0; i < initialSFXPoolSize; i++)
            {
                CreateSFXSource();
            }
        }

        private AudioSource CreateSFXSource()
        {
            GameObject obj = Instantiate(sfxSourcePrefab, sfxParent);
            AudioSource src = obj.GetComponent<AudioSource>();
            src.playOnAwake = false;
            sfxPool.Add(src);
            return src;
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var src in sfxPool)
            {
                if (!src.isPlaying) return src;
            }

            return CreateSFXSource();
        }

        private void InitAudioDictionaries()
        {
            bgmDict = new Dictionary<string, AudioClip>();

            foreach (var entry in bgmList)
            {
                if (!string.IsNullOrEmpty(entry.id) && entry.clip != null) bgmDict[entry.id] = entry.clip;
            }

            sfxDict = new Dictionary<string, AudioClip>();

            foreach (var entry in sfxList)
            {
                if (!string.IsNullOrEmpty(entry.id) && entry.clip != null) sfxDict[entry.id] = entry.clip;
            }
        }
        #endregion

        #region Save/Load Volume Settings
        private void LoadVolumeSettings()
        {
            float master = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
            float music = PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
            float sfx = PlayerPrefs.GetFloat(PREF_SFX, 1f);

            SetMasterVolume(master);
            SetMusicVolume(music);
            SetSFXVolume(sfx);
        }
        #endregion

        #region Volume Control
        public void SetMasterVolume(float volume)
        {
            float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat(MASTER_VOL, db);

            PlayerPrefs.SetFloat(PREF_MASTER, volume);
            PlayerPrefs.Save();
        }

        public float GetMasterVolume()
        {
            return PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        }

        public void SetMusicVolume(float volume)
        {
            float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat(MUSIC_VOL, db);

            PlayerPrefs.SetFloat(PREF_MUSIC, volume);
            PlayerPrefs.Save();
        }

        public float GetMusicVolume()
        {
            return PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
        }

        public void SetSFXVolume(float volume)
        {
            float db = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat(SFX_VOL, db);

            PlayerPrefs.SetFloat(PREF_SFX, volume);
            PlayerPrefs.Save();
        }

        public float GetSFXVolume()
        {
            return PlayerPrefs.GetFloat(PREF_SFX, 1f);
        }
        #endregion

        #region BGM Playback
        public void PlayBGM(string id, bool loop = true)
        {
            if (bgmDict.TryGetValue(id, out var clip))
            {
                PlayBGM(clip, loop);
            }
            else
            {
                Debug.LogWarning($"BGM ID '{id}' not found.");
            }
        }

        private void PlayBGM(AudioClip clip, bool loop = true)
        {
            if (clip == null || (currentBGM == clip && bgmSource.isPlaying)) return;

            currentBGM = clip;
            StartCoroutine(FadeAndPlayBGM(clip, loop));
        }

        private IEnumerator FadeAndPlayBGM(AudioClip newClip, bool loop)
        {
            float originalVol = bgmSource.volume;

            yield return bgmSource.DOFade(0f, bgmFadeDuration).WaitForCompletion();

            bgmSource.clip = newClip;
            bgmSource.loop = loop;
            bgmSource.Play();

            yield return bgmSource.DOFade(originalVol, bgmFadeDuration).WaitForCompletion();
        }
        #endregion

        #region SFX Playback
        public void PlaySFX(string id)
        {
            if (sfxDict.TryGetValue(id, out var clip))
            {
                PlaySFX(clip);
            }
            else
            {
                Debug.LogWarning($"SFX ID '{id}' not found.");
            }
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;

            AudioSource src = GetAvailableSFXSource();
            src.PlayOneShot(clip);
        }
        #endregion

        [Serializable]
        public struct AudioEntry
        {
            public string id;
            public AudioClip clip;
        }
    }
}