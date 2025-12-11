using UnityEngine;
using System.Collections.Generic;

public class AbsorbableEnemy : MonoBehaviour
{
    [Header("Score Settings")]
    public int scoreReward = 100; 
    
    [Header("Loot Configuration")]
    public List<InventoryItem> possibleLoot = new List<InventoryItem>(); 
    public bool isBeingInhaled = false;

    public string GetEnemyAbilityType()
    {
        if (possibleLoot != null && possibleLoot.Count > 0) return possibleLoot[0].type;
        return "Normal";
    }

    public void StartInhale()
    {
        isBeingInhaled = true;
        
        // 1. --- NEW: Explicitly Disable Attack Script ---
        // We do this first to stop any attacks immediately
        EnemyMultiAttack attackScript = GetComponent<EnemyMultiAttack>();
        if (attackScript != null) 
        {
            attackScript.enabled = false;
        }

        // 2. Disable Physics
        if(GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        if(GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        // 3. Aggressive Script Stopper (Backup)
        // This disables ANY other scripts attached (movement, AI, etc.)
        MonoBehaviour[] allScripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            // Don't disable THIS script (AbsorbableEnemy), or the loot logic will break!
            if (script != this) 
            {
                script.enabled = false;
            }
        }

        // 4. Detach from parent (if it was attached to a patrol path or spawner)
        transform.SetParent(null); 
    }
}