using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public string dialogueText;
    public Color dialogueColor = Color.white;
    public float displayDuration = 2f;
}

// CLASS FOR DIALOGUE NODES
[System.Serializable]
public class DialogueNode
{
    public bool closingDialogue;      // IS LAST DIALOGUE?
    public Dialogue[] startDialogue;    // START DIALOGUE?
    public string choiceA;            // PREVIEW OF OPTION A
    public string choiceB;            // PREVIEW OF OPTION B
    public Dialogue[] playerChoiceA;    // OPTION A DIALOGUE
    public Dialogue[] playerChoiceB;    // OPTION B DIALOGUE
    public Dialogue[] endDialogue;      // END DIALOGUE (OPTIONAL)
}

// CLASS FOR MANAGEMENT OF DIALOGUE NODES AND OVERALL DIALOGUE
public class DialogueManager : MonoBehaviour
{
    public DialogueNode[] nodes;             // LIST OF DIALOGUE NODES
    private TextMeshProUGUI getTextA, getTextB; // UI FOR CHOICES
    private Button getButtonA, getButtonB;
    private TextMeshProUGUI getSubtitles;        // UI FOR SUBTITLES

    private int currentNodeIndex = 0;        // CURRENT NODE
    private DialogueNode currentNode;        // CURRENT NODE DATA
    private bool playerMadeChoice = false;   // HAS PLAYER MADE A CHOICE

    private EventManager getEventManager; // EVENT MANAGER

    private void Start() {
        getEventManager = GameObject.FindObjectOfType<EventManager>();
        getSubtitles  = getEventManager.mainSubtitles;

        getTextA = getEventManager.choiceButtonA.GetComponentInChildren<TextMeshProUGUI>();
        getTextB = getEventManager.choiceButtonB.GetComponentInChildren<TextMeshProUGUI>();

        getButtonA = getEventManager.choiceButtonA.GetComponent<Button>();
        getButtonB = getEventManager.choiceButtonB.GetComponent<Button>();
    }

    // INIT DIALOGUE
    public void StartDialogue()
    {   
        getButtonA.onClick.AddListener(OnChoiceA);
        getButtonB.onClick.AddListener(OnChoiceB);
        currentNode = nodes[currentNodeIndex];
        StartCoroutine(PlayDialogueSequence(currentNode.startDialogue, true));
    }

    // PLAYS A SEQUENCE OF DIALOGUE ONE AFTER THE OTHER
    private IEnumerator PlayDialogueSequence(Dialogue[] dialogueSequence, bool showChoicesAfter = false)
    {
        foreach (Dialogue dialogue in dialogueSequence)
        {
            getSubtitles.text = dialogue.dialogueText;
            getSubtitles.color = dialogue.dialogueColor;
            yield return new WaitForSeconds(dialogue.displayDuration);  // WAIT BASED ON DIALOGUE DURATION
        }

        if (showChoicesAfter)
        {
        ShowChoices();
        }
    }

    // DISPLAY OPTIONS UI-WISE
    private void ShowChoices()
    {
        getTextA.text = currentNode.choiceA;
        getTextB.text = currentNode.choiceB;

        // ACTIVE OPTION UI
        getTextA.transform.parent.gameObject.SetActive(true);
        getTextB.transform.parent.gameObject.SetActive(true);
    }

    // PLAYER SELECTS OPTION A
    public void OnChoiceA()
    {
        playerMadeChoice = true;
        getTextA.transform.parent.gameObject.SetActive(false); 
        getTextB.transform.parent.gameObject.SetActive(false);
        StartCoroutine(HandlePlayerChoice(currentNode.playerChoiceA));
    }

    // PLAYER SELECTS OPTION B
    public void OnChoiceB()
    {
        playerMadeChoice = true;
        getTextA.transform.parent.gameObject.SetActive(false);  
        getTextB.transform.parent.gameObject.SetActive(false);
        StartCoroutine(HandlePlayerChoice(currentNode.playerChoiceB));
    }

    // HANDLE PLAYER'S CHOICE DIALOGUE
    private IEnumerator HandlePlayerChoice(Dialogue[] choiceDialogue)
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
            StartCoroutine(PlayDialogueSequence(currentNode.endDialogue));
            getButtonA.onClick.RemoveListener(OnChoiceA);
            getButtonB.onClick.RemoveListener(OnChoiceB);
            getEventManager.EndConversation();
            yield break;  // CONVERSATION ENDS
        }

        // MOVE TO NEXT NODE
        if (currentNodeIndex < nodes.Length - 1)
        {
            currentNodeIndex++;
            StartDialogue();  // START NEXT DIALOGUE
        }
        else
        {
            Debug.Log("End of dialogue nodes reached.");
        }
    }
}
