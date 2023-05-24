using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Selection")]
    public EventSystem _eventSystem;
    public GameObject _mainMenuSelectedObj, _settingsSelectedObj, _creditsSelectedObj;

    private InputAction escapeAction;
    private int lastSceneIndex;

    private void Start()
    {
        lastSceneIndex = PlayerPrefs.GetInt("LastSceneIndex", 0);
        _eventSystem.firstSelectedGameObject = _mainMenuSelectedObj;
    }

    private void Awake()
    {
        escapeAction = new InputAction("Escape", binding: "<Keyboard>/escape");
        escapeAction.performed += ctx => OnEscape();
    }

    private void OnEnable()
    {
        escapeAction.Enable();
    }

    private void OnDisable()
    {
        escapeAction.Disable();
    }

    public void RefreshSelected(GameObject selectedObj)
    {
        _eventSystem.SetSelectedGameObject(selectedObj);
    }

    private void OnEscape()
    {
        if (creditsPanel.activeSelf || settingsPanel.activeSelf)
        {
            creditsPanel.SetActive(false);
            settingsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
            RefreshSelected(_mainMenuSelectedObj);
        }
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Continue()
    {
        SceneManager.LoadScene(lastSceneIndex);
    }

    public void Settings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        RefreshSelected(_settingsSelectedObj);
    }

    public void Credits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
        RefreshSelected(_creditsSelectedObj);
    }

    public void ClosePanel()
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        mainMenuPanel.SetActive(true);
        RefreshSelected(_mainMenuSelectedObj);
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("LastSceneIndex", SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
        Debug.Log("Quit");
    }
}