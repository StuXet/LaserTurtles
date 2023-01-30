using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public List<int> enemyCounts;
    public Vector3 range;
    public int numberOfWaves = 1;

    private int waveCounter = 0;
    private bool spawned = false;
    private bool _beatWave = false;

    public bool BeatWave { get => _beatWave;}

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

                    GameObject enemy = Instantiate(enemyPrefabs[i], randomPos, Quaternion.identity);
                }
            }
            waveCounter++;
            yield return new WaitUntil(AllEnemiesDead);
        }
    }

    bool AllEnemiesDead()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length == 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, range);
    }
}