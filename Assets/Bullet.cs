using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))  // Use correct tag
        {
            Destroy(gameObject);  // Destroy the bullet, not the ground
        }
    }
}
