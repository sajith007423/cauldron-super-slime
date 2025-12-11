using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    public GlobalInventoryUI inventoryUI; 
    public TextMeshProUGUI consoleText;   

    // Link this to your "SELL" Button
    public void SellFirstItem()
    {
        if (GlobalGameManager.Instance == null) return;

        List<InventoryItem> playerInventory = GlobalGameManager.Instance.savedInventory;

        // 1. Check if empty
        if (playerInventory.Count == 0)
        {
            Log("Inventory is empty! Nothing to sell.");
            return;
        }

        // 2. Identify the FIRST item
        InventoryItem itemToSell = playerInventory[0];

        // 3. Get Price directly from the item
        int earning = (int)itemToSell.price;

        // 4. Sell Logic (Even if price is 0, we process it to clear the slot)
        // If you want to BLOCK selling 0-value items, put 'if (earning > 0)' back here.
        
        GlobalGameManager.Instance.AddScore(earning);
        GlobalGameManager.Instance.savedInventory.RemoveAt(0); // Remove from front

        if (earning > 0)
        {
            Log($"Sold {itemToSell.itemName} for {earning} points.");
        }
        else
        {
            Log($"Discarded {itemToSell.itemName} (No Value).");
        }
        
        // 5. Refresh UI
        if (inventoryUI != null) inventoryUI.UpdateUI();
    }

    void Log(string message)
    {
        if (consoleText != null)
        {
            // --- UPDATED: Overwrite text instead of appending ---
            // This replaces the previous line entirely.
            consoleText.text = "> " + message;
        }
        Debug.Log(message);
    }
}