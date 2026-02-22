using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Volume Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private void Start()
    {
        SetupQualityDropdown();
        SetupResolutionDropdown();
        LoadSettings();
    }

    #region Audio Settings

    public void SetMasterVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat("MasterVolume", dB);
        }

        if (masterVolumeText != null)
        {
            masterVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        }

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat("MusicVolume", dB);
        }

        if (musicVolumeText != null)
        {
            musicVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer != null)
        {
            float dB = volume > 0.0001f ? 20f * Mathf.Log10(volume) : -80f;
            audioMixer.SetFloat("SFXVolume", dB);
        }

        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = Mathf.RoundToInt(volume * 100) + "%";
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    #endregion

    #region Quality Settings

    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();
        
        // Récupérer les noms des Quality Levels de Unity
        List<string> qualityNames = new List<string>();
        string[] names = QualitySettings.names;
        
        foreach (string name in names)
        {
            qualityNames.Add(name);
        }

        qualityDropdown.AddOptions(qualityNames);
        
        // Définir la qualité actuelle
        int currentQuality = QualitySettings.GetQualityLevel();
        qualityDropdown.value = currentQuality;
        qualityDropdown.RefreshShownValue();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
        
        Debug.Log($"Quality set to: {QualitySettings.names[qualityIndex]}");
    }

    #endregion

    #region Resolution Settings

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        // Filtrer les résolutions pour éviter les doublons (même résolution, différents refresh rates)
        float currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;
        
        for (int i = 0; i < resolutions.Length; i++)
        {
            float refreshRate = (float)resolutions[i].refreshRateRatio.value;
            
            // Ne garder que les résolutions avec le taux de rafraîchissement actuel
            if (Mathf.Approximately(refreshRate, currentRefreshRate))
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        // Tri par ordre décroissant (plus grande résolution en premier)
        filteredResolutions = filteredResolutions.OrderByDescending(r => r.width * r.height).ToList();

        int currentResolutionIndex = 0;
        
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            if (filteredResolutions[i].width == Screen.width &&
                filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count)
        {
            Debug.LogWarning("Invalid resolution index");
            return;
        }

        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        
        Debug.Log($"Resolution set to: {resolution.width}x{resolution.height}");
    }

    #endregion

    #region Fullscreen Settings

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log($"Fullscreen: {isFullscreen}");
    }

    #endregion

    #region Load Settings

    private void LoadSettings()
    {
        // Charger les volumes
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = masterVolume;
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
        }

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        // Charger la qualité
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        SetQuality(qualityLevel);
        
        if (qualityDropdown != null)
        {
            qualityDropdown.value = qualityLevel;
            qualityDropdown.RefreshShownValue();
        }

        // Charger la résolution
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", filteredResolutions.Count - 1);
        
        if (resolutionIndex >= 0 && resolutionIndex < filteredResolutions.Count)
        {
            SetResolution(resolutionIndex);
            
            if (resolutionDropdown != null)
            {
                resolutionDropdown.value = resolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }
        }

        // Charger fullscreen
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        SetFullscreen(isFullscreen);
        
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
        }
    }

    #endregion

    #region Reset Settings

    public void ResetToDefault()
    {
        // Reset volumes
        SetMasterVolume(1f);
        SetMusicVolume(0.8f);
        SetSFXVolume(0.8f);

        if (masterVolumeSlider != null) masterVolumeSlider.value = 1f;
        if (musicVolumeSlider != null) musicVolumeSlider.value = 0.8f;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = 0.8f;

        // Reset quality to highest
        int highestQuality = QualitySettings.names.Length - 1;
        SetQuality(highestQuality);
        if (qualityDropdown != null)
        {
            qualityDropdown.value = highestQuality;
            qualityDropdown.RefreshShownValue();
        }

        // Reset to native resolution
        if (filteredResolutions.Count > 0)
        {
            SetResolution(filteredResolutions.Count - 1);
            if (resolutionDropdown != null)
            {
                resolutionDropdown.value = filteredResolutions.Count - 1;
                resolutionDropdown.RefreshShownValue();
            }
        }

        // Reset fullscreen
        SetFullscreen(true);
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = true;
        }

        PlayerPrefs.Save();
        Debug.Log("Settings reset to default");
    }

    #endregion
}
