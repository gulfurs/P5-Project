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
    private Dialogue[] currentDialogueSequence;
    private int currentDialogueIndex = 0;
    private DialogueNode currentNode; // Current node data

    private EventManager getEventManager; // Event manager
    private bool midConvo = false; // CHECK IF MID CONVO

    private bool isDialoguePlaying = false; // CHECK IF DIALOGUE IS PLAYING
    private bool signalReceived = false; // BOOL TO CHECK FOR SIGNAL

    private PlayableDirector playableDirector;

    private ClickActions getClickActions;

    private void Start() {
        getEventManager = GameObject.FindObjectOfType<EventManager>();
        if (getEventManager == null) {
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
        if (getAudiomotor == null) {
            Debug.LogError("NO AUDIOMOTOR HOMEBOY");
            return;
        }

        getFollow = getAudiomotor.GetComponent<LazyFollow>();
        getAudioSource = getAudiomotor.GetComponent<AudioSource>();
        playableDirector = GetComponent<PlayableDirector>();
        getClickActions = FindObjectOfType<ClickActions>();
    }

    public bool ChoicesActive { 
        get { 
            return getTextA?.transform.parent.gameObject.activeSelf == true || 
                   getTextB?.transform.parent.gameObject.activeSelf == true; 
        } 
    }

    // METHOD FOR STARTING DIALOGUE. OTHER CLASSES MIGHT WANT TO PUT THIS METHOD TO GOOD USE.
    public void StartDialogue() {   
        midConvo = true;

        getButtonA?.onClick.AddListener(OnChoiceA);
        getButtonB?.onClick.AddListener(OnChoiceB);
        getClickActions.dialogueMan = gameObject.GetComponent<DialogueManager>();

        currentNode = nodes[currentNodeIndex];
        PlayTimeline(currentNode.startTimeline);
        StartDialogueSequence(currentNode.startDialogue, true);

    }

        public void StartDialogueSequence(Dialogue[] dialogueSequence, bool showChoicesAfter) {
        if (isDialoguePlaying) return;
        isDialoguePlaying = true;
        currentDialogueSequence = dialogueSequence;
        currentDialogueIndex = 0;

        PlayDialogue(currentDialogueSequence[currentDialogueIndex], showChoicesAfter);
    }


        // PLAY DIALOGUE IN PROPER SHAPE AND IN THE FACE OF THE PROPER MAN.
        private void PlayDialogue(Dialogue dialogue, bool showChoicesAfter) {
            if (dialogue == null) return;

            getSubtitles.text = dialogue.dialogueText;
            getSubtitles.color = dialogue.actor.actorColor;
            getFollow.target = dialogue.actor.faceID?.transform;

            //CONSEQUENCES?
            if (!string.IsNullOrEmpty(dialogue.consequenceUID)) {
                getEventracker.Add(dialogue.consequenceUID);
            }

            // DEMAND TO KNOW IF SIGNAL IS STILL ACITVE
            StartCoroutine(WaitForSignal(showChoicesAfter));
        }

        // SIGNAL? PLAY NEXT DIALOGUE ACCORDINGLY
       private IEnumerator WaitForSignal(bool showChoicesAfter) {
    yield return new WaitUntil(() => signalReceived);
    signalReceived = false;

    currentDialogueIndex++;
    //Debug.Log($"Signal received - Current Dialogue Index: {currentDialogueIndex}, Current Node Index: {currentNodeIndex}");

    if (currentDialogueIndex < currentDialogueSequence.Length) {
        PlayDialogue(currentDialogueSequence[currentDialogueIndex], showChoicesAfter);
    } else {
        if (showChoicesAfter) {
            ShowChoices();
            isDialoguePlaying = false;
        } else {
            isDialoguePlaying = false;
        }
    }
}


    // DISPLAY OPTIONS (UI-WISE)
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

    // METHOD FOR CHOICE A AND ITS DIALOGUE. 
    public void OnChoiceA() {
        if (!ChoicesActive) return;
        HandleChoice(currentNode.playerChoiceA, currentNode.choiceATimeline);
    }   

    // METHOD FOR CHOICE B AND ITS DIALOGUE. 
    public void OnChoiceB() {
        if (!ChoicesActive) return;
        HandleChoice(currentNode.playerChoiceB, currentNode.choiceBTimeline);
    }

    // METHOD FOR CHOICE A AND B DIALOGUE
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

    // CHECK NEXT NODE
    private IEnumerator CheckNextNode() {
        //yield return new WaitForSeconds(2f);

        if (currentNode.closingDialogue) {
             PlayTimeline(currentNode.endTimeline);
            StartDialogueSequence(currentNode.endDialogue, false);
            yield return new WaitUntil(() => !isDialoguePlaying);
            EndDialogue();
            yield break;
        }

        // Move to next node if available
        if (currentNodeIndex < nodes.Length - 1) {
            currentNodeIndex++;
            StartDialogue(); // Begin next dialogue
        } else {
            EndDialogue();
        }
    }

    // END DIALOGUE
    private void EndDialogue() {
        Debug.Log("End of dialogue nodes reached.");
        getFollow.target = getEventManager.player.GetComponent<Actor>().faceID.transform;
        getButtonA?.onClick.RemoveListener(OnChoiceA);
        getButtonB?.onClick.RemoveListener(OnChoiceB);
        midConvo = false;
    }

    //METHOD FOR PLAYING DIALOGUE
    private void PlayTimeline(PlayableAsset timeline)
    {
        if (timeline != null)
        {
        playableDirector.playableAsset = timeline;
        playableDirector.Play();
        }
    }
    // SIGNAL EMITTED? SIGNAL RECEIVED NOW TRUE.
    public void OnSignalReceived() {
        signalReceived = true;
    }
}