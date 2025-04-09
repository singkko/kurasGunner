using UnityEngine;

public class EnemyCurveShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float projectileSpeed = 5f;
    public float shootRange = 7f;

    public AudioClip shootSound;
    private AudioSource audioSource;

    private Transform player;
    private float nextShootTime;
    private bool canShoot = true; // Controls shooting

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null || !canShoot) return; // Stop shooting if disabled

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < shootRange && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootInterval;
        }
    }

    void Shoot()
    {
        if (firePoint == null || projectilePrefab == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;
            rb.velocity = direction * projectileSpeed;
        }

        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    // Call this method to disable shooting when enemy dies
    public void DisableShooting()
    {
        canShoot = false;
    }
}
