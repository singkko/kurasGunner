using UnityEngine;

public class EnemySeparation : MonoBehaviour
{
    public float pushForce = 2f;  // Adjust force strength
    public float detectionRadius = 0.5f;  // Adjust radius based on enemy size

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        SeparateFromOthers();
    }

    void SeparateFromOthers()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D enemy in nearbyEnemies)
        {
            if (enemy.gameObject != gameObject && enemy.CompareTag("enemy"))
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

                if (enemyRb != null)
                {
                    Vector2 pushDirection = (transform.position - enemy.transform.position).normalized;
                    rb.AddForce(pushDirection * pushForce, ForceMode2D.Force);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw detection radius in Scene View for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
