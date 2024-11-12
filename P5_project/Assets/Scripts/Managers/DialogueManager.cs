using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.UI;

[System.Serializable]
public class Dialogue
{
    public string dialogueText;
    public Actor actor;
    public string consequenceUID;
    public AudioClip voiceLine; // Audio clip for voice line
}

[System.Serializable]
public class DialogueNode
{
    public bool closingDialogue;
    public bool options = true;
    public Dialogue[] startDialogue;
    public string choiceA;
    public string choiceB;
    public Dialogue[] playerChoiceA;
    public Dialogue[] playerChoiceB;
    public Dialogue[] endDialogue;

    public PlayableAsset startTimeline;
    public PlayableAsset choiceATimeline;
    public PlayableAsset choiceBTimeline;
    public PlayableAsset endTimeline;
}

public class DialogueManager : MonoBehaviour
{
    public DialogueNode[] nodes;
    private TextMeshProUGUI getTextA, getTextB;
    private Button getButtonA, getButtonB;
    private TextMeshProUGUI getSubtitles;
    private List<string> getEventracker;
    private GameObject getAudiomotor;
    private AudioSource getAudioSource;
    private LazyFollow getFollow;

    private int currentNodeIndex = 0;
    private Dialogue[] currentDialogueSequence;
    private int currentDialogueIndex = 0;      // Tracks audio line
    private DialogueNode currentNode;

    private EventManager getEventManager;
    private bool midConvo = false;
    private bool isDialoguePlaying = false;
    private bool waitingForPlayerInput = false;

    private PlayableDirector playableDirector;
    private ClickActions getClickActions;
    public List<Actor> additionalActorsToRemove;

    [Header("Input Settings")]
    public InputActionProperty continueDialogAction;

    private void Start()
    {
        continueDialogAction.action.Enable();

        getEventManager = GameObject.FindObjectOfType<EventManager>();
        if (getEventManager == null)
        {
            Debug.LogError("NO EVENTMANAGER HOMEBOY");
            return;
        }

        getSubtitles = getEventManager.mainSubtitles;
        getTextA = getEventManager.choiceButtonA?.GetComponentInChildren<TextMeshProUGUI>();
        getTextB = getEventManager.choiceButtonB?.GetComponentInChildren<TextMeshProUGUI>();
        getButtonA = getEventManager.choiceButtonA?.GetComponent<Button>();
        getButtonB = getEventManager.choiceButtonB?.GetComponent<Button>();
        getEventracker = getEventManager.eventTrackerList ?? new List<string>();

        getAudiomotor = FindObjectOfType<AudioMotor>()?.gameObject;
        if (getAudiomotor == null)
        {
            Debug.LogError("NO AUDIOMOTOR HOMEBOY");
            return;
        }

        getFollow = getAudiomotor.GetComponent<LazyFollow>();
        getAudioSource = getAudiomotor.GetComponent<AudioSource>();
        playableDirector = GetComponent<PlayableDirector>();
        getClickActions = FindObjectOfType<ClickActions>();
    }

    public bool ChoicesActive
    {
        get
        {
            return getTextA?.transform.parent.gameObject.activeSelf == true ||
                   getTextB?.transform.parent.gameObject.activeSelf == true;
        }
    }

    public void StartDialogue()
    {
        getEventManager.StartConversation();
        getEventManager.PlayerEvent = gameObject.transform;
        midConvo = true;
        AnimNext();
        getButtonA?.onClick.AddListener(OnChoiceA);
        getButtonB?.onClick.AddListener(OnChoiceB);
        getClickActions.dialogueMan = gameObject.GetComponent<DialogueManager>();
        currentNode = nodes[currentNodeIndex];
        StartDialogueSequence(currentNode.startDialogue, currentNode.options, currentNode.startTimeline);
    }

    public void StartDialogueSequence(Dialogue[] dialogueSequence, bool showChoicesAfter, PlayableAsset choiceTimeline)
    {
        //if (isDialoguePlaying) return;
        isDialoguePlaying = true;
        currentDialogueSequence = dialogueSequence;
        currentDialogueIndex = 0;
        PlayTimeline(choiceTimeline);

        // Display the initial line and start the first voice line without needing player input
        PlayNextLine();
    }

    public void PlayNextLine()
    {
        UpdateTextDisplay();
    }

    private void UpdateTextDisplay()
    {
            Dialogue dialogue = currentDialogueSequence[currentDialogueIndex];

            // Display dialogue text for the previous line
            string actorName = $"<color=#{ColorUtility.ToHtmlStringRGB(dialogue.actor.actorColor)}>{dialogue.actor.actorName}:</color>";
            getSubtitles.text = actorName + " " + dialogue.dialogueText;
            getFollow.target = dialogue.actor.faceID?.transform;
    }

    private void PlayVoiceLine()
    {
       /* if (currentDialogueIndex >= currentDialogueSequence.Length) return;

        Dialogue dialogue = currentDialogueSequence[currentDialogueIndex];

        // Play the voice line if available
        if (dialogue.voiceLine != null)
        {
            getAudioSource.clip = dialogue.voiceLine;
            getAudioSource.Play();
        }*/
    }

