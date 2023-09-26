using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectivesHandler : MonoBehaviour
{
    private TextMeshProUGUI _textUI;
    private Animator _objUIAnimator;

    [SerializeField] private GameObject _compassTracker, _objectiveIndicator;
    [SerializeField] private bool _useCompass = true;
    private float _objectiveIndicatorStartDist;
    private string _currentObjectiveText;

    private ObjectivesContainer _objectivesContainer;
    private List<ObjectiveBase> _objectivesList = new List<ObjectiveBase>();
    private ObjectiveBase _currentObjective;
    private int _currentObjectiveIndex;

    private bool _showUI;
    [SerializeField] private float _popupDuration = 2.5f;
    private float _popupTimer;


    private void Awake()
    {
        _objectivesContainer = FindObjectOfType<ObjectivesContainer>();
        //_objectivesList = _objectivesContainer.ObjectivesList;
    }

    // Start is called before the first frame update
    void Start()
    {
        _textUI = UIMediator.Instance.ObjectiveUI.GetComponentInChildren<TextMeshProUGUI>();
        _objUIAnimator = UIMediator.Instance.ObjectiveUI.GetComponent<Animator>();
        if (_objectiveIndicator) _objectiveIndicatorStartDist = _objectiveIndicator.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus();
        RenewText();
        ObjectivePopup();
        Compass();
    }

    private void ObjectivePopup()
    {
        if (_showUI)
        {
            _popupTimer += Time.deltaTime;
            if (_popupTimer <= _popupDuration)
            {
                _objUIAnimator.SetBool("Show", true);
            }
            else
            {
                _objUIAnimator.SetBool("Show", false);
                _showUI = false;
            }
        }
        else
        {
            _popupTimer = 0;
        }
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
            _textUI.text = "Objective: Well Done!";
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

                    // For UI Popup
                    _showUI = true;
                    _popupTimer = 0;
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

                // For UI Popup
                _showUI = true;
                _popupTimer = 0;
            }
        }
    }
}
