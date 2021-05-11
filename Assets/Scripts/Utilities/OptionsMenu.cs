using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsMenu : MonoBehaviour
{

    #region Player pref key constants
    private const string RESOLUTION_PREF_KEY = "resolution";
    private const string WINDOWED_PREF_KEY = "windowed";
    private const string MASTER_VOLUME_PREF_KEY = "masterVolume";
    private const string MUSIC_VOLUME_PREF_KEY = "musicVolume";
    private const string SFX_VOLUME_PREF_KEY = "SFXVolume";
    #endregion

    #region Inspector fields
    [SerializeField] private GameObject optionsButton;
    [SerializeField] private GameObject firstOption;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle windowedToggle;
    [SerializeField] private Slider masterVolume;
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider SFXVolume;
    [SerializeField] private AudioMixer audioMixer;
    #endregion

    #region Private fields
    private Resolution[] resolutions;
    #endregion

    void Awake()
    {
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height}).Distinct().ToArray();
        List<string> resolutionsText = new List<string>(resolutions.Length);
        int currentIndex = -1;

        currentIndex = PlayerPrefs.GetInt(RESOLUTION_PREF_KEY, -1);

        for(int i = 0; i < resolutions.Length; i++)
        {

            resolutionsText.Add(resolutions[i].width + " x " + resolutions[i].height);

            // By default set the resolution to the current screen resolution
            if(resolutions[i].width == Screen.currentResolution.width
                && resolutions[i].height == Screen.currentResolution.height && currentIndex == -1)
            {
                currentIndex = i;
            }
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionsText);

        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        //Set windowed status
        bool windowed = PlayerPrefs.GetInt(WINDOWED_PREF_KEY, 0) == 1;
        windowedToggle.isOn = windowed;
        Screen.fullScreen = !windowed;

        //Set volume sliders
        float masterValue = PlayerPrefs.GetFloat(MASTER_VOLUME_PREF_KEY, 0);
        masterVolume.value = masterValue;
        SetMasterVolume(masterValue);

        float musicValue = PlayerPrefs.GetFloat(MUSIC_VOLUME_PREF_KEY, 0);
        musicVolume.value = musicValue;
        SetMusicVolume(musicValue);

        float SFXValue = PlayerPrefs.GetFloat(SFX_VOLUME_PREF_KEY, 0);
        SFXVolume.value = SFXValue;
        SetSFXVolume(SFXValue);

    }
    
    public void OpenMenu()
    {
        gameObject.SetActive(true);
        SetSelectedGameObject(firstOption);
    }

    public void GoBack()
    {
        gameObject.SetActive(false);
        optionsButton.transform.parent.gameObject.SetActive(true);
        SetSelectedGameObject(optionsButton);
    }

    public void SetResolution(int resolutionIndex)
    {
        PlayerPrefs.SetInt(RESOLUTION_PREF_KEY, resolutionIndex);
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);
    }

    public void SetWindowed(bool isWindowed)
    {
        int prefNumber = (isWindowed ? 1 : 0);
        PlayerPrefs.SetInt(WINDOWED_PREF_KEY, prefNumber);
        Screen.fullScreen = !isWindowed;
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_PREF_KEY, value);
        audioMixer.SetFloat("MasterVolume", value);
        SetVolumeText(masterVolume.GetComponentInChildren<TextMeshProUGUI>(), value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_PREF_KEY, value);
        audioMixer.SetFloat("MusicVolume", value);
        SetVolumeText(musicVolume.GetComponentInChildren<TextMeshProUGUI>(), value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_PREF_KEY, value);
        audioMixer.SetFloat("SFXVolume", value);
        SetVolumeText(SFXVolume.GetComponentInChildren<TextMeshProUGUI>(), value);
    }

    void SetSelectedGameObject(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(go);
    }

    void SetVolumeText(TextMeshProUGUI text, float value)
    {
        float number = (value + 80) / 80;
        int roundedNumber = (int)Mathf.Round(number * 100);
        text.text = roundedNumber.ToString();
    }
}
