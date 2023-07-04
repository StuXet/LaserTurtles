using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectivesHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textUI;
    [SerializeField] private GameObject _compassTracker, _objectiveIndicator;
    [SerializeField] private bool _useCompass = true;
    private float _objectiveIndicatorStartDist;
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
        if (_objectiveIndicator) _objectiveIndicatorStartDist = _objectiveIndicator.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus();
        RenewText();
        Compass();
    }

    private void Compass()
    {
        if (_useCompass)
        {
            if (_currentObjective != null)
            {
                _compassTracker.SetActive(true);

                Vector3 objectivePos;
                if (_currentObjective is ObjectiveGainAccess)
                {
                    if ((_currentObjective as ObjectiveGainAccess).KeyObjectives.Count <= 1)
                    {
                        objectivePos = (_currentObjective as ObjectiveGainAccess).KeyObjectives[0].transform.position;
                    }
                    else
                    {
                        objectivePos = _currentObjective.transform.position;
                    }
                }
                else
                {
                    objectivePos = _currentObjective.transform.position;
                }
                objectivePos.y = transform.position.y;
                _compassTracker.transform.LookAt(objectivePos);

                float distFromObjective = Mathf.Abs(Vector3.Distance(_compassTracker.transform.position, objectivePos));

                if (distFromObjective >= _objectiveIndicatorStartDist)
                {
                    _objectiveIndicator.SetActive(true);
                    _objectiveIndicator.transform.localPosition = new Vector3(0, -1, _objectiveIndicatorStartDist);
                    _objectiveIndicator.transform.localRotation = Quaternion.Euler(Vector3.zero);
                }
                else if (distFromObjective < _objectiveIndicatorStartDist && distFromObjective >= _objectiveIndicatorStartDist / 2)
                {
                    _objectiveIndicator.SetActive(true);
                    _objectiveIndicator.transform.localPosition = new Vector3(0, -1, Mathf.Abs(Vector3.Distance(_compassTracker.transform.position, objectivePos)));
                    _objectiveIndicator.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
                }
                else
                {
                    _objectiveIndicator.SetActive(false);
                }
            }
            else
            {
                _compassTracker.SetActive(false);
            }
        }
        else
        {
            _compassTracker.SetActive(false);
        }
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
