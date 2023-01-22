using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectivesHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textUI;
    private string _currentObjectiveText;

    private List<ObjectiveBase> _objectivesList;
    private ObjectiveBase _currentObjective;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RenewText();
    }

    private void RenewText()
    {
        if (_currentObjective != null)
        {
            _currentObjectiveText = _currentObjective.ObjectiveName;
        }

        if (_currentObjective != null)
        {
            _textUI.text = _currentObjectiveText;
        }
        else
        {
            _textUI.text = "Objective: Explore!";
        }
    }

    private bool CheckCurrentObjectiveCompletion()
    {
        if (_currentObjective.CompletedObjective)
        {
            _currentObjective = null;
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObjectiveBase objective))
        {
            if (!_objectivesList.Contains(objective))
            {
                _objectivesList.Add(objective);
                if (_objectivesList.Count == 1)
                {
                    _currentObjective = _objectivesList[0];
                    _currentObjective.BeginObjective();
                }
            }
        }
    }
}
