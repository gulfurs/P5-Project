using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubtitleFollow : MonoBehaviour
{
    public Transform playerCamera;  // Reference to the camera (usually the main camera or VR headset camera)
    public float distanceFromCamera = 2.0f;  // Distance in front of the camera
    public Vector3 offset = new Vector3(0, -0.5f, 0);  // Offset to adjust the position

    void Update()
    {
        // Set the position in front of the camera
        Vector3 forwardPosition = playerCamera.position + playerCamera.forward * distanceFromCamera;
        transform.position = forwardPosition + offset;

        // Make sure the subtitles always face the camera
        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0);  // Rotate 180 degrees to face correctly
    }
}