    public void OnSignalReceived()
    {
        // Prepare to show the previous line's text and play the current voice line
        waitingForPlayerInput = true;
        playableDirector.Pause();
        getEventManager?.nextButton?.SetActive(true);
        // Advance to the next line for audio
        if (currentDialogueIndex < currentDialogueSequence.Length-1)
        {
            currentDialogueIndex++;
        }
        else
        {
            StartCoroutine(FinishDialogueSequence());
        }
    }

        private IEnumerator FinishDialogueSequence() {   
        isDialoguePlaying = false;

        // Check if the current node has options
        if (currentNode.options)
        {
            ShowChoices();  // Display choice options
            currentNode.options = false;  // Set options to false after displaying
        }
        else
        {
        // If there is an end dialogue sequence, play it
        if (currentNode.endTimeline != null)
        {
            // Start the end dialogue sequence and wait until it completes
            StartDialogueSequence(currentNode.endDialogue, !currentNode.options, currentNode.endTimeline);
            yield return new WaitUntil(() => !isDialoguePlaying);
        }
        
        // Move to the next node in the sequence
        yield return StartCoroutine(CheckNextNode());
        }
    }


    private void ShowChoices()
    {
        Debug.Log("End of dialogue");

        if (getTextA != null && getTextB != null)
        {
            getTextA.text = currentNode.choiceA;
            getTextB.text = currentNode.choiceB;
            getFollow.target = getEventManager.player.transform;

            getTextA.transform.parent.gameObject.SetActive(true);
            getTextB.transform.parent.gameObject.SetActive(true);

            getEventManager.nextButton.SetActive(false);
        }
    }

    public void OnChoiceA()
    {
        if (!ChoicesActive) return;
        HandleChoice(currentNode.playerChoiceA, currentNode.choiceATimeline);
       //playableDirector.Play();
    }

    public void OnChoiceB()
    {
        if (!ChoicesActive) return;
        HandleChoice(currentNode.playerChoiceB, currentNode.choiceBTimeline);
        //playableDirector.Play();
    }

    private void HandleChoice(Dialogue[] choiceDialogue, PlayableAsset choiceTimeline)
    {
        getTextA?.transform.parent.gameObject.SetActive(false);
        getTextB?.transform.parent.gameObject.SetActive(false);

        
        StartCoroutine(HandlePlayerChoice(choiceDialogue, choiceTimeline));
    }

    private IEnumerator HandlePlayerChoice(Dialogue[] choiceDialogue, PlayableAsset choiceTimeline)
    {
        if (choiceDialogue != null && choiceDialogue.Length > 0)
        {
            StartDialogueSequence(choiceDialogue, false, choiceTimeline);
            yield return new WaitUntil(() => !isDialoguePlaying);
        }
        yield return StartCoroutine(CheckNextNode());
    }

    private IEnumerator CheckNextNode()
    {
        if (currentNode.closingDialogue)
        {
            yield return new WaitUntil(() => !isDialoguePlaying);
            EndDialogue();
            yield break;
        }

        if (currentNodeIndex < nodes.Length - 1)
        {
            currentNodeIndex++;
            StartDialogue();
        }
        else
        {
            EndDialogue();
            yield break;
        }
    }

    private void EndDialogue()
    {
        if (midConvo) {
        Debug.Log("End of dialogue nodes reached.");
        HandleChoice(currentNode.endDialogue, currentNode.endTimeline);
        AnimNext();
        getFollow.target = getEventManager.player.GetComponent<Actor>().faceID.transform;
        getButtonA?.onClick.RemoveListener(OnChoiceA);
        getButtonB?.onClick.RemoveListener(OnChoiceB);
        getEventManager.actorManager.Actors.Remove(gameObject.GetComponent<Actor>());
        foreach (var actor in additionalActorsToRemove)
        {
            getEventManager.actorManager.Actors.Remove(actor);
        }
        getEventManager.EndConversation();
        midConvo = false;
        }
    }

    private void AnimNext()
    {
        Animator[] childAnimators = GetComponentsInChildren<Animator>();
        foreach (Animator animator in childAnimators)
        {
            animator.SetTrigger("NextConvo");
        }
    }

    private void PlayTimeline(PlayableAsset timeline)
    {
        if (timeline != null)
        {
            playableDirector.playableAsset = timeline;
            playableDirector.time = 0;
            playableDirector.Play();
        }
    }

    private void Update()
    {
        // Check if waiting for player input and if the button was pressed
        /*if (waitingForPlayerInput && getClickActions.PrimaryLeft.WasPressedThisFrame())
        {
            SimulatePress();
        }*/
    }

    public void SimulatePress(){
        if(!ChoicesActive && waitingForPlayerInput){
            waitingForPlayerInput = false; // Stop waiting for input
            PlayNextLine();
            playableDirector.Play(); // Resume the Timeline
            getEventManager?.nextButton?.SetActive(false);
        }
    }
}
