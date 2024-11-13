using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//KEEPS PLAYER AND ACTORS UNDER WRAP
public class ActorManager : MonoBehaviour
{
    public List<Actor> Actors { get; private set; }
    public GameObject Player { get; private set; }

    public event Action noActorsEvent;

    public void SetPlayer(GameObject player) => Player = player;

     void Awake()
        {
            Actors = new List<Actor>();

             // Find the GameObject tagged as "Player"
            GameObject playerObject = GameObject.FindWithTag("Player");
            
            if (playerObject != null)
            {
            SetPlayer(playerObject);
            }
            else
            {
            Debug.LogError("No GameObject tagged 'Player' found in the scene.");
            }
        }

    public void RemoveActor(Actor actor)
    {
        if (Actors.Contains(actor))
        {
            Actors.Remove(actor);
            CheckforActors();
        }
    }

    private void CheckforActors()
    {
        if (Actors.Count == 0)
        {
            noActorsEvent?.Invoke();
        }
    }
}
