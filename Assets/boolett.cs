using UnityEngine;

public class boolett : MonoBehaviour
{
    public GameObject boolet;   // Assign your projectile prefab here
    public Transform shootingPoint;       // Assign the shooting point (child object at the weapon's tip)
    public float projectileSpeed = 10f;   // Speed of the projectile

    void Update()
    {
        // Assuming you want to shoot with the right mouse button
        if (Input.GetMouseButtonDown(1)) // 0 for left-click, 1 for right-click
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Create the projectile at the shooting point's position and rotation
        GameObject projectile = Instantiate(boolet, shootingPoint.position, shootingPoint.rotation);

        // Get the Rigidbody2D component from the projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        // Apply velocity in the direction the shooting point is facing (forward direction)
        rb.velocity = shootingPoint.right * projectileSpeed;
    }
}
