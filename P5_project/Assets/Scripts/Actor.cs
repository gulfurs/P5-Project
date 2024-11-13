using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//HOME TO INTERACTABLE OBJECTS
public class Actor : MonoBehaviour
{
    ActorManager actorManager;  //GET ACTORMANAGER
    public GameObject actorObject; //GET GAMEOBJECT
    public bool selectable; //SELECTABLE OR NOT?
    public GameObject actorID;
    public Color actorColor;
    public GameObject faceID;
    public string actorName;

     void Awake()
    {
        actorObject = gameObject;
    }

    void Start()
        {
            faceID = RecursiveFindChild(actorObject.transform, "ORG-face")?.gameObject;

            actorManager = GameObject.FindObjectOfType<ActorManager>();

            // REGISTER ACTOR
            if (actorManager.Player != gameObject && !actorManager.Actors.Contains(this))
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

    Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if(child.name == childName)
            {
                return child;
            }
         else
        {
            Transform found = RecursiveFindChild(child, childName);
            if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }
}
