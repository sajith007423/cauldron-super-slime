using UnityEngine;
using System.Collections.Generic;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Connections")]
    public KirbyAbilities playerScript; 
    public List<InventorySlotUI> uiSlots; 

    private int currentStartIndex = 0; 

    void Start()
    {
        // DEBUG: Check connections on Start
        if (playerScript == null) Debug.LogError("ERROR: Player Script is NOT assigned in InventoryDisplay!");
        if (uiSlots == null || uiSlots.Count == 0) Debug.LogError("ERROR: UI Slots list is EMPTY! Drag your slot objects into the list.");
        
        UpdateUI();
    }

    void Update()
    {
        // Optimization: Only update if the inventory count changes or we scroll
        // (Constantly updating every frame in Update() can cause lag, but is fine for debugging)
        UpdateUI();
    }

    public void ScrollRight()
    {
        // DEBUG: Print values to console
        int totalItems = playerScript.playerInventory.Count;
        int slotCount = uiSlots.Count;
        
        Debug.Log($"Attempting Scroll Right... StartIndex: {currentStartIndex} | Slots: {slotCount} | Total Items: {totalItems}");

        if (currentStartIndex + slotCount < totalItems)
        {
            currentStartIndex++;
            Debug.Log("Scroll Right SUCCESS. New Index: " + currentStartIndex);
            UpdateUI();
        }
        else
        {
            Debug.Log("Scroll Right BLOCKED. You are at the end of the list.");
        }
    }

    public void ScrollLeft()
    {
        Debug.Log($"Attempting Scroll Left... Current Index: {currentStartIndex}");

        if (currentStartIndex > 0)
        {
            currentStartIndex--;
            Debug.Log("Scroll Left SUCCESS. New Index: " + currentStartIndex);
            UpdateUI();
        }
        else
        {
            Debug.Log("Scroll Left BLOCKED. You are already at the start.");
        }
    }

    public void UpdateUI()
    {
        if (playerScript == null) return;

        List<InventoryItem> inventory = playerScript.playerInventory;

        for (int i = 0; i < uiSlots.Count; i++)
        {
            int realIndex = currentStartIndex + i;

            if (realIndex < inventory.Count)
            {
                uiSlots[i].Setup(inventory[realIndex], realIndex);
            }
            else
            {
                uiSlots[i].Setup(null, 0);
            }
        }
    }
}