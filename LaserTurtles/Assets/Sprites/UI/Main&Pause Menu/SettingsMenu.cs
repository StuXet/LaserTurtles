using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    PlayerController playerController;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Toggle cursorLookToggle;
    public TMP_Dropdown graphicsDropdown;
    public Slider masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider, ambientVolumeSlider;
    [SerializeField] private AudioMixer _audioMixer;
    Resolution[] resolutions;

    private void Awake()
    {
        if (playerController == null && SceneManager.GetActiveScene().buildIndex != 0)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    private void Start()
    {
        cursorLookToggle.isOn = PlayerPrefs.GetInt("CursorLook", 1) == 1;

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
        bool tempVolBool = _audioMixer.GetFloat("Master", out tempVol);
        if (tempVolBool)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("Master", tempVol);
            _audioMixer.SetFloat("Master", masterVolumeSlider.value);   
            musicVolumeSlider.value = PlayerPrefs.GetFloat("Music", tempVol);
            _audioMixer.SetFloat("Music", musicVolumeSlider.value);
            ambientVolumeSlider.value = PlayerPrefs.GetFloat("Ambient", tempVol);
            _audioMixer.SetFloat("Ambient", ambientVolumeSlider.value);    
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFX", tempVol);
            _audioMixer.SetFloat("SFX", sfxVolumeSlider.value);
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

    public void SetVolumeMaster(float volume)
    {
        _audioMixer.SetFloat("Master", volume);
        //AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Master", volume);
    }

    public void SetVolumeMusic(float volume)
    {
        _audioMixer.SetFloat("Music", volume);
        //AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Music", volume);
    }

    public void SetVolumeSFX(float volume)
    {
        _audioMixer.SetFloat("SFX", volume);
        //AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SFX", volume);
    }

    public void SetVolumeAmbient(float volume)
    {
        _audioMixer.SetFloat("Ambient", volume);
        //AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Ambient", volume);
    }

    public void CursorLook(bool cursor)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }
            if (cursor)
            {
                playerController.MoveType = MovementType.WorldPosTrackLook;
            }
            else
            {
                playerController.MoveType = MovementType.WorldPos;
            }
        }
        PlayerPrefs.SetInt("CursorLook", cursor ? 1 : 0);
    }
}