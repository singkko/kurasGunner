using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerLoader : MonoBehaviour
{
    [Tooltip("Scene index to load when player touches this object.")]
    public int sceneIndexToLoad = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneIndexToLoad);
        }
    }
}
