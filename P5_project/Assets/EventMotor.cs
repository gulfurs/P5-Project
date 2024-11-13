using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EventMotor : MonoBehaviour
{
    public string consUID;
    public GameObject[] objectsToCheck;

    private EventManager eventManager;
    private PlayableDirector playableDirector;

    void Start()
    {
        eventManager = GetComponent<EventManager>();
        playableDirector = GetComponent<PlayableDirector>();

        var actorManager = FindObjectOfType<ActorManager>();
        actorManager.noActorsEvent += noActors;
    }

    void noActors()
    {
        Debug.Log("NO ACTORS");
        playableDirector.time = 0;
        playableDirector?.Play();
    }
}
