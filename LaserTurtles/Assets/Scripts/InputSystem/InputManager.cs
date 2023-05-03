using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions _plInputActions;
    public PlayerInputActions PlInputActions { get => _plInputActions; }

    private void Awake()
    {
        _plInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        GameManager.Instance.OnPauseToggle += Instance_OnPauseToggle;
    }

    private void OnEnable() 
    {
        _plInputActions.Player.Enable();
        _plInputActions.UI.Enable();
    }

    private void OnDisable() => _plInputActions.Player.Disable();


    private void Instance_OnPauseToggle(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGamePaused)
        {
            _plInputActions.Player.Disable();
        }
        else
        {
            _plInputActions.Player.Enable();
        }
    }

}
