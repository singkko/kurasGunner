using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 1; // Damage dealt to the player

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.TakeDamage(damage); // Reduce player health
            Destroy(gameObject); // Destroy the projectile on impact
        }

        if (other.CompareTag("Ground"))
        {
            
            Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}
