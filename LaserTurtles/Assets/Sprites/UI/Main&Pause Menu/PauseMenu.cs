using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    private void Start()
    {
        SaveGame();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsMenu.activeSelf)
            {
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
            }
            else if (pauseMenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Settings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void ClosePanel()
    {
        settingsMenu.SetActive(false);

        pauseMenu.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("LastSceneIndex", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}