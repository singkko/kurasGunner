using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float FollowSpeed = 2f;
    public float yOffset = 1f;
    public Transform target;
    public float mouseBiasFactor = 0.5f;
    public float maxDistance = 5f;

    public float enemyDetectionRadius = 7f;
    public float zoomOutAmount = 1.5f;
    public float zoomSpeed = 2f;

    private Camera cam;
    private float originalOrthoSize;

    void Start()
    {
        cam = Camera.main;
        originalOrthoSize = cam.orthographicSize;
    }

    void Update()
    {
        if (target == null)
        {
            GameObject newTarget = GameObject.FindGameObjectWithTag("Player");
            if (newTarget != null)
                target = newTarget.transform;
            else
                return;
        }

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 direction = (mousePos - target.position);
        float distance = Mathf.Min(direction.magnitude, maxDistance);
        Vector3 biasPos = target.position + direction.normalized * distance * mouseBiasFactor;

        // Enemy detection
        Collider2D[] enemies = Physics2D.OverlapCircleAll(target.position, enemyDetectionRadius, LayerMask.GetMask("Default")); // assumes enemies on Default
        Transform nearestEnemy = null;
        float minDist = Mathf.Infinity;

        foreach (var col in enemies)
        {
            if (col.CompareTag("enemy"))
            {
                float d = Vector2.Distance(target.position, col.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearestEnemy = col.transform;
                }
            }
        }

        Vector3 focusPoint = biasPos;

        // If enemy nearby, adjust focus between player and enemy, and zoom out
        if (nearestEnemy != null)
        {
            focusPoint = (target.position + nearestEnemy.position) / 2f;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalOrthoSize + zoomOutAmount, zoomSpeed * Time.deltaTime);
        }
        else
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalOrthoSize, zoomSpeed * Time.deltaTime);
        }

        Vector3 newPos = new Vector3(focusPoint.x, focusPoint.y + yOffset, -10f);
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // For visualizing the enemy detection radius
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, enemyDetectionRadius);
        }
    }
}
