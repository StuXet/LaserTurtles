using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveGainAccess : ObjectiveBase
{
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _doorLockIcon;
    [SerializeField] private TextMeshPro _objectivesCounterText;
    [SerializeField] private List<ObjectiveBase> keyObjectives = new List<ObjectiveBase>();

    private int _keyObjectivesCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        _door.SetActive(true);

        if (keyObjectives.Count == 0)
        {
            _doorLockIcon.SetActive(false);
            _objectivesCounterText.gameObject.SetActive(false);
        }
        else
        {
            _doorLockIcon.SetActive(true);
            _objectivesCounterText.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
        CheckKeyStatus();
        RefreshText();
    }


    private void OpenDoor()
    {
        _door.SetActive(false);
        _objectivesCounterText.gameObject.SetActive(false);
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

    private void RefreshText()
    {
        if (CompletedObjective)
        {
            _doorLockIcon.SetActive(false);
        }
        _objectivesCounterText.text = _keyObjectivesCounter.ToString() + "/" + keyObjectives.Count.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CompletedObjective)
        {
            BeginObjective();
        }
        else
        {
            OpenDoor();
        }
    }
}
