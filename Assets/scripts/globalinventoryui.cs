using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GlobalInventoryUI : MonoBehaviour
{
    [Header("UI Slots")]
    public List<InventorySlotUI> uiSlots; 

    [Header("Navigation Buttons")]
    public Button leftButton;
    public Button rightButton;

    private int currentStartIndex = 0; 

    void Start()
    {
        UpdateUI();
    }

    // Call this if the Shop changes the inventory data!
    public void UpdateUI()
    {
        if (GlobalGameManager.Instance == null) return;

        List<InventoryItem> globalList = GlobalGameManager.Instance.savedInventory;

        for (int i = 0; i < uiSlots.Count; i++)
        {
            int realIndex = currentStartIndex + i;
            if (realIndex < globalList.Count)
            {
                uiSlots[i].Setup(globalList[realIndex], realIndex);
            }
            else
            {
                uiSlots[i].Setup(null, 0);
            }
        }

        if (leftButton != null) leftButton.gameObject.SetActive(currentStartIndex > 0);
        if (rightButton != null) 
        {
            int visibleCapacity = currentStartIndex + uiSlots.Count;
            rightButton.gameObject.SetActive(visibleCapacity < globalList.Count);
        }
    }

    public void ScrollRight()
    {
        if (GlobalGameManager.Instance != null && currentStartIndex + uiSlots.Count < GlobalGameManager.Instance.savedInventory.Count)
        {
            currentStartIndex++;
            UpdateUI();
        }
    }

    public void ScrollLeft()
    {
        if (currentStartIndex > 0)
        {
            currentStartIndex--;
            UpdateUI();
        }
    }
}