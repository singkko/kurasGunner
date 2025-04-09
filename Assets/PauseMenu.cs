using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign your UI canvas or background sprite here
    public float normalTimeScale = 1f; // Normal time scale (1 is default)
    public float pausedTimeScale = 0f; // Paused time scale (0 stops time)

    void Update()
    {
        // Check if ESC is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseManager.IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);         // Hide the pause menu
        Time.timeScale = normalTimeScale;     // Return to normal time
        PauseManager.SetPause(false);         // Resume the game (unpause state)
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);          // Show the pause menu
        Time.timeScale = pausedTimeScale;     // Freeze time
        PauseManager.SetPause(true);          // Pause the game (pause state)
    }
}
