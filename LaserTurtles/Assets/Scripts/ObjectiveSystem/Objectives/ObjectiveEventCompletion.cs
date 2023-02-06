using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectiveEventCompletion : ObjectiveBase
{
    [SerializeField] private List<ObjectiveBase> keyObjectives = new List<ObjectiveBase>();

    public UnityEvent OnCompletion;

    private int _keyObjectivesCounter = 0;


    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
        CheckKeyStatus();
        CheckCompletionState();
    }

    private void CheckCompletionState()
    {
        if (CompletedObjective)
        {
            OnCompletion.Invoke();
        }
    }

    private void CheckKeyStatus()
    {
        if (!CompletedObjective)
        {
            _keyObjectivesCounter = 0;
            bool allDone = true;
            for (int i = 0; i < keyObjectives.Count; i++)
            {
                if (!keyObjectives[i].CompletedObjective)
                {
                    allDone = false;
                }
                else
                {
                    _keyObjectivesCounter++;
                }
            }

            if (allDone)
            {
                ObjectiveRequirementMet();
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!CompletedObjective)
        {
            BeginObjective();
        }
    }
}
