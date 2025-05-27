using UnityEngine;

public class PlayerRecoil : MonoBehaviour
{
    public float slowTimeScale = 0.5f;      // Adjust this to set how slow time gets when left-click is held
    public float normalTimeScale = 1f;      // Normal game speed
    private Rigidbody2D rb;

    private bool isPaused = false;  // Track the pause state

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   // Get the Rigidbody2D attached to the player
        rb.freezeRotation = true;           // Prevent the player from rotating
    }

    void Update()
    {
        // Do nothing if the game is paused
        if (isPaused)
        {
            return;
        }

        // Slow motion on left-click
        if (Input.GetMouseButton(0))
        {
            Time.timeScale = slowTimeScale;  // Slow down time
        }
        else
        {
            Time.timeScale = normalTimeScale;  // Return to normal time when left click is released
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("destroyobject"))
        {
            Destroy(collision.gameObject);  // Destroy the object upon collision
        }
    }

    // You can call this method to pause or resume the game manually from other scripts
    public void SetPauseState(bool pause)
    {
        isPaused = pause;
    }
}
