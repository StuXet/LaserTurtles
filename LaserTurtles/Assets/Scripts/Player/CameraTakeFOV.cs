using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTakeFOV : MonoBehaviour
{
    private Camera _thisCamera,_parentCamera;

    // Start is called before the first frame update
    void Start()
    {
        _thisCamera = GetComponent<Camera>();
        _parentCamera = transform.parent.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        _thisCamera.orthographicSize = _parentCamera.orthographicSize;
    }
}
