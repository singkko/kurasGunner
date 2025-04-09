using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float speed = 2f;           // Speed of the enemy
    public Transform pointA;           // Left boundary point
    public Transform pointB;           // Right boundary point
    public Transform player;           // Reference to the player
    public float chaseRange = 5f;      // Distance to start chasing the player
    public int hp = 10;                // Enemy health points

    public AudioClip hitSound;         // Sound when hit
    public AudioClip deathSound;       // Sound when destroyed

    private AudioSource audioSource;   // Reference to the AudioSource
    private bool movingRight = true;   // Direction flag
    private bool isChasing = false;    // Whether the enemy is currently chasing the player

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get AudioSource component
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < chaseRange)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Roam();
        }
    }

    void Roam()
    {
        if (isChasing) return; // Do not roam if chasing the player

        if (movingRight)
        {
            transform.position = Vector2.MoveTowards(transform.position, pointB.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, pointB.position) < 0.1f)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, pointA.position, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, pointA.position) < 0.1f)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    void ChasePlayer()
    {
        Vector2 targetPosition = player.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Flip the enemy to face the player
        if (player.position.x > transform.position.x && !movingRight)
        {
            movingRight = true;
            Flip();
        }
        else if (player.position.x < transform.position.x && movingRight)
        {
            movingRight = false;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Flip the enemy's sprite on the X axis
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("bullett"))
        {
            hp -= 1;  // Subtract health by 1 when hit by a bullet

            // Play hit sound
            if (audioSource && hitSound)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Destroy the enemy if health is 0 or less
            if (hp <= 0)
            {
                // Play death sound before destroying the enemy
                if (audioSource && deathSound)
                {
                    AudioSource.PlayClipAtPoint(deathSound, transform.position);
                }

                Destroy(gameObject);  // Destroy the enemy sprite
            }

            Destroy(collision.gameObject);  // Optionally destroy the bullet as well
        }
    }
}
