using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveBeatWave : ObjectiveBase
{
    [SerializeField] private EnemySpawner _spawner;

    private void Awake()
    {
        _spawner.BeatWaves += _spawner_BeatWaves;
    }

    // Update is called once per frame
    void Update()
    {
        FinishedObjective();
    }


    private void _spawner_BeatWaves(object sender, System.EventArgs e)
    {
        BeginObjective();
        ObjectiveRequirementMet();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!CompletedObjective)
    //    {
    //        BeginObjective();
    //    }
    //}
}
