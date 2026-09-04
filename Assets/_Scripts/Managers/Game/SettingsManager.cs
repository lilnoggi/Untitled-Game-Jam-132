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
    [SerializeField] private Toggle _musicMuteToggle;
    [SerializeField] private Toggle _sfxMuteToggle;

    private Resolution[] _resolutions;
    private bool _isMusicMuted;
    private bool _isSfxMuted;

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

    void OnDisable()
    {
        SaveSettings();
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

    public void ToggleMusicMute(bool isMuted)
    {
        _isMusicMuted = isMuted;

        // Force volume to 0 if muted, otherwise restore to whatever the slider is currently on
        _musicVCA.setVolume(_isMusicMuted ? 0f : _musicSlider.value);
    }

    public void SetSFXVolume(float volume)
    {
        // Same again here but for SFX
        _sfxVCA.setVolume(volume);
    }

    public void ToggleSfxMute(bool isMuted)
    {
        _isSfxMuted = isMuted;

        // Force volume to 0 if muted, otherwise restore to whatever the slider is currently on
        _sfxVCA.setVolume(_isSfxMuted ? 0f : _sfxSlider.value);
    }

    // --- SAVE & LOAD ---

    public void SaveSettings()
    {
        // Set the player prefs and save them
        PlayerPrefs.SetInt("ResolutionPreference", _resolutionsDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _sfxSlider.value);

        // Save the mute states (1 for true, 0 for false)
        PlayerPrefs.SetInt("MusicMute", _isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SfxMuted", _isSfxMuted ? 1 : 0);

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
            bool isFullscreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
            _fullscreenToggle.isOn = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }

        // Load mute states before loading the volume
        if (PlayerPrefs.HasKey("MusicMute"))
        {
            _isMusicMuted = PlayerPrefs.GetInt("MusicMute") == 1;
            if (_musicMuteToggle != null)
            {
                _musicMuteToggle.isOn = _isMusicMuted;
            }
        }

        if (PlayerPrefs.HasKey("SfxMute"))
        {
            _isSfxMuted = PlayerPrefs.GetInt("SfxMute") == 1;
            if (_sfxMuteToggle != null)
            {
                _sfxMuteToggle.isOn = _isSfxMuted;
            }
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

        _musicVCA.setVolume(_isMusicMuted ? 0f : _musicSlider.value);
        _sfxVCA.setVolume(_isSfxMuted ? 0f : _sfxSlider.value);
    }
}