using UnityEngine;
using System.Collections.Generic;

namespace KaiAlphabetAdventure.Core
{
    /// <summary>
    /// Manages all audio including music and sound effects.
    /// Supports: REQ-4.1.1, REQ-4.1.2, REQ-4.1.3, REQ-4.1.4, REQ-4.2.1, REQ-4.3.1, REQ-4.3.2
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music Tracks")]
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField] private AudioClip victoryMusic;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip letterCollectSFX;
        [SerializeField] private AudioClip wordCompleteSFX;
        [SerializeField] private AudioClip footstepSFX;
        [SerializeField] private AudioClip buttonClickSFX;
        [SerializeField] private AudioClip interactionSFX;
        [SerializeField] private AudioClip npcHitSFX;
        [SerializeField] private AudioClip letterDropSFX;

        [Header("Volume Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float musicVolume = 0.7f;
        [SerializeField] private float sfxVolume = 1f;

        private bool musicMuted = false;
        private bool sfxMuted = false;

        // Object pooling for sound effects
        private List<AudioSource> sfxPool = new List<AudioSource>();
        private const int SFX_POOL_SIZE = 10;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
            InitializeSFXPool();
            LoadAudioSettings();
        }

        private void InitializeAudioSources()
        {
            // Create music source if not assigned
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            // Create SFX source if not assigned
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        private void InitializeSFXPool()
        {
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.loop = false;
                source.playOnAwake = false;
                sfxPool.Add(source);
            }
        }

        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            sfxMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

            ApplyVolumeSettings();
        }

        private void ApplyVolumeSettings()
        {
            musicSource.volume = musicMuted ? 0 : masterVolume * musicVolume;
            sfxSource.volume = sfxMuted ? 0 : masterVolume * sfxVolume;

            foreach (var source in sfxPool)
            {
                source.volume = sfxMuted ? 0 : masterVolume * sfxVolume;
            }
        }

        // Music Controls
        public void PlayMusic(AudioClip clip, bool fadeIn = true)
        {
            if (clip == null) return;

            if (fadeIn && musicSource.isPlaying)
            {
                StartCoroutine(CrossfadeMusic(clip));
            }
            else
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
        }

        public void PlayMainMenuMusic()
        {
            PlayMusic(mainMenuMusic);
        }

        public void PlayGameplayMusic()
        {
            PlayMusic(gameplayMusic);
        }

        public void PlayVictoryMusic()
        {
            PlayMusic(victoryMusic);
        }

        public void StopMusic(bool fadeOut = true)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOutMusic());
            }
            else
            {
                musicSource.Stop();
            }
        }

        private System.Collections.IEnumerator CrossfadeMusic(AudioClip newClip, float duration = 1f)
        {
            float startVolume = musicSource.volume;

            // Fade out
            for (float t = 0; t < duration / 2; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / (duration / 2));
                yield return null;
            }

            // Switch clip
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in
            for (float t = 0; t < duration / 2; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, startVolume, t / (duration / 2));
                yield return null;
            }

            musicSource.volume = startVolume;
        }

        private System.Collections.IEnumerator FadeOutMusic(float duration = 1f)
        {
            float startVolume = musicSource.volume;

            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = startVolume;
        }

        // Sound Effect Controls
        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null || sfxMuted) return;

            // Get available audio source from pool
            AudioSource availableSource = GetAvailableSFXSource();
            if (availableSource != null)
            {
                availableSource.clip = clip;
                availableSource.volume = (sfxMuted ? 0 : masterVolume * sfxVolume) * volumeMultiplier;
                availableSource.Play();
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (var source in sfxPool)
            {
                if (!source.isPlaying)
                    return source;
            }
            return sfxPool[0]; // Return first if all busy
        }

        // Specific sound effects
        public void PlayLetterCollectSound() => PlaySFX(letterCollectSFX);
        public void PlayWordCompleteSound() => PlaySFX(wordCompleteSFX);
        public void PlayFootstepSound() => PlaySFX(footstepSFX, 0.5f);
        public void PlayButtonClickSound() => PlaySFX(buttonClickSFX);
        public void PlayInteractionSound() => PlaySFX(interactionSFX);
        public void PlayNPCHitSound() => PlaySFX(npcHitSFX);
        public void PlayLetterDropSound() => PlaySFX(letterDropSFX);

        // Volume Controls
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }

        public void ToggleMusicMute()
        {
            musicMuted = !musicMuted;
            ApplyVolumeSettings();
            PlayerPrefs.SetInt("MusicMuted", musicMuted ? 1 : 0);
        }

        public void ToggleSFXMute()
        {
            sfxMuted = !sfxMuted;
            ApplyVolumeSettings();
            PlayerPrefs.SetInt("SFXMuted", sfxMuted ? 1 : 0);
        }

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;
        public bool IsMusicMuted() => musicMuted;
        public bool IsSFXMuted() => sfxMuted;
    }
}
