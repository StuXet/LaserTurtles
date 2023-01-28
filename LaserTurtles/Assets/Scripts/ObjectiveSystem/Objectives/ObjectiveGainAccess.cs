using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveGainAccess : ObjectiveBase
{
    [SerializeField] private bool _useFade = true;
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _doorLockIcon;
    [SerializeField] private TextMeshPro _objectivesCounterText;
    [SerializeField] private List<ObjectiveBase> keyObjectives = new List<ObjectiveBase>();

    private bool _openDoor;
    private bool _doorOpened;
    private Material _doorMat;
    private int _keyObjectivesCounter = 0;

    private void Awake()
    {
        if (_door)
        {
            _doorMat = _door.GetComponent<MeshRenderer>().material;
        }
    }

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
        FadeDoor();
    }


    private void OpenDoor()
    {
        _openDoor = true;
        _objectivesCounterText.gameObject.SetActive(false);
        if (!_useFade)
        {
            _door.SetActive(false);
            _doorOpened = true;
        }
    }

    private void FadeDoor()
    {
        if (!_doorOpened && _useFade)
        {
            if (_openDoor)
            {
                float fadeVal = _doorMat.GetFloat("_Fade_Depth");

                fadeVal = Mathf.Lerp(fadeVal, 180, 2 * Time.deltaTime);

                _doorMat.SetFloat("_Fade_Depth", fadeVal);
            }

            if (_doorMat.GetFloat("_Fade_Depth") >= 150)
            {
                _door.SetActive(false);
                _doorOpened = true;
            }
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
