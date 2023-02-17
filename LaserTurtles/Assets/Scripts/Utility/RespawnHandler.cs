using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHandler : MonoBehaviour
{
    private CheckpointSystem _checkpointSystem;
    private PlayerController _playerController;

    private bool _colliding;

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
