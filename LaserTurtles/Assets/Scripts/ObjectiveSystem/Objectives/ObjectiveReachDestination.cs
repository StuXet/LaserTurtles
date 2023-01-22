using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveReachDestination : ObjectiveBase
{
    [SerializeField] private TargetDestination _destination;

    private void Awake()
    {
        _destination.ReachedDest += _destination_ReachedDest;
    }

    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
    }

    private void _destination_ReachedDest(object sender, System.EventArgs e)
    {
        if (BeganObjective)
        {
            ObjectiveRequirementMet();
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
