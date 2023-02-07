using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallVision : MonoBehaviour
{
    [SerializeField] private GameObject _wallVisionUI;
    [SerializeField] private Transform _objectToZone;
    [SerializeField] private LayerMask _playerLayer, _environmentLayer;

    // Start is called before the first frame update
    void Start()
    {
        _wallVisionUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 dir = (_objectToZone.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, dir, out hit, 1000, _playerLayer, QueryTriggerInteraction.UseGlobal))
        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.green);
            Debug.Log("PLayer Visible");
            _wallVisionUI.SetActive(false);
        }
        else
        {
            Debug.DrawRay(transform.position, dir * 1000, Color.red);
            Debug.Log("Player Blocked");
            _wallVisionUI.SetActive(true);
        }
    }
}
