using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        transform.LookAt(Camera.main.transform);
    }
}
