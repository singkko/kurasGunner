using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bulletVFX;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))  // Use correct tag
        {
            Instantiate(bulletVFX, transform.position, bulletVFX.transform.rotation);
            Destroy(gameObject);  // Destroy the bullet, not the ground
        }
    }
}
