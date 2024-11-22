using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CustomInteraction : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public PlayableAsset interactionTimeline; // Timeline to run for start dialogue

    public void InitializeInteraction() {
        PlayTimeline(interactionTimeline);
    }

    //METHOD FOR PLAYING INTERACTION EVENT
    private void PlayTimeline(PlayableAsset timeline)
    {
        if (timeline != null)
        {
        playableDirector.playableAsset = timeline;
        playableDirector.Play();
        }
    }
}
