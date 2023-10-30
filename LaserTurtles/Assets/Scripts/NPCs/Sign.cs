using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sign : MonoBehaviour
{
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private GameObject pressEIcon;
    [SerializeField] private GameObject popUpWindow;
    [SerializeField] private Animator popupAnim;
    [SerializeField] private AudioSource _popUpSFX, _popOutSFX;
    [SerializeField] private TMP_Text textObject;
    [TextArea(5, 20)]
    [SerializeField] private string textContent;

    private bool _used;
    private bool _interactable;
    private InputManager _inputManager;

    private void Start()
    {
        textObject.text = textContent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_inputManager == null)
            {
                _inputManager = other.GetComponent<InputManager>();
                _inputManager.PlInputActions.Player.Interact.performed += Interact;
            }
            exclamationMark.SetActive(false);
            pressEIcon.SetActive(true);
            _interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_used)
        {
            exclamationMark.SetActive(true);
        }
        pressEIcon.SetActive(false);
        if (popUpWindow.activeInHierarchy)
        {
            _popUpSFX.Stop();
            _popOutSFX.Play();
        }
        popUpWindow.SetActive(false);
        _interactable = false;
        popupAnim.SetBool("Popup", false);
    }

    private void CloseWindow()
    {
        //popUpWindow.SetActive(false);
        popupAnim.SetBool("Popup", false);
        pressEIcon.SetActive(true);
        _interactable = true;
        _popUpSFX.Stop();
        _popOutSFX.Play();
    }

    private void Interact(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_interactable)
        {
            if (!popUpWindow.activeSelf)
            {
                popupAnim.SetBool("Popup", true);
                pressEIcon.SetActive(false);
                //popUpWindow.SetActive(true);
                _used = true;
                _popOutSFX.Stop();
                _popUpSFX.Play();
            }
            else
            {
                CloseWindow();
            }
        }
    }
}
