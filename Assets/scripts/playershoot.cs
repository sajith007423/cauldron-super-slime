using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public Transform firePoint;     // Where the bullet spawns
    public GameObject projectilePrefab; // The bullet prefab
    public KeyCode shootKey = KeyCode.K;

    void Update()
    {
        // Check for K button press
        if (Input.GetKeyDown(shootKey))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Instantiate the bullet at the firePoint position
        // Quaternion.identity means "No Rotation" (Straight)
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }
}