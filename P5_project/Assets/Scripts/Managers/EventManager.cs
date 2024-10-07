using UnityEngine;

//MANAGES EVENT
public class EventManager : MonoBehaviour
{
    public Transform PlayerEvent;

    private ActorManager actorManager; //ACTORMANAGER
    GameObject player;

    void Start()
    {
        actorManager = GameObject.FindObjectOfType<ActorManager>();

        if (actorManager != null)
        {
            // Get the Player GameObject from ActorManager
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

       
        //Debug.Log("PlayerEvent updated to: " + PlayerEvent);
    }
    
    //HANDLES POST DIALOGUE
    public void EndConversation() {

        //ENABLES COLLIDERS OF ALL ACTORS
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

    //MAKE IT SO YOU CAN'T START THE SAME DIALOGUE
    Actor actor = PlayerEvent.gameObject.GetComponent<Actor>();
    if (actor != null)
    {
        actor.selectable = false;
    }
    
    //DISABLES OUTLINE
    Outline outline = PlayerEvent.gameObject.GetComponent<Outline>();
        if (outline != null) {
        
        outline.enabled = false;
        }
    
    PlayerEvent = null; // CLEAR PLAYEREVENT REFERENCE
    }

    // RETURNS PLAYER TO STARTING POSITION
    if (player != null)
    {
    FirstPersonMovement firstPersonMovement = player.GetComponent<FirstPersonMovement>();
    if (firstPersonMovement != null) {
        player.transform.position = firstPersonMovement.startingPosition;
            }
        }

    }
}
