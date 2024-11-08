using UnityEngine;

public class FollowHeadsetEndScene : MonoBehaviour
{
    public Transform headset; // Assign the VR headset (Camera) here in the Inspector

    void Update()
    {
        if (headset != null)
        {
            // Position the canvas a short distance in front of the headset
            transform.position = headset.position + headset.forward * 2.0f;

            // Rotate the canvas to face the headset
            transform.rotation = Quaternion.LookRotation(headset.forward);
        }
    }
}
