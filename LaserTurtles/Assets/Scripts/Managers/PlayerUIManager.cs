using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject LargeMapOverlay;
    private bool _isLargeMapOverlayOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        CloseAll();
    }

    // Update is called once per frame
    void Update()
    {
        ListenForInput();
        UIStatesRunner();
    }

    #region Main Methods
    private void ListenForInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleLargeMapOverlay();
        }
    }

    private void UIStatesRunner()
    {
        LargeMapState();
    }

    private void CloseAll()
    {
        _isLargeMapOverlayOpen = false;
    }
    #endregion


    #region Large Map Overlay
    public void ToggleLargeMapOverlay()
    {
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
        }
        else
        {
            LargeMapOverlay.SetActive(false);
        }
    }
    #endregion
}
