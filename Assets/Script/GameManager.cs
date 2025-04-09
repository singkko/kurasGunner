using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public int playerHealth = 9; // Player's starting health
    public int maxHealth = 9; // Maximum player health
    public int regenAmount = 1; // Amount of health to regenerate
    public float regenDelay = 5f; // Time before regeneration starts
    public float regenInterval = 1f; // Time between each regen tick
    public float regenSpeedMultiplier = 1f; // Adjustable speed multiplier for regeneration

    public TMP_Text healthText; // TextMeshPro UI element to display health
    public GameObject playerObject; // Reference to the player GameObject

    private Coroutine regenCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        UpdateHealthUI();

        if (playerHealth <= 0)
        {
            Debug.Log("Player has died!");
            DestroyPlayer();
        }
        else
        {
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }
    }

    private IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(regenDelay);

        while (playerHealth < maxHealth)
        {
            playerHealth += regenAmount;
            if (playerHealth > maxHealth)
            {
                playerHealth = maxHealth;
            }
            UpdateHealthUI();
            yield return new WaitForSeconds(regenInterval / regenSpeedMultiplier);
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + playerHealth + "/" + maxHealth;
        }
    }

    private void DestroyPlayer()
    {
        if (playerObject != null)
        {
            Destroy(playerObject); // Destroys the player GameObject
            Debug.Log("Player object destroyed!");
        }
    }
}
