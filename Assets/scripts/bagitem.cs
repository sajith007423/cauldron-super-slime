using UnityEngine;
using UnityEngine.SceneManagement;

// Inherit from MonoBehaviour (Standard Object) instead of AbsorbableEnemy
public class BagItem : MonoBehaviour 
{
    [Header("Reward Settings")]
    [Tooltip("How many extra slots to add if the random roll fails.")]
    public int extraSlots = 1;

    [Header("Randomization")]
    [Range(0, 100)]
    [Tooltip("0 = Always Extra Slots, 100 = Always Scene Load, 50 = 50/50 Chance")]
    public float sceneLoadChance = 50f;

    [Tooltip("Name of the scene to load if the random roll hits.")]
    public string sceneToLoad = "BonusLevel";

    // --- NEW: TRIGGER DETECTION ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object colliding with us is the Player
        // (Make sure your Player GameObject has the Tag "Player")
        if (collision.CompareTag("Player"))
        {
            CollectBag();
        }
    }

    void CollectBag()
    {
        // 1. Roll the Dice (0 to 100)
        float diceRoll = Random.Range(0f, 100f);
        bool isSceneLoad = diceRoll < sceneLoadChance;

        if (GlobalGameManager.Instance != null)
        {
            if (isSceneLoad)
            {
                Debug.Log($"Bag Collected! Loading Scene: {sceneToLoad}");
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                Debug.Log($"Bag Collected! Expanding Inventory by {extraSlots}...");
                
                // Increase Global Max Inventory
                GlobalGameManager.Instance.maxInventorySize += extraSlots;
            }
        }

        // 2. Destroy the bag immediately so it can't be picked up twice
        Destroy(gameObject);
    }
}