using UnityEngine;

public class FlyingEnemyChase : MonoBehaviour
{
    public float speed = 2f;
    public float chaseRange = 8f;
    public float stopDistance = 3f;
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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null || isDying || isKnockedBack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance)
        {
            ChasePlayer();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    void ChasePlayer()
    {
        Vector2 enemyPos = transform.position;
        Vector2 playerPos = player.position;

        float verticalMove = playerPos.y - enemyPos.y;
        float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);

        Vector2 newVelocity = Vector2.zero;

        // Always try to align vertically with the player
        if (Mathf.Abs(verticalMove) > 0.1f)
        {
            newVelocity.y = Mathf.Sign(verticalMove) * speed;
        }

        // Move horizontally depending on distance
        if (horizontalDistance > stopDistance)
        {
            newVelocity.x = Mathf.Sign(playerPos.x - enemyPos.x) * speed;
        }
        else if (horizontalDistance < stopDistance * 0.8f)
        {
            newVelocity.x = -Mathf.Sign(playerPos.x - enemyPos.x) * speed;
        }

        // Normalize only if moving diagonally
        if (newVelocity.sqrMagnitude > speed * speed)
        {
            newVelocity = newVelocity.normalized * speed;
        }

        rb.velocity = newVelocity;

        // Flip sprite based on horizontal movement only
        if (Mathf.Abs(newVelocity.x) > 0.01f)
        {
            float flipDirection = newVelocity.x > 0 ? 1f : -1f;
            transform.localScale = new Vector3(flipDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
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
                GameManager.Instance.AddScore(100);
            }
            else
            {
                ApplyKnockback(collision.transform.position);
                GameManager.Instance.IncreaseComboMultiplier();
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

        // Disable shooting when the enemy dies
        FlyingEnemySprayShoot enemyShoot = GetComponent<FlyingEnemySprayShoot>();
        if (enemyShoot != null)
        {
            enemyShoot.DisableShooting();
        }

        // Play death sound
        if (audioSource && deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Stop chasing
        rb.velocity = Vector2.zero;
        speed = 0;

        // Visual feedback
        if (animator != null) animator.enabled = false;
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f);

        // Remove all colliders to prevent future triggers
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // Let the enemy fall and spin by enabling dynamic body & unfreezing rotation
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
            rb.freezeRotation = false;
            rb.constraints = RigidbodyConstraints2D.None;
        }

        // Destroy after delay
        Destroy(gameObject, 1f);

        // Reset combo multiplier
        GameManager.Instance.ResetComboMultiplier();
    }
}
