using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//KEEPS PLAYER AND ACTORS UNDER WRAP
public class ActorManager : MonoBehaviour
{
    public List<Actor> Actors { get; private set; }
    public GameObject Player { get; private set; }

    public void SetPlayer(GameObject player) => Player = player;

     void Awake()
        {
            Actors = new List<Actor>();

             // Find the GameObject tagged as "Player"
            GameObject playerObject = GameObject.FindWithTag("Player");
            
            if (playerObject != null)
            {
            SetPlayer(playerObject);  // Assign the found player object
            }
            else
            {
            Debug.LogError("No GameObject tagged 'Player' found in the scene.");
            }
        }
}
