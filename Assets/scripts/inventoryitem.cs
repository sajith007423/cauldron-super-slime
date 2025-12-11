using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    
    [Tooltip("e.g. 'Weapon', 'Consumable', 'KeyItem'")]
    public string type; 

    public Sprite icon;
    
    [TextArea] public string description;

    [Header("Economy")]
    public float price = 0f;

    [Header("Rarity Settings")]
    [Range(0, 100)] 
    public float rarityPercentage = 50f; 
}