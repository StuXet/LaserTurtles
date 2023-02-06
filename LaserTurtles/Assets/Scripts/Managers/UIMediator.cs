using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMediator : MonoBehaviour
{
    [SerializeField] private GameObject _dialougeUI, _winUI;

    public GameObject DialougeUI { get => _dialougeUI;}
    public GameObject WinUI { get => _winUI; }


    private void Awake()
    {
        _dialougeUI.SetActive(false);
        _winUI.SetActive(false);
    }
}
