using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes

public class SimpleSceneChanger : MonoBehaviour
{
    [Tooltip("Type the exact name of the scene you want to load")]
    public string sceneToLoad; 

    // Link this to your Button's OnClick event
    public void LoadGameScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}