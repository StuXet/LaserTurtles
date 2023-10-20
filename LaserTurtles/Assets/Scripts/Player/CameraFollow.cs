using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Variables
    // --------------------
    private Camera _thisCamera;
    [SerializeField] private GameObject _objToFollow;
    [SerializeField] private float OffsetDistance = 10;
    private float _defaultOffsetDistance;
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
        _camOffset = new Vector3(-OffsetDistance, OffsetDistance, -OffsetDistance);
        _thisCamera.orthographicSize = OffsetDistance;
    }

    private void Awake()
    {
        _thisCamera = GetComponent<Camera>();
        _defaultOffsetDistance = OffsetDistance;
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
    public void ChangeCamDistance(float num)
    {
        if (_defaultOffsetDistance + num > 0)
        {
            OffsetDistance = _defaultOffsetDistance + num;
        }
    }

    public void ResetCamDistance()
    {
        OffsetDistance = _defaultOffsetDistance;
    }

    private void CalibrateDistance()
    {
        _camOffset = new Vector3(-OffsetDistance, OffsetDistance, -OffsetDistance);

        float camSize = _thisCamera.orthographicSize;

        if (camSize <= OffsetDistance + 0.01f && camSize >= OffsetDistance - 0.01f)
        {
            _thisCamera.orthographicSize = OffsetDistance;
        }
        else
        {
            float tempDist = Mathf.Lerp(camSize, OffsetDistance, Time.deltaTime);
            _thisCamera.orthographicSize = tempDist;
        }
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
