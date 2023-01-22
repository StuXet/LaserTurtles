using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectivesHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textUI;
    private string _currentObjectiveText;

    private ObjectivesContainer _objectivesContainer;
    private List<ObjectiveBase> _objectivesList = new List<ObjectiveBase>();
    private ObjectiveBase _currentObjective;
    private int _currentObjectiveIndex;


    private void Awake()
    {
        _objectivesContainer = FindObjectOfType<ObjectivesContainer>();
        //_objectivesList = _objectivesContainer.ObjectivesList;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus();
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
            _textUI.text = "Objective: " + _currentObjectiveText;
        }
        else
        {
            _textUI.text = "Objective: Explore!";
        }
    }

    private void CheckStatus()
    {
        if (_objectivesList.Count > 0)
        {
            if (_currentObjectiveIndex == 0)
            {
                _currentObjective = null;
            }
            else
            {
                _currentObjective = _objectivesList[_currentObjectiveIndex - 1];
            }


            if (_currentObjective != null)
            {
                if (_currentObjective.CompletedObjective)
                {
                    _currentObjectiveIndex--;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObjectiveBase objective))
        {
            if (!objective.CompletedObjective && !_objectivesList.Contains(objective))
            {
                _objectivesList.Add(objective);
                _currentObjectiveIndex = _objectivesList.Count;
            }
        }
    }
}
