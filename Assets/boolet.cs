using UnityEngine;

public class boolet : MonoBehaviour
{
    public GameObject shot;              // Assign your projectile prefab here
    public Transform shootingPoint;      // Assign the shooting point (child object at the weapon's tip)
    public float projectileSpeed = 10f;  // Speed of the projectile

    void Update()
    {
        // Check if the Spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Convert the mouse position from screen space to world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;  // Set Z to 0 since it's a 2D game

        // Calculate the direction from the shooting point to the mouse position
        Vector2 direction = (mousePosition - shootingPoint.position).normalized;

        // Instantiate the projectile at the shooting point's position and rotation
        GameObject projectile = Instantiate(shot, shootingPoint.position, Quaternion.identity);

        // Get the Rigidbody2D component from the projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        // Apply velocity in the direction of the cursor
        rb.velocity = direction * projectileSpeed;
    }
}
