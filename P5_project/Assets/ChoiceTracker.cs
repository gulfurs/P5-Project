using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceTracker : MonoBehaviour
{
    private static ChoiceTracker instance; 
    private List<string> playerChoices = new List<string>();

    //SINGLETON DONTDESTROY
     private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddChoice(string choice)
    {
        playerChoices.Add(choice);
    }

    public bool HasChoice(string choice)
    {
    return playerChoices.Contains(choice);
    }
}
