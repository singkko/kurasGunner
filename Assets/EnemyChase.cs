using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float speed = 2f;
    public float chaseRange = 5f;
    public int hp = 10;
    public float knockbackForce = 5f; // Knockback strength
    public float knockbackDuration = 0.2f; // Time before enemy can move again

    public AudioClip hitSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isDying = false;
    private bool isKnockedBack = false; // Prevents movement during knockback

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null) return;
    }

    void Update()
    {
        if (player == null || isDying || isKnockedBack) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector2 targetPosition = player.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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

        // Play death sound
        if (audioSource && deathSound)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        // Disable movement
        speed = 0;

        // Optional visual feedback
        if (animator != null)
        {
            animator.enabled = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        // Remove all colliders to prevent shot absorption
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // Destroy the game object after delay
        Destroy(gameObject, 1f);
    }

}
