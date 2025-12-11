using UnityEngine;

public class loopingbackground : MonoBehaviour
{
    public float speed = 4f;       // How fast it scrolls
    public float backgroundWidth;  // Width of the sprite

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        
        // Automatically calculate width based on the SpriteRenderer
        // ensuring it works regardless of image scale
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move the background to the left
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Check if the background has moved far enough to reset
        // We use absolute value to handle movement in either direction
        if (transform.position.x < startPosition.x - backgroundWidth)
        {
            // Reset position to the right side of the visual chain
            transform.position = new Vector3(transform.position.x + 2 * backgroundWidth, startPosition.y, startPosition.z);
        }
    }
}