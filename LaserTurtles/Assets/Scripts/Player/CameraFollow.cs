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
    private Vector3 _camOffset;
    public bool Follow;


    // Default Methods
    // --------------------
    private void OnValidate()
    {
        _thisCamera = GetComponent<Camera>();
    }

    private void Awake()
    {
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
        if (Follow && _objToFollow)
        {
            transform.position = _camOffset + _objToFollow.transform.position;
        }
        else
        {
            transform.position = _camOffset;
        }
    }
}
