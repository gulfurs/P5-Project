using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

//MANAGES EVENT
public class EventManager : MonoBehaviour
{
    public Transform PlayerEvent;
    public GameObject choiceButtonA;
    public GameObject choiceButtonB;
    public Button resumeButton;
    public TextMeshProUGUI mainSubtitles;
    
    public List<string> eventTrackerList = new List<string>();

    private ActorManager actorManager; //ACTORMANAGER
    public GameObject player;

    void Start()
    {
        actorManager = GameObject.FindObjectOfType<ActorManager>();

        if (actorManager != null)
        {
            player = actorManager.Player;

            if (player != null)
            {
                PlayerInteraction playerInteraction = player.GetComponent<PlayerInteraction>();

                if (playerInteraction != null)
                {
                    // NEW ACTOR STEPPING IN? NEW EVENT
                    playerInteraction.OnSelectionChanged += UpdatePlayerEvent;
                }
            }
        }
    }   

    // PREPARES NEW PLAYER EVENT
    private void UpdatePlayerEvent(Transform newSelection)
    {   
         if (newSelection == null) {
        return; 
        }
        
        //DISABLE COLLIDERS OF ALL ACTORS
        foreach (var actor in actorManager.Actors)
        {
            Collider[] colliders = actor.actorObject.GetComponents<Collider>();  

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;  // Disable each collider
            }
        }

        PlayerEvent = newSelection;

        //SET PLAYER TO POSITION OF THE SELECTION'S ROOT
        Transform playerRoot = newSelection.Find("PlayerRoot");
         if (playerRoot != null) {
        player.transform.position = playerRoot.position;
        }

        //START THE DIALOGUE OF THE SELECTION
        DialogueManager dialogueManager = newSelection.gameObject.GetComponent<DialogueManager>();
        if (dialogueManager != null)
        {
        dialogueManager.StartDialogue();
        }

        //START THE DIALOGUE OF THE SELECTION
        CustomInteraction getInteraction = newSelection.gameObject.GetComponent<CustomInteraction>();
        if (getInteraction != null)
        {
        getInteraction.InitializeInteraction();
        PlayerEvent = null;
        }

       
        //Debug.Log("PlayerEvent updated to: " + PlayerEvent);
    }
    

    public void StartConversation() {
        //DISABLE COLLIDERS OF ALL ACTORS
        foreach (var actor in actorManager.Actors)
        {
            Collider[] colliders = actor.actorObject.GetComponents<Collider>();  

            foreach (Collider collider in colliders)
            {
                collider.enabled = false;  // Disable each collider
            }
        }
    }

    public void EndConversation()
    {
    Debug.Log("EndConversation called"); // Debug log
    mainSubtitles.text = "";
    
    foreach (var actor in actorManager.Actors)
    {
        Collider[] colliders = actor.actorObject.GetComponents<Collider>();  

        foreach (Collider collider in colliders)
        {
            collider.enabled = true;  
        }
    }

    if (PlayerEvent != null)
    {
        Actor actor = PlayerEvent.gameObject.GetComponent<Actor>();
        if (actor != null)
        {
            actor.selectable = false;
        }
    
        Outline outline = PlayerEvent.gameObject.GetComponent<Outline>();
        if (outline != null) 
        {
            outline.enabled = false;
        }

        PlayerEvent = null;
    }
    /*
    FirstPersonMovement firstPersonMovement = player.GetComponent<FirstPersonMovement>();
    player.transform.position = firstPersonMovement.startingPosition;
    Debug.Log("Player moved to starting position");*/
    }
}
