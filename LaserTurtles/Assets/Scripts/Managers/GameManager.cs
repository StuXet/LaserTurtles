using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private EventSystem _eventSystem;
    private PlayerInputActions _plInputActions;
    private UIMediator _uIMediator;
    private GameObject _winTextRef;

    public event EventHandler OnPauseToggle;
    private bool _isGamePaused;

    public PlayerInputActions PlInputActions { get => _plInputActions; }
    public bool IsGamePaused { get => _isGamePaused; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _eventSystem = FindObjectOfType<EventSystem>();
        _plInputActions = FindObjectOfType<InputManager>().PlInputActions;
        _uIMediator = UIMediator.Instance;
        _winTextRef = _uIMediator.WinUI;
    }

    private void Start()
    {
        if (_plInputActions == null) _plInputActions = FindObjectOfType<InputManager>().PlInputActions;
    }

    public void RefreshSelectedUI(GameObject selectedObj)
    {
        _eventSystem.SetSelectedGameObject(selectedObj);
    }

    public void YouWin()
    {
        _winTextRef.SetActive(true);
        StartCoroutine(WinningSequence());
    }

    IEnumerator WinningSequence()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }

    public void SetGamePauseBool(bool state)
    {
        _isGamePaused = state;
        if (OnPauseToggle != null) OnPauseToggle(this, EventArgs.Empty);
    }
}
