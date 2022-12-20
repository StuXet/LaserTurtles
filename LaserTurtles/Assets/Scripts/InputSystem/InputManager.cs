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

    private void OnEnable() => _plInputActions.Player.Enable();

    private void OnDisable() => _plInputActions.Player.Disable();
}
