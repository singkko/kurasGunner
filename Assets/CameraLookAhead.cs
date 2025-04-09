using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAhead : MonoBehaviour
{
    public Transform player;          // Reference to the player
    public float maxOffset = 2f;      // Max distance the camera can offset
    public float smoothSpeed = 5f;    // How smoothly the camera follows the offset

    private Vector3 targetPosition;

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        // Make sure to use correct depth (Z distance from camera to player or camera target)
        float zDepth = Mathf.Abs(Camera.main.transform.position.z - player.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, zDepth));
        mouseWorldPos.z = 0f;

        // Get direction from player to mouse
        Vector3 offset = mouseWorldPos - player.position;

        // Clamp to avoid too far look-ahead
        offset = Vector3.ClampMagnitude(offset, maxOffset);

        // Smoothly move the camera target toward the desired look-ahead position
        targetPosition = Vector3.Lerp(transform.position, player.position + offset, smoothSpeed * Time.deltaTime);
        targetPosition.z = transform.position.z; // Keep Z locked in 2D

        transform.position = targetPosition;
    }

}
