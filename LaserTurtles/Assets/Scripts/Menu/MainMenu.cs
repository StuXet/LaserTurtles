using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameScene = "SampleScene";

    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (creditsPanel.activeSelf || settingsPanel.activeSelf)
            {
                creditsPanel.SetActive(false);
                settingsPanel.SetActive(false);

                mainMenuPanel.SetActive(true);
            }
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene(gameScene);
    }
    
    public void Continue()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void Settings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void Credits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        mainMenuPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}