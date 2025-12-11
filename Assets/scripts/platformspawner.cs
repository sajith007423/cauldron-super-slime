using UnityEngine;
using System.Collections.Generic; // Required for Lists

// Define the weighted entry class outside or inside the namespace

public class PlatformSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEntry
    {
        public string name; 
        public GameObject prefab;
        [Tooltip("Higher value means higher chance to spawn compared to others.")]
        [Range(0, 100)] public float weight = 10f; 
    }

    [Header("Platform Configuration")]
    // --- CHANGED: List of Platforms with weights ---
    public List<SpawnEntry> platformList; 

    [Header("Enemy Configuration")]
    // --- CHANGED: List of Enemies with weights ---
    public List<SpawnEntry> enemyList; 

    [Header("X Axis (Horizontal)")]
    public float spacingX = 4.0f;

    [Header("Y Axis (Vertical)")]
    public float maxSpacingY = 2.5f;
    public float absoluteMinY = -4f;
    public float absoluteMaxY = 3f;

    [Header("Enemy Settings")]
    [Tooltip("Global chance to spawn ANY enemy on a platform")]
    [Range(0f, 1f)] public float globalEnemySpawnChance = 0.5f; 
    public float enemyHeightOffset = 1.0f; 

    private GameObject lastSpawnedPlatform;

    void Start()
    {
        // Safety check
        if (platformList.Count == 0)
        {
            Debug.LogError("No Platforms assigned in Spawner!");
            return;
        }
        SpawnPlatform(transform.position);
    }

    void Update()
    {
        if (lastSpawnedPlatform == null)
        {
            SpawnPlatform(transform.position);
            return;
        }

        float distanceGap = transform.position.x - lastSpawnedPlatform.transform.position.x;

        if (distanceGap >= spacingX)
        {
            GenerateNextPosition();
        }
    }

    void GenerateNextPosition()
    {
        float prevY = lastSpawnedPlatform.transform.position.y;
        float randomYDiff = Random.Range(-maxSpacingY, maxSpacingY);
        float newY = Mathf.Clamp(prevY + randomYDiff, absoluteMinY, absoluteMaxY);
        
        Vector3 spawnPos = new Vector3(transform.position.x, newY, 0);

        SpawnPlatform(spawnPos);
    }

    void SpawnPlatform(Vector3 pos)
    {
        // 1. Pick a specific platform based on weight (e.g. Grass vs Ice)
        GameObject selectedPlatformPrefab = GetRandomWeightedPrefab(platformList);

        if (selectedPlatformPrefab != null)
        {
            lastSpawnedPlatform = Instantiate(selectedPlatformPrefab, pos, Quaternion.identity);

            // 2. Decide IF we spawn an enemy at all
            if (Random.value < globalEnemySpawnChance && enemyList.Count > 0)
            {
                SpawnEnemyOnPlatform(lastSpawnedPlatform);
            }
        }
    }

    void SpawnEnemyOnPlatform(GameObject platform)
    {
        Vector3 enemyPos = new Vector3(
            platform.transform.position.x, 
            platform.transform.position.y + enemyHeightOffset, 
            0
        );

        // 3. Pick a specific enemy based on weight (e.g. Slime vs Dragon)
        GameObject selectedEnemyPrefab = GetRandomWeightedPrefab(enemyList);

        if (selectedEnemyPrefab != null)
        {
            GameObject newEnemy = Instantiate(selectedEnemyPrefab, enemyPos, Quaternion.identity);
            
            // Parent enemy to platform so it moves with it
            newEnemy.transform.SetParent(platform.transform);
        }
    }

    // --- HELPER: Weighted Random Algorithm ---
    GameObject GetRandomWeightedPrefab(List<SpawnEntry> list)
    {
        if (list.Count == 0) return null;

        // Step A: Sum up all the weights
        float totalWeight = 0f;
        foreach (var entry in list)
        {
            totalWeight += entry.weight;
        }

        // Step B: Pick a random value within that total
        float randomValue = Random.Range(0, totalWeight);

        // Step C: Find which item corresponds to that value
        float currentSum = 0f;
        foreach (var entry in list)
        {
            currentSum += entry.weight;
            if (randomValue <= currentSum)
            {
                return entry.prefab;
            }
        }

        // Fallback: return the first one if math fails slightly
        return list[0].prefab;
    }
}