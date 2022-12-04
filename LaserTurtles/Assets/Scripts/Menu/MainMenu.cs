using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void NewGame()
    {
        SceneManager.LoadScene("Game");
    }

    void Continue()
    {
        SceneManager.LoadScene("Game");
    }

    void Settings()
    {
        Debug.Log("Settings");
    }

    void Credits()
    {
        Debug.Log("Credits");
    }

    void Quit()
    {
        Application.Quit();
    }
}
