using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject settingsRoot;
        [SerializeField] private GameObject audioPanel;
        [SerializeField] private GameObject videoPanel;
        [SerializeField] private GameObject controlsPanel;

        [Header("Tab Buttons")]
        [SerializeField] private Button audioTabButton;
        [SerializeField] private Button videoTabButton;
        [SerializeField] private Button controlsTabButton;

        [Header("Audio")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Video")]
        [SerializeField] private Toggle fullscreenToggle;

        private void OnEnable()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MasterVolume", 1f));
            if (musicVolumeSlider != null)
                musicVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("MusicVolume", 1f));
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat("SFXVolume", 1f));
            if (fullscreenToggle != null)
                fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);

            if (audioPanel != null)
                ShowAudio();
        }

        public void Open()
        {
            if (settingsRoot != null)
                settingsRoot.SetActive(true);
        }

        public void Close()
        {
            if (settingsRoot != null)
                settingsRoot.SetActive(false);
        }

        public void ShowAudio() => ShowPanel(audioPanel);
        public void ShowVideo() => ShowPanel(videoPanel);
        public void ShowControls() => ShowPanel(controlsPanel);

        public void SetMasterVolume(float value)
        {
            AudioListener.volume = value;
            PlayerPrefs.SetFloat("MasterVolume", value);
        }

        public void SetMusicVolume(float value)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        public void SetSfxVolume(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
        }

        public void SetFullscreen(bool value)
        {
            Screen.fullScreen = value;
        }

        private void ShowPanel(GameObject target)
        {
            if (audioPanel != null) audioPanel.SetActive(false);
            if (videoPanel != null) videoPanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
            if (target != null) target.SetActive(true);
        }
    }
}
