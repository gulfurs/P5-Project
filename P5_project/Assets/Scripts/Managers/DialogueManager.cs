using System.Collections;
using UnityEngine;
using TMPro;

//CLASS FOR DIALOGUE NODES
[System.Serializable]
public class DialogueNode
{
    public bool closingDialogue;      // IS LAST DIALOGUE?
    public string[] startDialogue;    // START DIALOGUE?
    public string choiceA;            // PREVIEW OF OPTION A
    public string choiceB;            // PREVIEW OF OPTION B
    public string[] playerChoiceA;    // OPTION A DIALOGUE
    public string[] playerChoiceB;    // OPTION B DIALOGUE
    public string[] endDialogue;      // END DIALOGUE (OPTIONAL)

    // Process dialogue for coloring before sending to DialogueManager
    public string[] GetProcessedStartDialogue()
    {
        return ProcessDialogueArray(startDialogue);
    }

    public string[] GetProcessedPlayerChoiceA()
    {
        return ProcessDialogueArray(playerChoiceA);
    }

    public string[] GetProcessedPlayerChoiceB()
    {
        return ProcessDialogueArray(playerChoiceB);
    }

    public string[] GetProcessedEndDialogue()
    {
        return ProcessDialogueArray(endDialogue);
    }

    // Process an array of dialogue strings for coloring
    private string[] ProcessDialogueArray(string[] dialogueArray)
    {
        string[] processedDialogue = new string[dialogueArray.Length];

        for (int i = 0; i < dialogueArray.Length; i++)
        {
            processedDialogue[i] = ProcessText(dialogueArray[i]);
        }

        return processedDialogue;
    }

    // Private method to process and color the text
    private string ProcessText(string input)
    {
        // Process each marker and replace it with the appropriate color tag
        if (input.StartsWith("[P]"))
        {
            input = input.Replace("[P]", "");
            return $"<color=#FFC0CB>{input}</color>";  // Pink
        }
        else if (input.StartsWith("[R]"))
        {
            input = input.Replace("[R]", "");
            return $"<color=#DC143C>{input}</color>";  // Crimson Red
        }
        else if (input.StartsWith("[B]"))
        {
            input = input.Replace("[B]", "");
            return $"<color=#87CEEB>{input}</color>";  // Sky Blue
        }
        else if (input.StartsWith("[G]"))
        {
            input = input.Replace("[G]", "");
            return $"<color=#32CD32>{input}</color>";  // Limegreen
        }
        
        // Return the text as is if no special marker is found
        return input;
    }
}

//CLASS FOR MANAGEMENT OF DIALOGUENODES AND OVERALL DIALOGUE
public class DialogueManager : MonoBehaviour
{
    public DialogueNode[] nodes;             // LIST OF DIALOGUE NODES
    public TextMeshProUGUI optionA, optionB; // UI FOR CHOICES
    public TextMeshProUGUI subtitles;        // UI FOR SUBTITLES

    private int currentNodeIndex = 0;        // CURRENT NODE
    private DialogueNode currentNode;        // CURRENT NODE DATA
    private bool playerMadeChoice = false;   // HAVE PLAYER MADE A CHOICE

    private EventManager d_eventManager; //EVENT MANAGER
    
    private void Start() {
    d_eventManager = GameObject.FindObjectOfType<EventManager>();
    }
    
    // INIT DIALOGUE
    public void StartDialogue()
    {
        currentNode = nodes[currentNodeIndex];
        string[] processedDialogue = currentNode.GetProcessedStartDialogue();
        StartCoroutine(PlayDialogueSequence(processedDialogue));
    }

    // PLAYS A SEQUENCE OF DIALOGUE ONE AFTER THE OTHER, AFTER THE OTHER ETC.
    private IEnumerator PlayDialogueSequence(string[] dialogueSequence)
    {
        foreach (string dialogue in dialogueSequence)
        {
            subtitles.text = dialogue;
            yield return new WaitForSeconds(2f);  // WAIT TIME BETWEEN EACH DIALOGUE
        }

        // SHOW CHOICES WHEN DIALOGUE IS DONE
        ShowChoices();
    }

    // DISPLAY OPTIONS UI-WISE
    private void ShowChoices()
    {
        optionA.text = currentNode.choiceA;
        optionB.text = currentNode.choiceB;

        //ACTIVE OPTION UI
        optionA.gameObject.SetActive(true);  
        optionB.gameObject.SetActive(true);  
    }

    // PLAYER SELECTS OPTION A
    public void OnChoiceA()
{   
    playerMadeChoice = true;
    optionA.gameObject.SetActive(false);
    optionB.gameObject.SetActive(false);
    string[] processedChoiceA = currentNode.GetProcessedPlayerChoiceA();
    
    // PLAYS CHOICE DIALOGUE
    StartCoroutine(HandlePlayerChoice(processedChoiceA));
    }

    // PLAYER SELECTS OPTION B
    public void OnChoiceB()
    {
    playerMadeChoice = true;
    optionA.gameObject.SetActive(false);
    optionB.gameObject.SetActive(false);
    string[] processedChoiceB = currentNode.GetProcessedPlayerChoiceB();

    // PLAYS CHOICE DIALOGUE
    StartCoroutine(HandlePlayerChoice(processedChoiceB));
    }

    // CHECK NEXT DIALOGUE AFTER DEALING WTIH CURRENT DIALOGUE
    private IEnumerator HandlePlayerChoice(string[] choiceDialogue)
    {
    yield return StartCoroutine(PlayDialogueSequence(choiceDialogue));
    yield return StartCoroutine(CheckNextNode()); 
    }


    // CHECK NEXT NODE IN SESSION (OR FINISH THE SESSION)
    private IEnumerator CheckNextNode()
    {
        yield return new WaitForSeconds(2f);  // GIVE TIME FOR CHOICES TO FINISH

        if (currentNode.closingDialogue)
        {
            string[] processedEndDialogue = currentNode.GetProcessedEndDialogue();
            StartCoroutine(PlayDialogueSequence(processedEndDialogue));
            d_eventManager.EndConversation();
            yield break;  // CONVERSATION ENDS
        }

        // MOVES TO NEXT NODE
        if (currentNodeIndex < nodes.Length - 1)
        {
            currentNodeIndex++;
            StartDialogue();  // STARTS NEXT DIALOGUE
        }
        else
        {
            Debug.Log("End of dialogue nodes reached.");
        }
    }
}
