using UnityEngine;
using System.Collections;

public class BossBulletSpiral : MonoBehaviour
{
    public GameObject bulletPrefab; // Assign the bullet prefab in the Inspector
    public Transform[] spawnPoints; // Assign multiple spawn points in the Inspector
    public float bulletSpeed = 5f; // Speed of bullets
    public float fireRate = 0.2f; // Time between individual shots
    public float rotationSpeed = 45f; // Rotation speed in degrees per second
    public float bulletLifetime = 5f; // Destroy bullets after 5 seconds
    public float shootingDuration = 3f; // Time each spawn point shoots
    public float cooldownBetweenPoints = 5f; // Cooldown before switching spawn points
    public float fullCycleCooldown = 10f; // Cooldown after all points are used

    public AudioClip shootSFX; // Assign shooting sound in Inspector
    private AudioSource audioSource;

    private float currentRotation = 0f; // Keeps track of the current angle

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        StartCoroutine(FireBulletSpiral());
    }

    IEnumerator FireBulletSpiral()
    {
        while (true) // Infinite loop for continuous shooting
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                Transform currentSpawnPoint = spawnPoints[i];

                // Shoot for a set duration (3 seconds)
                float shootEndTime = Time.time + shootingDuration;
                while (Time.time < shootEndTime)
                {
                    FireBullets(currentSpawnPoint.position);

                    // Rotate the bullet spawn direction gradually
                    currentRotation -= rotationSpeed * fireRate;

                    yield return new WaitForSeconds(fireRate);
                }

                // Wait for cooldown before moving to the next spawn point
                yield return new WaitForSeconds(cooldownBetweenPoints);
            }

            // Wait for a full cycle cooldown (10 seconds)
            yield return new WaitForSeconds(fullCycleCooldown);
        }
    }

    void FireBullets(Vector2 spawnPosition)
    {
        for (int i = 0; i < 4; i++) // Fire in 4 directions (N, E, S, W)
        {
            float angle = currentRotation + (i * 90f); // Cardinal directions + rotation
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }

            // Destroy bullet after bulletLifetime seconds
            Destroy(bullet, bulletLifetime);
        }

        // Play shooting sound effect
        if (shootSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSFX);
        }
    }
}
