using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//PLAYER INTERACTION MADE POSSIBLE
public class PlayerInteraction : MonoBehaviour {

    private Transform highlight;
    public Transform selection;
    private RaycastHit raycastHit;

    public event Action<Transform> OnSelectionChanged; //EVENT FOR CHANGE OF SELECTION

    void Update()
    {
        // REMOVES HIGHLIGHT AND DEACTIVATES actorID
        if (highlight != null)
        {
            var outline = highlight.gameObject.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }

            // Deactivate actorID when no longer hovering
            Actor prevActor = highlight.GetComponent<Actor>();
            if (prevActor != null && prevActor.actorID != null)
            {
                prevActor.actorID.SetActive(false);
            }

            highlight = null;
        }

        // CAST RAY FROM MOUSE. CHECKS IF HITS GAMEOBJECT
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;

            // CHECK IF ACTOR
            Actor actor = highlight.GetComponent<Actor>();
            if (actor != null && actor.selectable && highlight != selection)
            {
                var outline = highlight.gameObject.GetComponent<Outline>();
                if (actor.actorID != null) {
                    actor.actorID.SetActive(true); // Show actorID on hover
                }
                outline.enabled = true;
            }
            else
            {
                highlight = null; // Reset highlight when not selectable
            }
        }

        // CLICK HIGHLIGHTED OBJECT?
        if (Input.GetMouseButtonDown(0))
        {
            if (highlight)
            {
                if (selection != null)
                {
                    //selection.gameObject.GetComponent<Outline>().enabled = false;
                }
                selection = raycastHit.transform;
                //selection.gameObject.GetComponent<Outline>().enabled = true;
                highlight = null;
                OnSelectionChanged?.Invoke(selection);
            }
            else
            {
                if (selection)
                {
                    //selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                    OnSelectionChanged?.Invoke(null);
                }
            }
        }
    }
}
