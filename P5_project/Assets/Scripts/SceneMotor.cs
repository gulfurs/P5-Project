using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMotor : MonoBehaviour
{
    private AsyncOperation preloadOperation;

    // Public method to load the next scene in the build index order
    public void NextScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Calculate the next scene index, wrapping around if needed
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        
        // Load the next scene
        //SceneManager.LoadScene(nextSceneIndex);

        StartCoroutine(PreloadScene(nextSceneIndex));
    }

    private IEnumerator PreloadScene(int sceneIndex)
    {
        // PRELOAD SCENES ASYNC
        preloadOperation = SceneManager.LoadSceneAsync(sceneIndex);
        preloadOperation.allowSceneActivation = false; 

        // WAIT TILL SCENE IS LOADED
        while (preloadOperation.progress < 0.9f)
        {
            // DISPLAY PROGRESS??
            Debug.Log($"Loading progress: {preloadOperation.progress * 100}%");
            yield return null;
        }

        Debug.Log("Scene preloaded but not activated.");
    }

    // ACTIVATE PRELOADED SCENE
    public void ActivatePreloadedScene()
    {
        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = true;
        }
    }
}
