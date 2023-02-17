using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance;

    private Checkpoint[] _checkpoints = new Checkpoint[0];

    private Transform _latestCheckpoint;

    public Transform LatestCheckpoint { get => _latestCheckpoint; set => _latestCheckpoint = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _checkpoints = GetComponentsInChildren<Checkpoint>();
        _latestCheckpoint = _checkpoints[0].transform;
    }
}
