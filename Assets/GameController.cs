using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Vector2 startPos;
    public float respawnDelay = 1f; // Time delay before respawning

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Aray"))
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(RespawnWithDelay());
    }

    IEnumerator RespawnWithDelay()
    {
        yield return new WaitForSeconds(respawnDelay); // Waits for the delay
        Respawn();
    }

    void Respawn()
    {
        transform.position = startPos;
    }
}
