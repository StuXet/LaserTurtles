using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private CheckpointSystem _checkpointSystem;

    private void Awake()
    {
        _checkpointSystem = GetComponentInParent<CheckpointSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _checkpointSystem.LatestCheckpoint = transform;
        }
    }
}
