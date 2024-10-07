using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//HOME TO INTERACTABLE OBJECTS
public class Actor : MonoBehaviour
{
    ActorManager actorManager;  //GET ACTORMANAGER
    public GameObject actorObject; //GET GAMEOBJECT
    public bool selectable; //SELECTABLE OR NOT?

     void Awake()
    {
        actorObject = gameObject;
    }

    void Start()
        {
            actorManager = GameObject.FindObjectOfType<ActorManager>();

            // REGISTER ACTOR
            if (!actorManager.Actors.Contains(this))
            {
                actorManager.Actors.Add(this);
            }
        }

    void OnDestroy()
        {
            // UNREGISTER ACTOR
            if (actorManager)
            {
                actorManager.Actors.Remove(this);
            }
        }
}
