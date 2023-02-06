using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Range(0, 2)] public float TimeScale = 1;

    private UIMediator _uIMediator;
    private GameObject _winTextRef;

    private void Awake()
    {
        _uIMediator = FindObjectOfType<UIMediator>();
        _winTextRef = _uIMediator.WinUI;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TimeScale != Time.timeScale) Time.timeScale = TimeScale;
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
}
