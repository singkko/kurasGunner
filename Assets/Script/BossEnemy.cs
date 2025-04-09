using UnityEngine;
using System.Collections;

public class BossEnemy : MonoBehaviour
{
    public float speed = 2f;
    public float chaseRange = 5f;
    public int hp = 10;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    public AudioClip hitSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isDying = false;
    private bool isKnockedBack = false;

    private GameObject bossBulletManager; // Reference to the bullet manager
    public GameObject jumpPadPrefab; // Reference to the JumpPad prefab

    public Vector3 jumpPadSpawnPosition; // Position where JumpPad will spawn

    private Transform[] dashPoints; // Now auto-finds in scene
    public float dashSpeed = 10f; // Speed of the dash
    private bool canDash = true; // Cooldown check

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        bossBulletManager = GameObject.Find("BossBulletManager");

        // Auto-find all Dash Points in the scene
        GameObject[] dashPointObjects = GameObject.FindGameObjectsWithTag("DashPoint");
        dashPoints = new Transform[dashPointObjects.Length];
        for (int i = 0; i < dashPointObjects.Length; i++)
        {
            dashPoints[i] = dashPointObjects[i].transform;
        }
    }

    void Update()
    {
        if (player == null || isDying || isKnockedBack) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
        {
            ChasePlayer();
        }

        // Start dash if not on cooldown
        if (canDash && dashPoints.Length > 0)
        {
            StartCoroutine(DashToRandomPoint());
        }
    }

    void ChasePlayer()
    {
        Vector2 targetPosition = player.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    IEnumerator DashToRandomPoint()
    {
        canDash = false; // Start cooldown

        Transform targetPoint = dashPoints[Random.Range(0, dashPoints.Length)]; // Pick a random point
        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;
        float dashDuration = 0.5f; // Adjust based on how fast you want the dash

        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPoint.position, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPoint.position; // Ensure it reaches exactly

        yield return new WaitForSeconds(5f); // Dash cooldown
        canDash = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullett") && !isDying)
        {
            hp -= 1;
            if (audioSource && hitSound)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (hp <= 0)
            {
                Die();
            }
            else
            {
                ApplyKnockback(collision.transform.position);
            }

            Destroy(collision.gameObject);
        }
    }

    void ApplyKnockback(Vector2 hitPosition)
    {
        if (rb != null)
        {
            isKnockedBack = true;
            Vector2 knockbackDirection = (transform.position - (Vector3)hitPosition).normalized;
            rb.velocity = knockbackDirection * knockbackForce;

            Invoke(nameof(ResetKnockback), knockbackDuration);
        }
    }

    void ResetKnockback()
    {
        isKnockedBack = false;
        rb.velocity = Vector2.zero;
    }

    void Die()
    {
        if (isDying) return;
        isDying = true;

        // Disable shooting
        EnemyShoot enemyShoot = GetComponent<EnemyShoot>();
        if (enemyShoot != null)
        {
            enemyShoot.DisableShooting();
        }

        // Disable BossBulletManager if it exists
        if (bossBulletManager != null)
        {
            bossBulletManager.SetActive(false);
        }

        // Spawn the JumpPad at the designated position
        if (jumpPadPrefab != null)
        {
            Instantiate(jumpPadPrefab, jumpPadSpawnPosition, Quaternion.identity);
        }

        // Play death sound
        if (audioSource && deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        speed = 0;
        if (animator != null)
        {
            animator.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        Destroy(gameObject, 1f);
    }
}
