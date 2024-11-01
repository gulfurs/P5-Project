using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEqualizing : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI handText;
    //[SerializeField] private TextMeshProUGUI subtitleText;
    //public TextMeshProUGUI handText;
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.GetComponent<TextMeshPro>().text = handText.text;
        //gameObject.GetComponent<TextMeshPro>().text = subtitleText.text;
    }
}
