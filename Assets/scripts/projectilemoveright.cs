using UnityEngine;

public class ProjectileMoveRight : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 10f;
    public float lifeTime = 2f; // Auto-destroy after 2 seconds
    public int damage = 1;

    // REMOVED: Rigidbody2D variable
    // REMOVED: Awake() method

    void Start()
    {
        // Schedule auto-destruction to clean up memory
        Destroy(gameObject, lifeTime);
    }

    // --- NEW: Movement Logic ---
    void Update()
    {
        // Move the object to the Right every single frame
        // Time.deltaTime ensures it moves at the same speed on fast/slow computers
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Logic to hit enemy
        // Example: EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
        // if (enemy != null) enemy.TakeDamage(damage);

        // Destroy bullet on impact (unless it's the player or a trigger)
        if (!hitInfo.CompareTag("Player") && !hitInfo.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}