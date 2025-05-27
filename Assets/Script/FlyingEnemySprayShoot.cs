using UnityEngine;
using System.Collections;

public class FlyingEnemySprayShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int bulletCount = 5;
    public float spreadAngle = 45f;
    public float shootInterval = 3f;
    public float timeBetweenBullets = 0.1f;
    public float projectileSpeed = 5f;
    public float shootRange = 7f;

    public AudioClip shootSound;
    private AudioSource audioSource;

    private Transform player;
    private float nextShootTime;
    private bool canShoot = true;
    private bool isSpraying = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player == null || !canShoot || isSpraying) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < shootRange && Time.time >= nextShootTime)
        {
            StartCoroutine(SprayShoot());
            nextShootTime = Time.time + shootInterval;
        }
    }

    IEnumerator SprayShoot()
    {
        if (projectilePrefab == null || firePoint == null) yield break;

        isSpraying = true;

        Vector2 toPlayer = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - spreadAngle / 2f;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 shootDirection = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = shootDirection * projectileSpeed;

            if (audioSource != null && shootSound != null)
                audioSource.PlayOneShot(shootSound);

            yield return new WaitForSeconds(timeBetweenBullets);
        }

        isSpraying = false;
    }

    public void DisableShooting()
    {
        canShoot = false;
    }
}
