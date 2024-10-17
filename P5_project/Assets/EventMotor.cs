using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMotor : MonoBehaviour
{
    public string consUID;
    public GameObject[] objectsToCheck;

    private EventManager eventManager;

    void Start()
    {
        eventManager = GetComponent<EventManager>();
    }

    void Update()
    {
        if (eventManager.eventTrackerList.Contains(consUID))
        {
            foreach (GameObject obj in objectsToCheck)
            {
                Actor actor = obj.GetComponent<Actor>();

                if (actor != null)
                {
                    actor.selectable = true;
                }
            }
        }
    }
}
