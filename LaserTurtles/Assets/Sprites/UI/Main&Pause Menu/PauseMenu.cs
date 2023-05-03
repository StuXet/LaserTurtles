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

    [Header("Selection")]
    public GameObject _pauseMenuSelectedObj;
    public GameObject _settingsSelectedObj;

    private void Start()
    {
        SaveGame();
        Time.timeScale = 1;

        GameManager.Instance.PlInputActions.Player.ESC.performed += ESC_performed;
    }

    private void ESC_performed(InputAction.CallbackContext obj)
    {
        PausingToggle();
    }

    private void PausingToggle()
    {
        if (settingsMenu.activeSelf)
        {
            ClosePanel();
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

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        GameManager.Instance.RefreshSelectedUI(_pauseMenuSelectedObj);
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
        GameManager.Instance.RefreshSelectedUI(_settingsSelectedObj);
    }

    public void ClosePanel()
    {
        settingsMenu.SetActive(false);

        pauseMenu.SetActive(true);

        GameManager.Instance.RefreshSelectedUI(_pauseMenuSelectedObj);
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
        Debug.Log("Quit");
    }
}