using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMotor : MonoBehaviour
{
    // Public method to load the next scene in the build index order
    public void NextScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Calculate the next scene index, wrapping around if needed
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        
        // Load the next scene
        SceneManager.LoadScene(nextSceneIndex);
    }
}
