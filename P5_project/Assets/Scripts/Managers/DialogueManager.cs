using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;

[System.Serializable]
public class Dialogue
{
    public string dialogueText;
    public float displayDuration = 2f;
    public Actor actor; 
    public string consequenceUID;
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

    public AudioClip startQuip;         // Sound for start dialogue
    public AudioClip choiceAQuip;        // Sound for choice A
    public AudioClip choiceBQuip;        // Sound for choice B
    public AudioClip endQuip;    // Sound for end dialogue
}

// CLASS FOR MANAGEMENT OF DIALOGUE NODES AND OVERALL DIALOGUE
public class DialogueManager : MonoBehaviour
{
    public DialogueNode[] nodes;             // LIST OF DIALOGUE NODES
    private TextMeshProUGUI getTextA, getTextB; // UI FOR CHOICES
    private Button getButtonA, getButtonB;
    private TextMeshProUGUI getSubtitles;        // UI FOR SUBTITLES
    private List<string> getEventracker;

    public GameObject getAudiomotor;
    private AudioSource getAudioSource;
    private LazyFollow getFollow;

    private int currentNodeIndex = 0;        // CURRENT NODE
    private DialogueNode currentNode;        // CURRENT NODE DATA

    private EventManager getEventManager; // EVENT MANAGER

    private void Start() {
        getEventManager = GameObject.FindObjectOfType<EventManager>();
        getSubtitles  = getEventManager.mainSubtitles;

        getTextA = getEventManager.choiceButtonA.GetComponentInChildren<TextMeshProUGUI>();
        getTextB = getEventManager.choiceButtonB.GetComponentInChildren<TextMeshProUGUI>();

        getButtonA = getEventManager.choiceButtonA.GetComponent<Button>();
        getButtonB = getEventManager.choiceButtonB.GetComponent<Button>();
        getEventracker = getEventManager.eventTrackerList;

        getAudiomotor = FindObjectOfType<AudioMotor>().gameObject;
        getFollow = getAudiomotor.GetComponent<LazyFollow>();
        getAudioSource = getAudiomotor.GetComponent<AudioSource>();
    }

    // INIT DIALOGUE
    public void StartDialogue()
    {   
        getButtonA.onClick.AddListener(OnChoiceA);
        getButtonB.onClick.AddListener(OnChoiceB);
        currentNode = nodes[currentNodeIndex];

        if (currentNode.startQuip != null)
        {
            getAudioSource.PlayOneShot(currentNode.startQuip);
        }

        StartCoroutine(PlayDialogueSequence(currentNode.startDialogue, true));
    }

    // PLAYS A SEQUENCE OF DIALOGUE ONE AFTER THE OTHER
    private IEnumerator PlayDialogueSequence(Dialogue[] dialogueSequence, bool showChoicesAfter = false)
    {
        
        foreach (Dialogue dialogue in dialogueSequence)
        {
            getSubtitles.text = dialogue.dialogueText;
            getSubtitles.color = dialogue.actor.actorColor;
            getFollow.target = dialogue.actor.faceID.transform;

            if (!string.IsNullOrEmpty(dialogue.consequenceUID))
            {
            getEventracker.Add(dialogue.consequenceUID);
            }


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
        getTextA.transform.parent.gameObject.SetActive(false); 
        getTextB.transform.parent.gameObject.SetActive(false);

        currentNode = nodes[currentNodeIndex];
        if (currentNode.choiceAQuip != null)
        {
            getAudioSource.PlayOneShot(currentNode.choiceAQuip);
        }
        StartCoroutine(HandlePlayerChoice(currentNode.playerChoiceA));
    }   

    // PLAYER SELECTS OPTION B
    public void OnChoiceB()
    {
        getTextA.transform.parent.gameObject.SetActive(false);  
        getTextB.transform.parent.gameObject.SetActive(false);
        
        currentNode = nodes[currentNodeIndex];
        if (currentNode.choiceBQuip != null)
        {
            getAudioSource.PlayOneShot(currentNode.choiceBQuip);
        }

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
            if (currentNode.endQuip != null)
            {
                getAudioSource.PlayOneShot(currentNode.endQuip);
            }

            StartCoroutine(PlayDialogueSequence(currentNode.endDialogue));
            getButtonA.onClick.RemoveListener(OnChoiceA);
            getButtonB.onClick.RemoveListener(OnChoiceB);
            
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
            getEventManager.EndConversation();
            Debug.Log("End of dialogue nodes reached.");
        }
    }
}
