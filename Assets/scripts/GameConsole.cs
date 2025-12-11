using UnityEngine;
using TMPro; // Required for Text Mesh Pro
using System.Collections.Generic;

public class GameConsole : MonoBehaviour
{
    public static GameConsole Instance; // Singleton for easy access

    [Header("Settings")]
    public GameObject textPrefab; // Drag your Text Prefab here
    public int maxLines = 5;      // How many lines to show before deleting old ones
    public float textLifeTime = 3.0f; // How long a line stays visible

    private List<GameObject> consoleLines = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    // Call this function to print to your screen!
    public void Log(string message, Color optionalColor = default)
    {
        // 1. Create the new text line inside the panel
        GameObject newLine = Instantiate(textPrefab, transform);
        
        // 2. Set the Text
        TMP_Text tmpText = newLine.GetComponent<TMP_Text>();
        tmpText.text = "> " + message; // Add a console bracket ">"

        // 3. Set Color (If provided, otherwise keep prefab default)
        if (optionalColor != default)
        {
            tmpText.color = optionalColor;
        }

        // 4. Set it as the LAST sibling (appears at bottom)
        // If you want new items at the TOP, use SetAsFirstSibling()
        newLine.transform.SetAsLastSibling();

        // 5. Manage the List size
        consoleLines.Add(newLine);

        if (consoleLines.Count > maxLines)
        {
            // Destroy the oldest line (index 0)
            Destroy(consoleLines[0]);
            consoleLines.RemoveAt(0);
        }

        // 6. Auto-destroy this specific line after X seconds
        Destroy(newLine, textLifeTime);
    }
}