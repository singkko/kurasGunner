using UnityEngine;

public class DynamicBGMManager : MonoBehaviour
{
    public AudioSource baseBGM;
    public AudioSource enemyBGM;
    public float detectionRange = 10f;
    public float fadeSpeed = 1f;

    private Transform player;
    private bool wasEnemyNearby = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        baseBGM.volume = 1f;
        enemyBGM.volume = 0f;
        enemyBGM.Play();
        enemyBGM.Pause();
    }

    void Update()
    {
        if (player == null) return;

        bool enemyNearby = false;

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("enemy"))
        {
            if (Vector2.Distance(player.position, enemy.transform.position) < detectionRange)
            {
                enemyNearby = true;
                break;
            }
        }

        if (enemyNearby && !wasEnemyNearby)
        {
            baseBGM.volume = 0f;
            enemyBGM.Stop();
            enemyBGM.Play();
        }
        else if (!enemyNearby && wasEnemyNearby)
        {
            enemyBGM.Pause();
        }

        if (enemyNearby)
        {
            enemyBGM.volume = Mathf.MoveTowards(enemyBGM.volume, 1f, fadeSpeed * Time.deltaTime);
        }
        else
        {
            baseBGM.volume = Mathf.MoveTowards(baseBGM.volume, 1f, fadeSpeed * Time.deltaTime);
            enemyBGM.volume = Mathf.MoveTowards(enemyBGM.volume, 0f, fadeSpeed * Time.deltaTime);
        }

        wasEnemyNearby = enemyNearby;
    }

    public void StopAllMusic()
    {
        if (baseBGM != null)
        {
            baseBGM.Stop();
        }

        if (enemyBGM != null)
        {
            enemyBGM.Stop();
        }
    }
}
