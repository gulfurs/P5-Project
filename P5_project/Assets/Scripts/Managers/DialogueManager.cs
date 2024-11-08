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
    private int currentDialogueIndex = 0;
    private DialogueNode currentNode;

    private EventManager getEventManager;
    private bool midConvo = false;
    private bool isDialoguePlaying = false;
    private bool signalReceived = false;
    private bool waitingForPlayerInput = false;

    private PlayableDirector playableDirector;
    private ClickActions getClickActions;
    public List<Actor> additionalActorsToRemove;

    // New field to configure input in Inspector
    [Header("Input Settings")]
    public InputActionProperty continueDialogAction;

    private void Start()
    {
        // Ensure input action is enabled
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
        midConvo = true;
        AnimNext();
        getButtonA?.onClick.AddListener(OnChoiceA);
        getButtonB?.onClick.AddListener(OnChoiceB);
        getClickActions.dialogueMan = gameObject.GetComponent<DialogueManager>();
        currentNode = nodes[currentNodeIndex];
        PlayTimeline(currentNode.startTimeline);
        StartDialogueSequence(currentNode.startDialogue, currentNode.options);
        getEventManager.StartConversation();
        getEventManager.PlayerEvent = gameObject.transform;
    }

    public void StartDialogueSequence(Dialogue[] dialogueSequence, bool showChoicesAfter)
    {
        if (isDialoguePlaying) return;
        isDialoguePlaying = true;
        currentDialogueSequence = dialogueSequence;
        currentDialogueIndex = 0;
        PlayDialogue(currentDialogueSequence[currentDialogueIndex], showChoicesAfter);
    }

    private void PlayDialogue(Dialogue dialogue, bool showChoicesAfter)
    {
        if (dialogue == null) return;

        string actorName = $"<color=#{ColorUtility.ToHtmlStringRGB(dialogue.actor.actorColor)}>{dialogue.actor.actorName}:</color>";
        getSubtitles.text = actorName + " " + dialogue.dialogueText;
        getFollow.target = dialogue.actor.faceID?.transform;

        if (!string.IsNullOrEmpty(dialogue.consequenceUID))
        {
            getEventracker.Add(dialogue.consequenceUID);
        }

        StartCoroutine(WaitForSignal(showChoicesAfter));
    }

    private IEnumerator WaitForSignal(bool showChoicesAfter)
    {
        yield return new WaitUntil(() => signalReceived);
        signalReceived = false;

        currentDialogueIndex++;

        if (currentDialogueIndex < currentDialogueSequence.Length)
        {
            PlayDialogue(currentDialogueSequence[currentDialogueIndex], showChoicesAfter);
        }
        else
        {
            if (showChoicesAfter)
            {
                ShowChoices();
                isDialoguePlaying = false;
            }
            else
            {
                HandleChoice(currentNode.endDialogue, currentNode.endTimeline);
                isDialoguePlaying = false;
            }
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
        }
    }

    public void OnChoiceA()
    {
        if (!ChoicesActive) return;
        HandleChoice(currentNode.playerChoiceA, currentNode.choiceATimeline);
    }

    public void OnChoiceB()
    {
        if (!ChoicesActive) return;
        HandleChoice(currentNode.playerChoiceB, currentNode.choiceBTimeline);
    }

    private void HandleChoice(Dialogue[] choiceDialogue, PlayableAsset choiceTimeline)
    {
        getTextA?.transform.parent.gameObject.SetActive(false);
        getTextB?.transform.parent.gameObject.SetActive(false);

        PlayTimeline(choiceTimeline);
        StartCoroutine(HandlePlayerChoice(choiceDialogue));
    }

    private IEnumerator HandlePlayerChoice(Dialogue[] choiceDialogue)
    {
        if (choiceDialogue != null && choiceDialogue.Length > 0)
        {
            StartDialogueSequence(choiceDialogue, false);
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
        }
    }

    private void EndDialogue()
    {
        Debug.Log("End of dialogue nodes reached.");
        AnimNext();
        getFollow.target = getEventManager.player.GetComponent<Actor>().faceID.transform;
        getButtonA?.onClick.RemoveListener(OnChoiceA);
        getButtonB?.onClick.RemoveListener(OnChoiceB);
        midConvo = false;
        getEventManager.actorManager.Actors.Remove(gameObject.GetComponent<Actor>());
        foreach (var actor in additionalActorsToRemove)
        {
            getEventManager.actorManager.Actors.Remove(actor);
        }
        getEventManager.EndConversation();
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
            playableDirector.Play();
        }
    }

    public void OnSignalReceived()
    {
        playableDirector.Pause();
        signalReceived = true;
        waitingForPlayerInput = true;
    }

    private void Update()
    {
        if (waitingForPlayerInput && continueDialogAction.action.WasPressedThisFrame())
        {
            waitingForPlayerInput = false;
            playableDirector.Play();
            signalReceived = false;
        }
    }
}
