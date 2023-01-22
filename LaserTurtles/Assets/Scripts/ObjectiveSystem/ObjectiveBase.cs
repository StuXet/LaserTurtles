using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveBase : MonoBehaviour
{
    private bool _beganObjective;
    private bool _requirementMet;
    private bool _completedObjective;

    [SerializeField] private string _objectiveName;

    public bool CompletedObjective { get => _completedObjective;}
    public string ObjectiveName { get => _objectiveName;}


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginObjective()
    {
        _beganObjective = true;
    }

    private void FinishObjective()
    {
        if (_beganObjective && _requirementMet)
        {
            _completedObjective = true;
        }
    }
}
