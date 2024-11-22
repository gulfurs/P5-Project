using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMotor : MonoBehaviour
{
    // The layer index for "Face" in the NPC's Animator
    private const int faceLayerIndex = 1;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Animator npcAnimator = other.GetComponent<Animator>();
        if (npcAnimator != null)
        {
            
            npcAnimator.SetLayerWeight(faceLayerIndex, 1);
            CustomAnimatorProfile customFace = other.GetComponent<CustomAnimatorProfile>();
            if (customFace != null) {
                npcAnimator.SetLayerWeight(faceLayerIndex, customFace.faceWeight);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Animator npcAnimator = other.GetComponent<Animator>();
        if (npcAnimator != null)
        {
            npcAnimator.SetLayerWeight(faceLayerIndex, 0);
        }
    }
}
