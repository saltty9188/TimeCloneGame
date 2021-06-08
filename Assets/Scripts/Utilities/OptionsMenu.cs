using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The OptionsMenu class is responsible for allowing the player to adjust resolution and volume settings.
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    #region Player pref key constants
    private const string RESOLUTION_PREF_KEY = "resolution";
    private const string WINDOWED_PREF_KEY = "windowed";
    #endregion

    #region Inspector fields
    [Tooltip("The button that opens this menu.")]
    [SerializeField] private GameObject _optionsButton;
    [Tooltip("The first option in the menu.")]
    [SerializeField] private GameObject _firstOption;
    [Tooltip("The dropdown holding the different resolutions.")]
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [Tooltip("The toggle for full screen vs windowed.")]
    [SerializeField] private Toggle _windowedToggle;
    [Tooltip("The master volume slider.")]
    [SerializeField] private Slider _masterVolume;
    [Tooltip("The music volume slider.")]
    [SerializeField] private Slider _musicVolume;
    [Tooltip("The sound effects volume slider.")]
    [SerializeField] private Slider _SFXVolume;
    [Tooltip("The main audio mixer.")]
    [SerializeField] private AudioMixer _audioMixer;
    #endregion

    #region Private fields
    private Resolution[] _resolutions;
    private bool _lastWindowSet;
    #endregion

    void Awake()
    {
        // Only want unique resolutions (no duplicates with different framerates)
        _resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height}).Distinct().ToArray();
        List<string> resolutionsText = new List<string>(_resolutions.Length);
        int currentIndex = -1;

        currentIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, -1);

        // populate the drop down
        for(int i = 0; i < _resolutions.Length; i++)
        {
            resolutionsText.Add(_resolutions[i].width + " x " + _resolutions[i].height);

            // By default set the resolution to the current screen resolution
            if(_resolutions[i].width == Screen.currentResolution.width
                && _resolutions[i].height == Screen.currentResolution.height && currentIndex == -1)
            {
                currentIndex = i;
            }
        }
        _resolutionDropdown.ClearOptions();
        _resolutionDropdown.AddOptions(resolutionsText);

        _resolutionDropdown.value = currentIndex;
        _resolutionDropdown.RefreshShownValue();

        //Set windowed status
        bool windowed = PlayerPrefs.GetInt(WINDOWED_PREF_KEY, 0) == 1;
        _windowedToggle.isOn = windowed;
        Screen.fullScreen = !windowed;
        _lastWindowSet = windowed;

        //Set volume sliders
        float masterValue = PlayerPrefs.GetFloat(AudioManager.MASTER_VOLUME_PREF_KEY, 1);
        _masterVolume.value = masterValue;
        SetMasterVolume(masterValue);

        float musicValue = PlayerPrefs.GetFloat(AudioManager.MUSIC_VOLUME_PREF_KEY, 1);
        _musicVolume.value = musicValue;
        SetMusicVolume(musicValue);

        float SFXValue = PlayerPrefs.GetFloat(AudioManager.SFX_VOLUME_PREF_KEY, 1);
        _SFXVolume.value = SFXValue;
        SetSFXVolume(SFXValue);
    }

    void Update()
    {
        // Update the checkbox if the player uses the alt+enter shortcut
        if(!Screen.fullScreen != _lastWindowSet)
        {
            _lastWindowSet = !_lastWindowSet;
            int prefNumber = (_lastWindowSet ? 1 : 0);
            PlayerPrefs.SetInt(WINDOWED_PREF_KEY, prefNumber);
            _windowedToggle.SetIsOnWithoutNotify(_lastWindowSet);
        }
    }
    
    /// <summary>
    /// Opens the OptionsMenu.
    /// </summary>
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        SetSelectedGameObject(_firstOption);
    }

    /// <summary>
    /// Goes back to the menu that opened this OptionsMenu..
    /// </summary>
    public void GoBack()
    {
        gameObject.SetActive(false);
        _optionsButton.transform.parent.parent.gameObject.SetActive(true);
        _optionsButton.transform.parent.gameObject.SetActive(true);
        SetSelectedGameObject(_optionsButton);
    }

    /// <summary>
    /// Sets the resolution of the game.
    /// </summary>
    /// <param name="resolutionIndex">The index of the resolution value.</param>
    public void SetResolution(int resolutionIndex)
    {
        // save the setting
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, resolutionIndex);
        Screen.SetResolution(_resolutions[resolutionIndex].width, _resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    /// <summary>
    /// Set whether the game is windowed or not.
    /// </summary>
    /// <param name="isWindowed">Whether or not the game is windowed.</param>
    public void SetWindowed(bool isWindowed)
    {
        Screen.fullScreen = !isWindowed;
        _lastWindowSet = isWindowed;
        // can't store bools in playerprefs directly and can't typecast
        int prefNumber = (isWindowed ? 1 : 0);
        // save the setting
        PlayerPrefs.SetInt(WINDOWED_PREF_KEY, prefNumber);
    }

    /// <summary>
    /// Sets the master volume mixer value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="value"/> parameter is transformed logarithmically by base 10 then multipled by the audio multiplier value before being applied to the mixer.
    /// </remarks>
    /// <seealso cref="AudioManager.AUDIO_MULTIPLIER"/>
    /// <param name="value">The value from the slider between 0.0001 and 1.</param>
    public void SetMasterVolume(float value)
    {
        // store the value before transformation
        SetVolumeText(_masterVolume.GetComponentInChildren<TextMeshProUGUI>(), value);
        PlayerPrefs.SetFloat(AudioManager.MASTER_VOLUME_PREF_KEY, value);
        // transform logarithmically to make the transition smoother
        value = Mathf.Log10(value) * AudioManager.AUDIO_MULTIPLIER;
        _audioMixer.SetFloat("MasterVolume", value);
    }

    /// <summary>
    /// Sets the music volume mixer value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="value"/> parameter is transformed logarithmically by base 10 then multipled by the audio multiplier value before being applied to the mixer.
    /// </remarks>
    /// <seealso cref="AudioManager.AUDIO_MULTIPLIER"/>
    /// <param name="value">The value from the slider between 0.0001 and 1.</param>
    public void SetMusicVolume(float value)
    {
        SetVolumeText(_musicVolume.GetComponentInChildren<TextMeshProUGUI>(), value);
        PlayerPrefs.SetFloat(AudioManager.MUSIC_VOLUME_PREF_KEY, value);
        // transform logarithmically to make the transition smoother
        value = Mathf.Log10(value) * AudioManager.AUDIO_MULTIPLIER;
        _audioMixer.SetFloat("MusicVolume", value);
    }

    /// <summary>
    /// Sets the sound effects volume mixer value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="value"/> parameter is transformed logarithmically by base 10 then multipled by the audio multiplier value before being applied to the mixer.
    /// </remarks>
    /// <seealso cref="AudioManager.AUDIO_MULTIPLIER"/>
    /// <param name="value">The value from the slider between 0.0001 and 1.</param>
    public void SetSFXVolume(float value)
    {
        SetVolumeText(_SFXVolume.GetComponentInChildren<TextMeshProUGUI>(), value);
        PlayerPrefs.SetFloat(AudioManager.SFX_VOLUME_PREF_KEY, value);
        // transform logarithmically to make the transition smoother
        value = Mathf.Log10(value) * AudioManager.AUDIO_MULTIPLIER;
        _audioMixer.SetFloat("SFXVolume", value);
    }

    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }

    void SetVolumeText(TextMeshProUGUI text, float value)
    {
        int roundedNumber = (int)Mathf.Round(value * 100);
        text.text = roundedNumber.ToString();
    }
}
