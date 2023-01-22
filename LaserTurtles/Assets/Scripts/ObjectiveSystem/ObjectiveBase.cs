using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveBase : MonoBehaviour
{
    [SerializeField] private string _objectiveName;

    private bool _beganObjective;
    private bool _requirementMet;
    private bool _completedObjective;

    public bool BeganObjective { get => _beganObjective;}
    public bool CompletedObjective { get => _completedObjective;}
    public bool RequirementMet { get => _requirementMet;}
    public string ObjectiveName { get => _objectiveName;}


    public void BeginObjective()
    {
        _beganObjective = true;
    }

    public void ObjectiveRequirementMet()
    {
        if (_beganObjective)
        {
            _requirementMet = true;
        }
    }

    public void FinishedObjective()
    {
        if (_beganObjective && _requirementMet)
        {
            _completedObjective = true;
        }
    }
}
