using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 

public class GameOverUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI finalScoreText; 

    [Header("Scene Settings")]
    public string sceneToLoad = "Level1"; 

    void Start()
    {
        if (GlobalGameManager.Instance != null)
        {
            // 1. STOP the score counter so it doesn't keep adding points
            GlobalGameManager.Instance.autoScoreEnabled = false;

            // 2. Get the final number
            int score = GlobalGameManager.Instance.totalScore;
            finalScoreText.text = "Final Score: " + score.ToString();
        }
        else
        {
            finalScoreText.text = "Final Score: 0";
        }
    }

    public void RestartGame()
    {
        if (GlobalGameManager.Instance != null)
        {
            // 3. Reset Data for the new game
            GlobalGameManager.Instance.totalScore = 0;
            GlobalGameManager.Instance.maxInventorySize = 5;
            GlobalGameManager.Instance.savedInventory.Clear();
            
            // 4. Restart the score timer
            GlobalGameManager.Instance.autoScoreEnabled = true; 
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}