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
    private int _waveCounter = 0;
    private bool _spawned = false;
    private bool _created = false;

    private void Start()
    {
        CreateEnemies();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_spawned)
        {
            StartCoroutine(SpawnEnemyWaves());
            _spawned = true;
        }
    }

    private void CreateEnemies()
    {
        if (!_created)
        {
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                for (int j = 0; j < enemyCounts[i]; j++)
                {
                    GameObject enemy = Instantiate(enemyPrefabs[i], RandomPos(), Quaternion.identity, transform);
                    _enemyInstances.Add(enemy);
                    enemy.SetActive(false);
                }
            }

            _created = true;
        }
    }

    Vector3 RandomPos()
    {
        float x = Random.Range(-range.x / 2, range.x / 2);
        float y = Random.Range(-range.y / 2, range.y / 2);
        float z = Random.Range(-range.z / 2, range.z / 2);
        return new Vector3(x, y, z) + transform.position;
    }

    float RandomDelay()
    {
        return Random.Range(0, SpawnMaxDelay);
    }

    IEnumerator SpawnEnemyWaves()
    {
        while (_waveCounter < numberOfWaves)
        {
            for (int i = 0; i < _enemyInstances.Count; i++)
            {
                StartCoroutine(ActivateWithDelay(RandomDelay(), _enemyInstances[i], RandomPos()));
            }
            _waveCounter++;
            yield return new WaitUntil(AllEnemiesSpawned);
            yield return new WaitUntil(AllEnemiesDead);
        }
        if (BeatWaves != null) { BeatWaves.Invoke(this, EventArgs.Empty); }
    }

    IEnumerator ActivateWithDelay(float delay, GameObject enemyPref, Vector3 spawnPos)
    {
        yield return new WaitForSeconds(delay);
        enemyPref.transform.position = spawnPos;
        enemyPref.SetActive(true);

    }

    bool AllEnemiesSpawned()
    {
        int totalEnemyAmount = 0;
        int totalEnemiesActive = 0;

        foreach (var count in enemyCounts)
        {
            totalEnemyAmount += count;
        }

        foreach (var enemy in _enemyInstances)
        {
            if (enemy.activeInHierarchy)
            {
                totalEnemiesActive++;
            }
        }

        if (totalEnemyAmount == totalEnemiesActive)
        {
            return true;
        }
        return false;
    }

    bool AllEnemiesDead()
    {
        int totalEnemiesActive = 0;

        foreach (var enemy in _enemyInstances)
        {
            if (enemy.activeInHierarchy)
            {
                totalEnemiesActive++;
            }
        }

        return totalEnemiesActive == 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, range);
    }
}