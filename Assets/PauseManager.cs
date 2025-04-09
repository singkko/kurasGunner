using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    public static void SetPause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0f : 1f; // Pause or resume the game
    }
}
