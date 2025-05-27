using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public int playerHealth = 9;
    public int maxHealth = 9;
    public int regenAmount = 1;
    public float regenDelay = 5f;
    public float regenInterval = 1f;
    public float regenSpeedMultiplier = 1f;

    public TMP_Text healthText;
    public TMP_Text scoreText;
    public TMP_Text comboText;
    public GameObject playerObject;

    public GameObject gameOverPanel; // Assign in inspector
    public AudioSource gameOverSFX;  // Assign in inspector
    public GameObject bgmManager;    // Assign in inspector

    private int score = 0;
    private int comboMultiplier = 1;
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
        UpdateScoreUI();
        UpdateComboUI();
        gameOverPanel.SetActive(false);
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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateComboUI()
    {
        if (comboText != null)
        {
            comboText.text = "Combo: X" + comboMultiplier.ToString();
        }
    }

    private void DestroyPlayer()
    {
        if (playerObject != null)
        {
            Destroy(playerObject);
        }

        // Stop BGM
        if (bgmManager != null)
        {
            DynamicBGMManager bgm = bgmManager.GetComponent<DynamicBGMManager>();
            if (bgm != null)
            {
                bgm.StopAllMusic();
            }
        }

        // Disable UI elements
        if (healthText != null) healthText.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (comboText != null) comboText.gameObject.SetActive(false);

        // Show Game Over UI
        gameOverPanel.SetActive(true);

        // Play Game Over sound
        if (gameOverSFX != null)
        {
            gameOverSFX.Play();
        }
    }


    public void AddScore(int points)
    {
        score += points * comboMultiplier;
        UpdateScoreUI();
    }

    public void ResetComboMultiplier()
    {
        comboMultiplier = 1;
        UpdateComboUI();
    }

    public void IncreaseComboMultiplier()
    {
        if (comboMultiplier < 5)
        {
            comboMultiplier++;
        }
        UpdateComboUI();
    }

    // UI Buttons
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0); // Assumes index 0 is main menu
    }
}
