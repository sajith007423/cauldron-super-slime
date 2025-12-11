using UnityEngine;
using UnityEngine.SceneManagement; 

public class ChangeSceneOnKey : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The exact name of the scene you want to load.")]
    public string sceneToLoad;

    void Update()
    {
        // Check if ANY key or mouse button was pressed this frame
        if (Input.anyKeyDown)
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("ChangeSceneOnKey: No scene name assigned in the Inspector!");
        }
    }
}