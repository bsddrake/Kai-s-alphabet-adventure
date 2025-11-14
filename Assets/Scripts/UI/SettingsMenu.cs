using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KaiAlphabetAdventure.Core;

namespace KaiAlphabetAdventure.UI
{
    /// <summary>
    /// Settings menu for audio and display options.
    /// Supports: REQ-3.2.3, REQ-4.3.1, REQ-4.3.2
    /// </summary>
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Audio Controls")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle musicMuteToggle;
        [SerializeField] private Toggle sfxMuteToggle;

        [Header("Display Controls")]
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        [Header("Value Displays")]
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        private void Start()
        {
            InitializeControls();
            LoadSettings();
        }

        private void InitializeControls()
        {
            // Audio sliders
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

            // Mute toggles
            if (musicMuteToggle != null)
                musicMuteToggle.onValueChanged.AddListener(OnMusicMuteToggled);

            if (sfxMuteToggle != null)
                sfxMuteToggle.onValueChanged.AddListener(OnSFXMuteToggled);

            // Display controls
            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);

            if (resolutionDropdown != null)
            {
                PopulateResolutionDropdown();
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
            }
        }

        private void LoadSettings()
        {
            if (AudioManager.Instance != null)
            {
                if (masterVolumeSlider != null)
                {
                    masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
                    UpdateVolumeText(masterVolumeText, masterVolumeSlider.value);
                }

                if (musicVolumeSlider != null)
                {
                    musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
                    UpdateVolumeText(musicVolumeText, musicVolumeSlider.value);
                }

                if (sfxVolumeSlider != null)
                {
                    sfxVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
                    UpdateVolumeText(sfxVolumeText, sfxVolumeSlider.value);
                }

                if (musicMuteToggle != null)
                    musicMuteToggle.isOn = AudioManager.Instance.IsMusicMuted();

                if (sfxMuteToggle != null)
                    sfxMuteToggle.isOn = AudioManager.Instance.IsSFXMuted();
            }

            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;
        }

        private void OnMasterVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMasterVolume(value);
            UpdateVolumeText(masterVolumeText, value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance?.SetMusicVolume(value);
            UpdateVolumeText(musicVolumeText, value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance?.SetSFXVolume(value);
            UpdateVolumeText(sfxVolumeText, value);
            AudioManager.Instance?.PlayButtonClickSound(); // Preview SFX volume
        }

        private void OnMusicMuteToggled(bool muted)
        {
            AudioManager.Instance?.ToggleMusicMute();
        }

        private void OnSFXMuteToggled(bool muted)
        {
            AudioManager.Instance?.ToggleSFXMute();
        }

        private void OnFullscreenToggled(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
        }

        private void PopulateResolutionDropdown()
        {
            if (resolutionDropdown == null) return;

            resolutionDropdown.ClearOptions();

            Resolution[] resolutions = Screen.resolutions;
            var options = new System.Collections.Generic.List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private void OnResolutionChanged(int index)
        {
            Resolution[] resolutions = Screen.resolutions;
            if (index >= 0 && index < resolutions.Length)
            {
                Resolution resolution = resolutions[index];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            }
        }

        private void UpdateVolumeText(TextMeshProUGUI text, float value)
        {
            if (text != null)
            {
                text.text = Mathf.RoundToInt(value * 100f) + "%";
            }
        }
    }
}
