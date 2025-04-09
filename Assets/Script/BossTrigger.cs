using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    public GameObject bossPrefab; // Assign the boss prefab in the inspector
    public Transform spawnPoint;  // Assign where the boss should spawn
    public GameObject bossBulletManager; // Assign the BossBulletManager object

    private bool bossSpawned = false; // Ensure boss only spawns once

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!bossSpawned && other.CompareTag("Player"))
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        // Instantiate the boss at the specified spawn point
        Instantiate(bossPrefab, spawnPoint.position, Quaternion.identity);

        // Enable BossBulletManager
        if (bossBulletManager != null)
        {
            bossBulletManager.SetActive(true);
        }

        // Ensure boss doesn't spawn again
        bossSpawned = true;
    }
}
