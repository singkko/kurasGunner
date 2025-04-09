using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportPoint; // Assign in the Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Check if it's the player
        {
            if (teleportPoint != null) // Ensure there's a teleport point assigned
            {
                collision.transform.position = teleportPoint.position; // Move player to teleport point
            }
            else
            {
                Debug.LogWarning("No teleport point assigned to " + gameObject.name);
            }
        }
    }
}
