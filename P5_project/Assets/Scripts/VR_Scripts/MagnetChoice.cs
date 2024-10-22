using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetChoice : MonoBehaviour
{
    public Transform hand;
    public float attractionRange = 0.5f;
    public float attractionForce = 5f;

    public Material normalMaterial;
    public Material highlightMaterial;

    private Renderer cubeRenderer;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        cubeRenderer.material = normalMaterial;
    }

    void Update()
    {
        float distanceToHand = Vector3.Distance(transform.position, hand.position);
        if (distanceToHand < attractionRange)
        {
            hand.position = Vector3.Lerp(hand.position, transform.position, attractionForce * Time.deltaTime);
        }
    }

    public void Highlight(bool isHighlighted)
    {
        cubeRenderer.material = isHighlighted ? highlightMaterial : normalMaterial;
    }
}
