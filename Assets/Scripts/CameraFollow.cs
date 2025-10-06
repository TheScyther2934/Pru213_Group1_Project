using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;     // Player
    public float smoothSpeed = 0.125f;  // Follow smoothness
    public Vector3 offset;       // Optional offset

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position = player's position + offset
        Vector3 desiredPosition = target.position + offset;
        // Smoothly move the camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
