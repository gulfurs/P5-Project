using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimatorProfile : MonoBehaviour
{
    private Animator animator;
     private int faceLayerIndex = 1;
     private int blinkLayerIndex = 2;

    [Range(0, 1)]
    public float faceWeight = 0.5f;
    [Range(0, 1)]
    public float blinkWeight = 0.5f; 

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //animator.SetLayerWeight(faceLayerIndex, faceWeight);
        animator.SetLayerWeight(blinkLayerIndex, blinkWeight);
    }
}
