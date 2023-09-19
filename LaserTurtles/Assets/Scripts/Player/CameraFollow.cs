using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Variables
    // --------------------
    private Camera _thisCamera;
    [SerializeField] private GameObject _objToFollow;
    public float OffsetDistance = 10;
    public float SmoothTime = 0.25f;
    private Vector3 _camOffset;
    private Vector3 _camVelocity = Vector3.zero;
    public bool Follow;
    public bool SmoothDamp;

    public Vector3 CamOffset { get => _camOffset; }
    public GameObject ObjToFollow { get => _objToFollow; }


    // Default Methods
    // --------------------
    private void OnValidate()
    {
        _thisCamera = GetComponent<Camera>();
        CalibrateDistance();
    }

    private void Awake()
    {
        _thisCamera = GetComponent<Camera>();
    }

    void Update()
    {
        CalibrateDistance();
    }

    private void LateUpdate()
    {
        SettingFollowPos();
    }


    // Created Methods
    // --------------------
    private void CalibrateDistance()
    {
        _camOffset = new Vector3(-OffsetDistance, OffsetDistance, -OffsetDistance);
        _thisCamera.orthographicSize = OffsetDistance;
    }

    private void SettingFollowPos()
    {
        if (Follow && ObjToFollow)
        {
            if (!SmoothDamp)
            {
                transform.position = _camOffset + ObjToFollow.transform.position;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, _camOffset + ObjToFollow.transform.position, ref _camVelocity, SmoothTime);
            }
        }
    }
}
