using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public string playerTag;
    
    public GameObject enemyPrefab;
    public int numberOfEnemies;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float minZ;
    public float maxZ;

    private bool spawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == playerTag && !spawned)
        {
            for (int i = 0; i < numberOfEnemies; i++)
            {
                float x = Random.Range(minX, maxX);
                float y = Random.Range(minY, maxY);
                float z = Random.Range(minZ, maxZ);
                Vector3 randomPos = new Vector3(x, y, z);

                GameObject enemy = Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            }
            spawned = true;
        }
    }
}