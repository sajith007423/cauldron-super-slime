using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager Instance;

    [Header("Persistent Data")]
    public int totalScore = 0;
    public int maxInventorySize = 10;
    // This list holds InventoryItems. Since InventoryItem now has a 'price' field,
    // this list automatically supports it!
    public List<InventoryItem> savedInventory = new List<InventoryItem>();

    [Header("Passive Score Settings")]
    public bool autoScoreEnabled = true; 
    public int pointsPerInterval = 1;    
    public float interval = 1.0f;        

    [Tooltip("List the exact names of scenes where score should increase")]
    public List<string> allowedScoreScenes = new List<string>(); 
    
    private float timer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (autoScoreEnabled)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (allowedScoreScenes.Contains(currentScene))
            {
                timer += Time.deltaTime;

                if (timer >= interval)
                {
                    AddScore(pointsPerInterval);
                    timer = 0f; 
                }
            }
        }
    }

    public void AddScore(int amount)
    {
        totalScore += amount;
    }
}