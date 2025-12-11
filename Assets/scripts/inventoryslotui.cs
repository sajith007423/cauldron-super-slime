using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI numberText; 
    public GameObject contentParent;

    // Notice: We removed the 'ShopManager' parameter from this function
    public void Setup(InventoryItem item, int indexNumber)
    {
        if (item != null)
        {
            contentParent.SetActive(true);
            iconImage.sprite = item.icon;
            
            if (numberText != null) 
                numberText.text = "#" + (indexNumber + 1).ToString();
        }
        else
        {
            contentParent.SetActive(false);
        }
    }
}