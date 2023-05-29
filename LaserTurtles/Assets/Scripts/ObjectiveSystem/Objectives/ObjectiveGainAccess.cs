using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveGainAccess : ObjectiveBase
{
    [SerializeField] private bool _useFade = true;
    [SerializeField] private float _fadeSpeed = 1.0f;
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _doorLockIcon;
    [SerializeField] private TextMeshPro _objectivesCounterText;
    [SerializeField] private List<ObjectiveBase> keyObjectives = new List<ObjectiveBase>();

    private bool _openDoor;
    private bool _doorOpened;
    private Material _doorMat;
    private Color _doorColor;
    private Color _tempColor;
    private int _keyObjectivesCounter = 0;

    public List<ObjectiveBase> KeyObjectives { get => keyObjectives;}

    private void Awake()
    {
        if (_door)
        {
            _doorMat = _door.GetComponent<MeshRenderer>().material;
            _doorColor = _doorMat.color;
            _tempColor = _doorColor;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _door.SetActive(true);

        if (keyObjectives.Count == 0)
        {
            if (_doorLockIcon != null) _doorLockIcon.SetActive(false);
            if (_objectivesCounterText != null) _objectivesCounterText.gameObject.SetActive(false);
        }
        else
        {
            if (_doorLockIcon != null) _doorLockIcon.SetActive(true);
            if (_objectivesCounterText != null) _objectivesCounterText.gameObject.SetActive(true);
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
        if (_objectivesCounterText != null) _objectivesCounterText.gameObject.SetActive(false);
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
            //if (_openDoor)
            //{
            //    float fadeVal = _doorMat.GetFloat("_Fade_Depth");

            //    fadeVal = Mathf.Lerp(fadeVal, 180, 2 * Time.deltaTime);

            //    _doorMat.SetFloat("_Fade_Depth", fadeVal);
            //}

            //if (_doorMat.GetFloat("_Fade_Depth") >= 150)
            //{
            //    _door.SetActive(false);
            //    _doorOpened = true;
            //}

            if (_openDoor)
            {
                _tempColor.a = Mathf.Lerp(_tempColor.a,0, _fadeSpeed * Time.deltaTime);
                _doorMat.color = _tempColor;
            }

            if (_doorMat.color.a <= 0.1f)
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
            if (_doorLockIcon != null) _doorLockIcon.SetActive(false);
        }
        if (_objectivesCounterText != null) _objectivesCounterText.text = _keyObjectivesCounter.ToString() + "/" + keyObjectives.Count.ToString();
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
