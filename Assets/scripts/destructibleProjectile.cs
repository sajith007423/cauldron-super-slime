using UnityEngine;

public class DestructibleProjectile : MonoBehaviour
{
    [Header("Destruction Settings")]
    [Tooltip("How many times this projectile needs to be hit to be destroyed")]
    public int hitsRequired = 3;

    [Tooltip("The exact Tag of the bullet that can damage this projectile (e.g., 'PlayerBullet')")]
    public string damagingBulletTag = "friendlyprojectile";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object hitting us has the specific tag we are looking for
        if (collision.CompareTag(damagingBulletTag))
        {
            // Reduce the health/hits
            hitsRequired--;

            // Optional: Destroy the bullet that hit us so it doesn't pass through
            Destroy(collision.gameObject);

            // If hits reach 0, destroy this projectile
            if (hitsRequired <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}