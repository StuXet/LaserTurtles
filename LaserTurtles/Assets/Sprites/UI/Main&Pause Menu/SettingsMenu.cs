using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown graphicsDropdown;
    public Slider volumeSlider;
    [SerializeField] private AudioMixer _audioMixer;
    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
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
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = fullscreenToggle.isOn;

        graphicsDropdown.ClearOptions();
        List<string> graphicsOptions = new List<string>() { "Low", "Medium", "High" };
        graphicsDropdown.AddOptions(graphicsOptions);
        graphicsDropdown.value = PlayerPrefs.GetInt("Graphics", QualitySettings.GetQualityLevel());
        graphicsDropdown.RefreshShownValue();

        //volumeSlider.value = PlayerPrefs.GetFloat("Volume", AudioListener.volume);
        float tempVol;
        bool tempVolBool = _audioMixer.GetFloat("Volume", out tempVol);
        if (tempVolBool)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", tempVol);
            _audioMixer.SetFloat("Volume", volumeSlider.value);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetGraphics(int graphicsIndex)
    {
        QualitySettings.SetQualityLevel(graphicsIndex);
        PlayerPrefs.SetInt("Graphics", graphicsIndex);
    }

    public void SetVolume(float volume)
    {
        _audioMixer.SetFloat("Volume", volume);
        //AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
}