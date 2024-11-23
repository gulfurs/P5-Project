using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TriggerEvents : MonoBehaviour
{
    public PlayableDirector triggerDirector;
    public PlayableAsset triggerTimeline; // Timeline to run for start dialogue

    public void Start() {
        Debug.Log(gameObject);
    }

    public void triggeringTimeline() {
        PlayTimeline(triggerTimeline);
    }

    //METHOD FOR PLAYING INTERACTION EVENT
    private void PlayTimeline(PlayableAsset timeline)
    {
        if (timeline != null)
        {
        triggerDirector.playableAsset = timeline;
        triggerDirector.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            triggeringTimeline();
        }
    }
}
