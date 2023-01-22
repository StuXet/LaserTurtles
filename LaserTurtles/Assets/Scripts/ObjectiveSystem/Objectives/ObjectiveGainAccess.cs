using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGainAccess : ObjectiveBase
{
    [SerializeField] private GameObject _door;

    [SerializeField] private List<ObjectiveBase> keyObjectives = new List<ObjectiveBase>();

    // Start is called before the first frame update
    void Start()
    {
        _door.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
        CheckKeyStatus();
        OpenDoor();
    }


    private void OpenDoor()
    {
        if (CompletedObjective)
        {
            _door.SetActive(false);
        }
    }

    private void CheckKeyStatus()
    {
        if (!CompletedObjective)
        {
            bool allDone = true;
            for (int i = 0; i < keyObjectives.Count; i++)
            {
                if (!keyObjectives[i].CompletedObjective)
                {
                    allDone = false;
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
