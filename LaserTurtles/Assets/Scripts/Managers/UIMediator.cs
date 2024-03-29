using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMediator : MonoBehaviour
{
    public static UIMediator Instance;

    [SerializeField] private GameObject _dialougeUI, _winUI, _bossHPUI, _objectiveUI;

    public GameObject DialougeUI { get => _dialougeUI; }
    public GameObject WinUI { get => _winUI; }
    public GameObject BossHPUI { get => _bossHPUI; }
    public GameObject ObjectiveUI { get => _objectiveUI; }


    private void Awake()
    {
        gameObject.SetActive(true);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _winUI.SetActive(false);
        //_bossHPUI.SetActive(false);
    }
}
