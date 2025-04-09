using UnityEngine;
using Cinemachine;

public class CameraZoomManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Zoom Settings")]
    public float normalDistance = 10f;
    public float zoomedOutDistance = 15f;
    public float zoomSpeed = 3f;

    [Header("Enemy Detection")]
    public float enemyDetectionRadius = 5f;
    public LayerMask enemyLayer;

    private float targetDistance;
    private CinemachineFramingTransposer transposer;

    private void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        if (transposer == null)
        {
            Debug.LogError("No FramingTransposer found on virtual camera.");
            return;
        }

        targetDistance = normalDistance;
        transposer.m_CameraDistance = normalDistance;
    }

    private void Update()
    {
        if (transposer == null) return;

        bool enemyNearby = IsEnemyNearby();
        targetDistance = enemyNearby ? zoomedOutDistance : normalDistance;

        // Smoothly interpolate the camera distance
        transposer.m_CameraDistance = Mathf.Lerp(
            transposer.m_CameraDistance,
            targetDistance,
            zoomSpeed * Time.deltaTime
        );
    }

    private bool IsEnemyNearby()
    {
        if (player == null) return false;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.position, enemyDetectionRadius, enemyLayer);
        foreach (var enemy in enemies)
        {
            if (enemy.CompareTag("enemy")) return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, enemyDetectionRadius);
    }
}
