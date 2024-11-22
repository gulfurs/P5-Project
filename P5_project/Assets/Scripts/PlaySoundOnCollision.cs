using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    public AudioClip soundClip; // The sound to play
    private AudioSource audioSource; // The audio source component
    private bool hasPlayed = false; // Flag to ensure sound is only played once

    void Start()
    {
        // Get or add an AudioSource component to the GameObject
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object that collided has the tag "Player" and sound hasn't been played yet
        if (other.CompareTag("Player") && !hasPlayed)
        {
            // Play the sound
            audioSource.PlayOneShot(soundClip);
            hasPlayed = true; // Prevent further sound plays
        }
    }
}
