using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonLoader : MonoBehaviour
{
    // Loads the next scene based on build index
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    // Quits the application
    public void QuitGame()
    {
        Debug.Log("Quit Game");  // This will show in the Editor console
        Application.Quit();      // This works in a built application
    }
}
