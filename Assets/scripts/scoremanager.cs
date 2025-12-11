using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [Header("UI Reference")]
    public TextMeshProUGUI scoreText; 

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        // Continuously update the text to match Global Data
        if (GlobalGameManager.Instance != null && scoreText != null)
        {
            // 1. Get Score
            int currentScore = GlobalGameManager.Instance.totalScore;

            // 2. Get Inventory Details
            int itemCount = GlobalGameManager.Instance.savedInventory.Count;
            int maxSize = GlobalGameManager.Instance.maxInventorySize;

            // 3. Format the text with a line break (\n)
            // Example Output:
            // Score: 105
            // Size: 3/10
            scoreText.text = $"Score: {currentScore}\nSize: {itemCount}/{maxSize}";
        }
    }

    public void AddScore(int amount)
    {
        if (GlobalGameManager.Instance != null)
        {
            GlobalGameManager.Instance.AddScore(amount);
        }
    }
}