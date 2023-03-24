using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnHandler : MonoBehaviour
{
    private CheckpointSystem _checkpointSystem;
    private PlayerController _playerController;

    private bool _colliding;

    private GameObject _tempObj;

    private void Awake()
    {
        _checkpointSystem = CheckpointSystem.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController = other.GetComponent<PlayerController>();
            _colliding = true;

            StartCoroutine(RespawnDelay());
        }
        else if (other.CompareTag("Item"))
        {
            _tempObj = other.gameObject;
            if (_tempObj != null) _tempObj.transform.position = _checkpointSystem.LatestCheckpoint.position + new Vector3(0, 1, 0);
            _tempObj = null;
        }
        else if (other.CompareTag("Damager"))
        {
            _tempObj = other.gameObject;
            if (other.TryGetComponent(out ItemObject item))
            {
                if (item.CanBePicked)
                {
                    if (_tempObj != null) _tempObj.transform.position = _checkpointSystem.LatestCheckpoint.position + new Vector3(0, 1, 0);
                    _tempObj = null;
                }
            }
        }
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForEndOfFrame();
        if (!_playerController.IsDead && _colliding)
        {
            _playerController.transform.position = _checkpointSystem.LatestCheckpoint.position + new Vector3(0, 2, 0);
        }
        _colliding = false;
    }
}
