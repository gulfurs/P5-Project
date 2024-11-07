using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMotor : MonoBehaviour
{
    // The layer index for "Face" in the NPC's Animator
    private const int faceLayerIndex = 1;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has an Animator component with the "NPCController" controller
        Animator npcAnimator = other.GetComponent<Animator>();
        if (npcAnimator != null /*&& npcAnimator.runtimeAnimatorController.name == "NPCController"*/)
        {
            // Set the "Face" layer weight to 1
            Debug.Log("IT WOOOOOOOOOOOOOOOORKS");
            npcAnimator.SetLayerWeight(faceLayerIndex, 1);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the other object has an Animator component with the "NPCController" controller
        Animator npcAnimator = other.GetComponent<Animator>();
        if (npcAnimator != null /*&& npcAnimator.runtimeAnimatorController.name == "NPCController"*/)
        {
            // Reset the "Face" layer weight to 0
            Debug.Log("Not anymore");
            npcAnimator.SetLayerWeight(faceLayerIndex, 0);
        }
    }
}
