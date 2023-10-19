using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveBase : MonoBehaviour
{
    // For Objective

    [SerializeField] private string _objectiveName;

    private bool _beganObjective;
    private bool _requirementMet;
    private bool _completedObjective;

    public bool BeganObjective { get => _beganObjective; }
    public bool CompletedObjective { get => _completedObjective; }
    public bool RequirementMet { get => _requirementMet; }
    public string ObjectiveName { get => _objectiveName; }

    // For Camera Zoom Effect
    [SerializeField] private float _camDistanceChange;
    private CameraFollow _cameraFollowRef;


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
        if (_beganObjective && _requirementMet && !_completedObjective)
        {
            _completedObjective = true;

            // Cam Effect
            if (_cameraFollowRef) _cameraFollowRef.ResetCamDistance();
        }
    }

    public void CamZoomOnEnter(Collider other)
    {
        if (_camDistanceChange != 0 && other.CompareTag("Player"))
        {
            if (_cameraFollowRef == null) _cameraFollowRef = other.GetComponent<PlayerController>().PlayerCam.GetComponent<CameraFollow>();
            _cameraFollowRef.ChangeCamDistance(_camDistanceChange);
        }
    }

    public void CamZoomOnExit(Collider other)
    {
        if (other.CompareTag("Player") && _cameraFollowRef != null)
        {
            _cameraFollowRef.ResetCamDistance();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CompletedObjective)
        {
            BeginObjective();
        }

        CamZoomOnEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        CamZoomOnExit(other);
    }
}
