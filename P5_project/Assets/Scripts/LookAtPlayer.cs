using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public Transform neck;    // Reference to the NPC's neck/head transform
    public float lookChance = 0.5f;  // Probability to look at the player (0 to 1)
    public float checkInterval = 2f; // Time interval to re-check (in seconds)
    
    private float timer = 0f;  // Timer to track when to re-check

    private bool shouldLookAtPlayer = true;  // Whether the NPC should currently look at the player

    void LateUpdate()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if it's time to re-evaluate whether the NPC should look at the player
        if (timer >= checkInterval)
        {
            // Reset the timer
            timer = 0f;

            // Randomly decide whether to look at the player (based on lookChance)
            shouldLookAtPlayer = Random.value < lookChance;
        }

        if (shouldLookAtPlayer && player != null && neck != null)
        {
            // Calculate the direction to the player (ignore Y-axis)
            Vector3 directionToPlayer = player.position - neck.position;
            directionToPlayer.y = 0;  // Zero out the Y-axis to avoid vertical movement

            if (directionToPlayer != Vector3.zero)
            {
                // Rotate the neck to face the player on the Y-axis
                neck.rotation = Quaternion.LookRotation(directionToPlayer);

                // Lock the rotation so it only affects the Y-axis
                neck.rotation = Quaternion.Euler(0f, neck.rotation.eulerAngles.y, 0f);
            }
        }
    }
}
