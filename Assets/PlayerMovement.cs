using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float ascentMultiplier = 2f; // Makes ascent faster without increasing height
    public float jumpCooldown = 0.5f; // Adjustable cooldown for jumping
    public float inertia = 0.1f; // The smoothness of the movement (1 for no inertia, lower for more)
    public float fallMultiplier = 2.5f; // Controls fall speed, increases gravity when falling

    public AudioClip jumpSound;  // Reference to the jump sound clip
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool canJump = true; // Ensures only one jump is allowed
    private float jumpTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
    }

    void Update()
    {
        HandleMovementInput();
        UpdateJumpCooldown();
        ApplyGravityMultiplier();
    }

    void FixedUpdate()
    {
        // Smoothly interpolate the horizontal velocity to simulate inertia
        float targetVelocityX = moveDirection.x * moveSpeed;
        float smoothVelocityX = Mathf.Lerp(rb.velocity.x, targetVelocityX, inertia);
        rb.velocity = new Vector2(smoothVelocityX, rb.velocity.y);
    }

    void HandleMovementInput()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        moveDirection = new Vector2(moveInput, 0);

        // Jumping with cooldown
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            // Play jump sound
            if (audioSource && jumpSound)
            {
                audioSource.PlayOneShot(jumpSound);
            }

            // Apply the jump force using AddForce
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canJump = false; // Disable jumping
            jumpTimer = jumpCooldown; // Start cooldown timer
        }
    }

    void UpdateJumpCooldown()
    {
        if (!canJump)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                canJump = true; // Re-enable jumping after cooldown
            }
        }
    }

    void ApplyGravityMultiplier()
    {
        // Check if the player is rising or falling
        if (rb.velocity.y > 0) // Rising
        {
            rb.gravityScale = ascentMultiplier;
        }
        else if (rb.velocity.y < 0) // Falling
        {
            rb.gravityScale = fallMultiplier; // Increase gravity during fall
        }
        else // When on the ground
        {
            rb.gravityScale = 1f; // Default gravity
        }
    }
}
