using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelSlideshow : MonoBehaviour
{
    [Header("Content")]
    [Tooltip("Drag your Image GameObjects here. They MUST have a CanvasGroup component attached.")]
    public List<GameObject> slides; 

    [Header("Controls")]
    public Button leftButton;  // "Previous"
    public Button rightButton; // "Next"

    // Internal state
    private int currentIndex = 0;

    void Start()
    {
        if (slides == null || slides.Count == 0)
        {
            Debug.LogError("No slides assigned!");
            return;
        }

        // Initialize: Make the first one visible (Alpha 1), rest invisible (Alpha 0)
        for (int i = 0; i < slides.Count; i++)
        {
            SetSlideVisibility(i, i == 0);
        }

        UpdateButtonStates();
    }

    public void NextSlide()
    {
        if (currentIndex < slides.Count - 1)
        {
            // Hide current
            SetSlideVisibility(currentIndex, false);
            
            // Increment
            currentIndex++;
            
            // Show next
            SetSlideVisibility(currentIndex, true);
            
            UpdateButtonStates();
        }
    }

    public void PrevSlide()
    {
        if (currentIndex > 0)
        {
            // Hide current
            SetSlideVisibility(currentIndex, false);
            
            // Decrement
            currentIndex--;
            
            // Show prev
            SetSlideVisibility(currentIndex, true);
            
            UpdateButtonStates();
        }
    }

    // Helper function to handle the CanvasGroup logic
    void SetSlideVisibility(int index, bool isVisible)
    {
        if (slides[index] == null) return;

        // Try to find the CanvasGroup
        CanvasGroup cg = slides[index].GetComponent<CanvasGroup>();
        
        if (cg != null)
        {
            // Set Alpha: 1 = Visible, 0 = Invisible
            cg.alpha = isVisible ? 1f : 0f;
            
            // Optional: Block raycasts on invisible items so you can't click buttons on hidden slides
            cg.blocksRaycasts = isVisible; 
            cg.interactable = isVisible;
        }
        else
        {
            // Fallback if you forgot to add CanvasGroup component
            Debug.LogWarning($"Slide '{slides[index].name}' is missing a CanvasGroup component! Using SetActive instead.");
            slides[index].SetActive(isVisible);
        }
    }

    void UpdateButtonStates()
    {
        if (leftButton != null) leftButton.interactable = (currentIndex > 0);
        if (rightButton != null) rightButton.interactable = (currentIndex < slides.Count - 1);
    }
}