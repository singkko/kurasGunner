using UnityEngine;

public class IgnoreEnemyCollisions : MonoBehaviour
{
    void Start()
    {
        // Ignore collision with anything tagged as "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), enemy.GetComponent<Collider2D>());
        }
    }

   
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            // Ignore collision with enemies (if needed)
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
