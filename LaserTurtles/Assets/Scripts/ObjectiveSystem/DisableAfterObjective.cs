using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DisableAfterObjective : MonoBehaviour
{
    [SerializeField] private GameObject _objToDisable;
    [SerializeField] private ObjectiveBase _keyObjective;
    private bool _completed;


    // Update is called once per frame
    void Update()
    {
        CheckCompletionState();
    }

    private void CheckCompletionState()
    {
        if (_keyObjective.CompletedObjective && !_completed)
        {
            _completed = true;
            _objToDisable.SetActive(false);
        }
    }
}
