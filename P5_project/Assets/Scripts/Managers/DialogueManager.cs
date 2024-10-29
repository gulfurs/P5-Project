using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class Dialogue
{
    public string dialogueText;
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

    public PlayableAsset startTimeline; // Timeline to run for start dialogue
    public PlayableAsset choiceATimeline; // Timeline for choice A
    public PlayableAsset choiceBTimeline; // Timeline for choice B
    public PlayableAsset endTimeline; // Timeline for end dialogue
}

// CLASS FOR MANAGEMENT OF DIALOGUE NODES AND OVERALL DIALOGUE
public class DialogueManager : MonoBehaviour
{
    public DialogueNode[] nodes; // List of dialogue nodes
    private TextMeshProUGUI getTextA, getTextB; // UI for choices
    private Button getButtonA, getButtonB;
    private TextMeshProUGUI getSubtitles; // UI for subtitles
    private List<string> getEventracker;

    public GameObject getAudiomotor;
    private AudioSource getAudioSource;
    private LazyFollow getFollow;

    private int currentNodeIndex = 0; // Current node
    private DialogueNode currentNode; // Current node data

    private EventManager getEventManager; // Event manager
    private bool isDialogueRunning = false; // Track if dialogue is running

    private bool isDialoguePlaying = false; // Track if dialogue is currently playing
    private int currentDialogueIndex = 0; 
    private bool signalReceived = false; // Flag to track if signal has been received

    private PlayableDirector playableDirector;

    private void Start() {
        // Initialize references and check for nulls
        getEventManager = GameObject.FindObjectOfType<EventManager>();
        if (getEventManager == null) {
            Debug.LogError("EventManager not found.");
            return;
        }

        getSubtitles = getEventManager.mainSubtitles;
        getTextA = getEventManager.choiceButtonA?.GetComponentInChildren<TextMeshProUGUI>();
        getTextB = getEventManager.choiceButtonB?.GetComponentInChildren<TextMeshProUGUI>();
        getButtonA = getEventManager.choiceButtonA?.GetComponent<Button>();
        getButtonB = getEventManager.choiceButtonB?.GetComponent<Button>();
        getEventracker = getEventManager.eventTrackerList ?? new List<string>();

        getAudiomotor = FindObjectOfType<AudioMotor>()?.gameObject;
        if (getAudiomotor == null) {
            Debug.LogError("AudioMotor not found.");
            return;
        }

        getFollow = getAudiomotor.GetComponent<LazyFollow>();
        getAudioSource = getAudiomotor.GetComponent<AudioSource>();
        playableDirector = GetComponent<PlayableDirector>();
    }

    // Start the dialogue
    public void StartDialogue() {   
        if (isDialogueRunning) return; // Prevent overlapping dialogue starts
        isDialogueRunning = true;

        getButtonA?.onClick.AddListener(OnChoiceA);
        getButtonB?.onClick.AddListener(OnChoiceB);

        currentNode = nodes[currentNodeIndex];
        PlayTimeline(currentNode.startTimeline);
        StartDialogueSequence(currentNode.startDialogue, true);
    }

    // Start the dialogue sequence
    public void StartDialogueSequence(Dialogue[] dialogueSequence, bool showChoicesAfter) {
        if (isDialoguePlaying) return; // Prevent starting multiple sequences
        isDialoguePlaying = true; // Set to true to indicate dialogue is playing
        currentDialogueIndex = 0; // Reset dialogue index

        // Start playing the first dialogue immediately
        PlayDialogue(dialogueSequence[currentDialogueIndex], showChoicesAfter);
    }

    // Play an individual dialogue and wait for a signal to proceed
    private void PlayDialogue(Dialogue dialogue, bool showChoicesAfter) {
        if (dialogue == null) return; // Skip if dialogue is null

        getSubtitles.text = dialogue.dialogueText;
        getSubtitles.color = dialogue.actor.actorColor;
        getFollow.target = dialogue.actor.faceID?.transform;

        if (!string.IsNullOrEmpty(dialogue.consequenceUID)) {
            getEventracker.Add(dialogue.consequenceUID);
        }

        // Wait for the signal emitted from the Timeline
        StartCoroutine(WaitForSignal(showChoicesAfter));
    }

