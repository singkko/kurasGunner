using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Assign your Enemy prefab in the Inspector
    public Transform[] spawnPoints; // Assign 3 spawn points in the Inspector
    public float spawnInterval = 2f; // Time between spawns
    public int maxSpawnCount = 10; // Maximum number of enemies to spawn

    private int currentSpawnCount = 0;
    private bool isActivated = false;
    private int lastSpawnIndex = -1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (currentSpawnCount < maxSpawnCount)
        {
            int newSpawnIndex;
            do
            {
                newSpawnIndex = Random.Range(0, spawnPoints.Length);
            } while (newSpawnIndex == lastSpawnIndex); // Ensure different spawn point

            lastSpawnIndex = newSpawnIndex;
            Instantiate(enemyPrefab, spawnPoints[newSpawnIndex].position, Quaternion.identity);
            currentSpawnCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
