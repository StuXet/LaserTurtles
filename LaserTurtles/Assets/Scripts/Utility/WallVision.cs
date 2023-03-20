using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallVision : MonoBehaviour
{
    [SerializeField] private GameObject _wallVisionUI;
    [SerializeField] private Transform _objectToZone;
    [SerializeField] private bool _useWallVision;
    private bool _isWallVision;

    // Start is called before the first frame update
    void Start()
    {
        _wallVisionUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_useWallVision)
        {
            RaycastHit hit;
            Vector3 dir = (_objectToZone.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, dir, out hit, 1000))
            {
                //Debug.Log(hit.transform.name);
                if (hit.transform.CompareTag("Player"))
                {
                    Debug.DrawRay(transform.position, dir * hit.distance, Color.green);
                    //Debug.Log("PLayer Visible");
                    _isWallVision = false;
                }
                else
                {
                    Debug.DrawRay(transform.position, dir * 1000, Color.red);
                    //Debug.Log("Player Blocked");
                    _isWallVision = true;
                }
            }

            _wallVisionUI.SetActive(_isWallVision);
        }
        else
        {
            _wallVisionUI.SetActive(false);
        }
    }
}
