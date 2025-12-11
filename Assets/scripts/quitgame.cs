using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Link this function to your Button's OnClick event
    public void QuitTheGame()
    {
        Debug.Log("Quit Game triggered!"); // Confirms it works in the Console

        // 1. If we are running in the Unity Editor, stop playing
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 2. If we are in a built game, close the application
            Application.Quit();
        #endif
    }
}