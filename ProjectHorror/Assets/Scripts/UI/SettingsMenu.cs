using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] Slider musicVolume = null, soundVolume = null, mouseSensitivity = null;

    SettingsManager sm = null;

    public TMPro.TMP_Dropdown resolutionDropdown;
    public TMPro.TMP_Dropdown qualityDropdown;

    [SerializeField] Toggle fullscreenToggle = null;

    void Start()
    {
        sm = SettingsManager.instance;

        musicVolume.value = sm.musicVolume;
        soundVolume.value = sm.soundVolume;
        mouseSensitivity.value = sm.mouseSensitivity;
        fullscreenToggle.isOn = sm.fullScreen;

        musicVolume.onValueChanged.AddListener(OnMusicVolumeChange);
        soundVolume.onValueChanged.AddListener(OnSoundVolumeChange);
        mouseSensitivity.onValueChanged.AddListener(OnMouseSensitivityChange);

        SetupResolutionsDropdown();
        SetupQualityDropdown();
    }

    void SetupQualityDropdown()
    {
        if (sm.currentQualityIndex != -1) qualityDropdown.value = sm.currentQualityIndex;
    }

    void SetupResolutionsDropdown()
    {
        Resolution[] availableResolutions = sm.resolutions;
        Debug.Log(availableResolutions.Length);
        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            string option = availableResolutions[i].width + " x " + availableResolutions[i].height;
            resolutionOptions.Add(option);

            if (availableResolutions[i].width == Screen.width && availableResolutions[i].height == Screen.height)
            {
                Debug.Log("!!! FOUND RESOLUTION !!!" + availableResolutions[i].width + " / " + availableResolutions[i].height);
                currentResolutionIndex = i;
            }
        }
        Debug.Log("!!! currentResolutionIndex:  " + currentResolutionIndex);

        resolutionDropdown.AddOptions(resolutionOptions);
        if (sm.currentResolutionIndex != -1) currentResolutionIndex = sm.currentResolutionIndex;
        else sm.currentResolutionIndex = currentResolutionIndex;
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        Debug.Log("!!! resolutionDropdown.value:  " + resolutionDropdown.value);
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape)) gameObject.SetActive(false);
    }

    public void SetResolution(int resolutionIndex)
    {
        sm.SetResolution(resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        sm.SetFullscreen(isFullscreen);
    }

    public void SetQuality(int qualityIndex)
    {
        sm.SetQuality(qualityIndex);
    }

    public void OnMusicVolumeChange(float value)
    {
        sm.ChangeMusicVolume(value);
    }

    public void OnSoundVolumeChange(float value)
    {
        sm.ChangeSoundVolume(value);
    }

    public void OnMouseSensitivityChange(float value)
    {
        sm.ChangeMouseSensitivity(value);
    }
}
