using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public event EventHandler BeatWaves;

    public List<GameObject> enemyPrefabs;
    public List<int> enemyCounts;
    public Vector3 range;
    public int numberOfWaves = 1;
    public float SpawnMaxDelay = 1;

    private List<GameObject> _enemyInstances = new List<GameObject>();
    private int waveCounter = 0;
    private bool spawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !spawned)
        {
            StartCoroutine(SpawnEnemyWaves());
            spawned = true;
        }
    }

    IEnumerator SpawnEnemyWaves()
    {
        while (waveCounter < numberOfWaves)
        {
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                for (int j = 0; j < enemyCounts[i]; j++)
                {
                    float x = Random.Range(-range.x / 2, range.x / 2);
                    float y = Random.Range(-range.y / 2, range.y / 2);
                    float z = Random.Range(-range.z / 2, range.z / 2);
                    Vector3 randomPos = transform.position + new Vector3(x, y, z);

                    //GameObject enemy = Instantiate(enemyPrefabs[i], randomPos, Quaternion.identity);
                    //_enemyInstances.Add(enemy);

                    float randNum = Random.Range(0, SpawnMaxDelay);
                    StartCoroutine(SpawnWithDelay(randNum, enemyPrefabs[i], randomPos));
                }
            }
            waveCounter++;
            yield return new WaitUntil(AllEnemiesSpawned);
            yield return new WaitUntil(AllEnemiesDead);
        }
        if (BeatWaves != null) { BeatWaves.Invoke(this, EventArgs.Empty); }
    }

    IEnumerator SpawnWithDelay(float delay, GameObject enemyPref, Vector3 spawnPos)
    {
        yield return new WaitForSeconds(delay);
        GameObject enemy = Instantiate(enemyPref, spawnPos, Quaternion.identity);
        _enemyInstances.Add(enemy);
    }

    bool AllEnemiesSpawned()
    {
        int totalEnemyAmount = 0;

        foreach (var count in enemyCounts)
        {
            totalEnemyAmount += count;
        }

        if (_enemyInstances.Count == totalEnemyAmount)
        {
            return true;
        }
        return false;
    }

    bool AllEnemiesDead()
    {
        for (int i = 0; i < _enemyInstances.Count; i++)
        {
            if (_enemyInstances[i] == null)
            {
                _enemyInstances.RemoveAt(i);
            }
        }

        return _enemyInstances.Count == 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, range);
    }
}