    // Coroutine that waits for the next signal
    private IEnumerator WaitForSignal(bool showChoicesAfter) {
        // Wait until a signal is received
        yield return new WaitUntil(() => signalReceived);

        signalReceived = false;
        
        // After receiving a signal, increment the dialogue index and check if we need to show choices
        currentDialogueIndex++;
        if (currentDialogueIndex < currentNode.startDialogue.Length) {
            PlayDialogue(currentNode.startDialogue[currentDialogueIndex], showChoicesAfter); // Play next dialogue
        } else {
            if (showChoicesAfter) {
            ShowChoices(); // Show choices if required
            isDialoguePlaying = false; // Reset the flag once dialogue sequence is complete
            }
        }
    }

    // Display options UI-wise
    private void ShowChoices() {
        Debug.Log("End of dialogue");
        
        if (getTextA != null && getTextB != null) {
            getTextA.text = currentNode.choiceA;
            getTextB.text = currentNode.choiceB;
            getFollow.target = getEventManager.player.transform;

            getTextA.transform.parent.gameObject.SetActive(true);
            getTextB.transform.parent.gameObject.SetActive(true);
        }
    }

    // Player selects option A
    public void OnChoiceA() {
        HandleChoice(currentNode.playerChoiceA, currentNode.choiceATimeline);
    }   

    // Player selects option B
    public void OnChoiceB() {
        HandleChoice(currentNode.playerChoiceB, currentNode.choiceBTimeline);
    }

    // Handles player choice (A or B) to avoid duplicate code
    private void HandleChoice(Dialogue[] choiceDialogue, PlayableAsset choiceTimeline) {
        getTextA.transform.parent.gameObject.SetActive(false);
        getTextB.transform.parent.gameObject.SetActive(false);

        PlayTimeline(choiceTimeline);
        StartCoroutine(HandlePlayerChoice(choiceDialogue));
    }

    // Handle player's choice dialogue
    private IEnumerator HandlePlayerChoice(Dialogue[] choiceDialogue) {
        if (choiceDialogue != null && choiceDialogue.Length > 0) {
            // Start the dialogue sequence for the player's choice
            StartDialogueSequence(choiceDialogue, false);
            yield return new WaitUntil(() => !isDialoguePlaying); // Wait until dialogue sequence is complete
        }
        yield return StartCoroutine(CheckNextNode());
    }

    // Check next node in session (or finish the session)
    private IEnumerator CheckNextNode() {
        yield return new WaitForSeconds(2f); // Pause before moving to next node

        if (currentNode.closingDialogue) {
             PlayTimeline(currentNode.endTimeline);
            StartDialogueSequence(currentNode.endDialogue, false);
            yield return new WaitUntil(() => !isDialoguePlaying); // Wait until dialogue sequence is complete
            EndDialogue();
            yield break;
        }

        // Move to next node if available
        if (currentNodeIndex < nodes.Length - 1) {
            currentNodeIndex++;
            StartDialogue(); // Begin next dialogue
        } else {
            Debug.Log("End of dialogue nodes reached.");
            EndDialogue();
        }
    }

    // Cleanup after dialogue session ends
    private void EndDialogue() {
        getButtonA?.onClick.RemoveListener(OnChoiceA);
        getButtonB?.onClick.RemoveListener(OnChoiceB);
        isDialogueRunning = false;
    }

    private void PlayTimeline(PlayableAsset timeline)
{
    if (timeline != null)
    {
        playableDirector.playableAsset = timeline;
        playableDirector.Play();
    }
}
    // Method to be called when the signal is emitted
    public void OnSignalReceived() {
        signalReceived = true; // Set this to true to trigger the next dialogue
        Debug.Log("SIGNAL RECEIVED");
    }
}