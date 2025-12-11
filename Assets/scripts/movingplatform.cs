using UnityEngine;

public class movingplatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;          // Speed moving to the left
    public float deadZone = -15f;     // X position where object is destroyed

    void Update()
    {
        // Move the platform to the left constantly
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // Garbage collection: Destroy platform if it moves too far left
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}