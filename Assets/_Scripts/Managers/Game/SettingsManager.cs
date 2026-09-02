using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Settings UI")]
    [SerializeField] private TMP_Dropdown _resolutionsDropdown;
    [SerializeField] private Toggle _fullscreenToggle;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private Resolution[] _resolutions;

    // FMOD VARIABLES
    private FMOD.Studio.VCA _musicVCA;
    private FMOD.Studio.VCA _sfxVCA;

    // --------------------------------------------------------------

    private void Start()
    {
        // Get the VCA for music & sfx
        _musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music"); // ASSUMING directory is that
        _sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");

        SetupResolutionDropdown();
        LoadSettings();
    }

    // --- RESOLUTION ---

    private void SetupResolutionDropdown()
    {
        // Get the user's resolutions
        _resolutions = Screen.resolutions;

        // Clear the dropdown
        _resolutionsDropdown.ClearOptions();

        // A list of the resolution options
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            // Format the option and add to the list
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        _resolutionsDropdown.AddOptions(options);
        _resolutionsDropdown.value = currentResolutionIndex;
        _resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    // --- AUDIO ---

    public void SetMusicVolume(float volume)
    {
        // Set volume here using the volume parameter
        _musicVCA.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        // Same again here but for SFX
        _sfxVCA.setVolume(volume);
    }

    // --- SAVE & LOAD ---

    public void SaveSettings()
    {
        // Set the player prefs and save them
        PlayerPrefs.SetInt("ResolutionPreference", _resolutionsDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _sfxSlider.value);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionPreference"))
        {
            // Get the saved resolution and set it
            _resolutionsDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
            SetResolution(_resolutionsDropdown.value);
        }

        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            bool isFullscreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreferences"));
            _fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume");
            _musicSlider.value = musicVol;
            SetMusicVolume(musicVol);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume");
            _sfxSlider.value = sfxVol;
            SetSFXVolume(sfxVol);
        }
    }
}