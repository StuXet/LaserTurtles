using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private InputManager _inputManagerRef;
    private PlayerInputActions _plInputActions;

    [SerializeField] private GameObject LargeMapOverlay;
    private bool _isLargeMapOverlayOpen = false;

    [SerializeField] private GameObject InventoryOverlay;
    private bool _isInventoryOverlayOpen = false;

    private GameObject _objectiveUIRef;

    // Start is called before the first frame update
    void Start()
    {
        _objectiveUIRef = UIMediator.Instance.ObjectiveUI;
        _plInputActions = _inputManagerRef.PlInputActions;
        _plInputActions.Player.Map.performed += MapToggle;
        _plInputActions.Player.Inventory.performed += InventoryToggle;
        CloseAll();
    }


    private void MapToggle(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ToggleLargeMapOverlay();
    }

    private void InventoryToggle(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        ToggleInventoryOverlay();
    }


    // Update is called once per frame
    void Update()
    {
        UIStatesRunner();
    }


    #region Main Methods
    private void UIStatesRunner()
    {
        LargeMapState();
        InventoryOverlayState();
    }

    private void CloseAll()
    {
        _isLargeMapOverlayOpen = false;
        _isInventoryOverlayOpen = false;
    }

    private bool CloseAllExcept(bool exceptBool)
    {
        CloseAll();
        return exceptBool;
    }
    #endregion


    #region Large Map Overlay
    public void ToggleLargeMapOverlay()
    {
        _isLargeMapOverlayOpen = CloseAllExcept(_isLargeMapOverlayOpen);

        if (_isLargeMapOverlayOpen)
        {
            _isLargeMapOverlayOpen = false;
        }
        else
        {
            _isLargeMapOverlayOpen = true;
        }
    }

    private void LargeMapState()
    {
        if (_isLargeMapOverlayOpen)
        {
            LargeMapOverlay.SetActive(true);
            _objectiveUIRef.SetActive(true);
        }
        else
        {
            LargeMapOverlay.SetActive(false);
            _objectiveUIRef.SetActive(false);
        }
    }
    #endregion


    #region Inventory Overlay
    private void ToggleInventoryOverlay()
    {
        _isInventoryOverlayOpen = CloseAllExcept(_isInventoryOverlayOpen);

        if (_isInventoryOverlayOpen)
        {
            _isInventoryOverlayOpen = false;
        }
        else
        {
            _isInventoryOverlayOpen = true;
        }
    }

    private void InventoryOverlayState()
    {
        if (_isInventoryOverlayOpen)
        {
            InventoryOverlay.SetActive(true);
        }
        else
        {
            InventoryOverlay.SetActive(false);
        }
    }
    #endregion
}
