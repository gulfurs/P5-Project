using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEqualizing : MonoBehaviour
{

    public TextMeshProUGUI handText;
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.GetComponent<TextMeshPro>().text = handText.text;
    }
}
