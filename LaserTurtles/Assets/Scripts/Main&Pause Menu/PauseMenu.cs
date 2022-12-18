using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu.activeSelf)
            {
                // Close the settings menu and show the pause menu
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
        // Add code here to restart the game
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

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}