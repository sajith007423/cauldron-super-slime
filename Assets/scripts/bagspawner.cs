using UnityEngine;
using System.Collections.Generic;

public class BagSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject bagPrefab; // Drag your BagNScale prefab here
    public float checkInterval = 2.0f; // Check every 2 seconds
    
    [Header("Spawn Area")]
    public Vector2 xRange = new Vector2(-8, 8); // Left/Right limits
    public Vector2 yRange = new Vector2(-4, 4); // Up/Down limits

    private GameObject currentBag; // Track existing bag so we don't spawn 100 of them
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            CheckAndSpawn();
            timer = 0f;
        }
    }

    void CheckAndSpawn()
    {
        if (GlobalGameManager.Instance == null) return;

        // 1. Check if Inventory is Full
        int currentCount = GlobalGameManager.Instance.savedInventory.Count;
        int maxSize = GlobalGameManager.Instance.maxInventorySize;

        if (currentCount >= maxSize)
        {
            // 2. Only spawn if one doesn't already exist
            if (currentBag == null)
            {
                SpawnBag();
            }
        }
    }

    void SpawnBag()
    {
        // Pick random position
        float randX = Random.Range(xRange.x, xRange.y);
        float randY = Random.Range(yRange.x, yRange.y);
        Vector3 spawnPos = new Vector3(randX, randY, 0);

        // Spawn
        currentBag = Instantiate(bagPrefab, spawnPos, Quaternion.identity);
        
        Debug.Log("Inventory Full! BagNScale spawned at: " + spawnPos);
    }
}