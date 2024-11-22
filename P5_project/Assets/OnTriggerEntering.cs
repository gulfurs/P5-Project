using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEntering : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;

    
    [SerializeField]
    private bool enableOnEnter = true;

    private void OnTriggerEnter(Collider other)
    {
        if (targetObject != null)
        {
            targetObject.SetActive(enableOnEnter);
        }
    }
}
