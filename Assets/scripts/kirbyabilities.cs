using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;

public class KirbyAbilities : MonoBehaviour
{   
    [Header("Controls")]
    public KeyCode inhaleKey = KeyCode.LeftControl;

    [Header("Game Over Settings")]
    public string enemyBulletTag = "EnemyBullet"; 
    public string gameOverSceneName = "GameOver"; 
    public Image transitionPanel;   
    public float fadeDuration = 1.0f; 

    [Header("Settings")]
    public Transform mouthPoint;
    public float inhaleRange = 3f;
    public float suckSpeed = 10f;
    public LayerMask enemyLayer;

    [Tooltip("If true, releasing the key kills the enemy. If false, it drops them.")]
    public bool killOnInterrupt = true;

    [Header("Inhale Charge System")]
    public float maxCharge = 100f;
    public float depletionRate = 20f;
    public float rechargeRate = 15f;
    public float rechargeDelay = 1.0f;

    [Header("Debug View")]
    [SerializeField] private float currentCharge;
    private float lastInhaleTime;
    private float lastFullInventoryLogTime; // Limits log spam

    [Header("UI Battery Display")]
    public Image batteryImage;
    public Sprite bat100, bat50, bat25, bat0;

    [Header("UI Game Console")]
    public Transform consoleContainer; 
    public GameObject textPrefab;      
    public float textLifeTime = 3.0f;  
    
    // --- UPDATED: Single Object Reference (No List) ---
    private GameObject activeConsoleLine; 

    [Header("Visual Effects")]
    public Animator playerAnimator;
    public string animParamName = "isInhaling";
    public ParticleSystem suckEffect;
    public Color fullChargeColor = Color.cyan;
    public Color emptyChargeColor = Color.red;

    [Header("Inventory")]
    public List<InventoryItem> playerInventory = new List<InventoryItem>();
    
    [Tooltip("Read-Only View. The real limit is in GlobalGameManager.")]
    [SerializeField] private int currentLimitDebug; 

    private AbsorbableEnemy targetEnemy;

    void Start()
    {
        currentCharge = maxCharge;

        if (GlobalGameManager.Instance != null)
        {
            playerInventory = GlobalGameManager.Instance.savedInventory;
            if (transitionPanel != null) transitionPanel.color = new Color(0,0,0,0);
        }
    }

    void Update()
    {
        HandleInputAndCharge();
    }

    int GetMaxInventorySize()
    {
        if (GlobalGameManager.Instance != null)
            return GlobalGameManager.Instance.maxInventorySize;
        return 10; 
    }

    void HandleInputAndCharge()
    {
        currentLimitDebug = GetMaxInventorySize();

        bool isHoldingButton = Input.GetKey(inhaleKey);
        bool canInhale = currentCharge > 0;
        bool isActivelyInhaling = isHoldingButton && canInhale;

        if (isActivelyInhaling)
        {
            currentCharge -= depletionRate * Time.deltaTime;
            lastInhaleTime = Time.time;
            if (currentCharge < 0) currentCharge = 0;
        }
        else
        {
            if (Time.time > lastInhaleTime + rechargeDelay)
            {
                currentCharge += rechargeRate * Time.deltaTime;
                if (currentCharge > maxCharge) currentCharge = maxCharge;
            }
        }

        UpdateVisuals(isActivelyInhaling);

        // --- UPDATED STATE MACHINE ---
        if (isActivelyInhaling && targetEnemy == null)
        {
            // Check: Is there space?
            if (playerInventory.Count < GetMaxInventorySize())
            {
                PerformInhale();
            }
            else
            {
                // --- NEW LOGIC: Log "Inventory Full" ---
                // We use a timer so it logs once every 2 seconds instead of every frame
                if (Time.time > lastFullInventoryLogTime + 2.0f)
                {
                    LogToConsole("Inventory Full! Spawning BagNScale...", Color.cyan);
                    lastFullInventoryLogTime = Time.time;
                }
            }
        }
        else if (!isActivelyInhaling && targetEnemy != null)
        {
            ReleaseEnemy();
        }

        if (targetEnemy != null)
        {
            MoveEnemyToMouth();
        }
    }

    void ReleaseEnemy()
    {
        if (targetEnemy != null)
        {
            // --- NEW: Log the failure message ---
            LogToConsole("Failed absorption... may be due to low charge.", Color.yellow);

            if (killOnInterrupt)
            {
                Destroy(targetEnemy.gameObject);
            }
            else
            {
                targetEnemy.isBeingInhaled = false;
                Collider2D col = targetEnemy.GetComponent<Collider2D>();
                if (col) col.enabled = true;
                Rigidbody2D rb = targetEnemy.GetComponent<Rigidbody2D>();
                if (rb) rb.isKinematic = false;

                MonoBehaviour[] allScripts = targetEnemy.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in allScripts) script.enabled = true;
            }
            targetEnemy = null;
        }
    }

    void PerformInhale()
    {
        Collider2D hit = Physics2D.OverlapCircle(mouthPoint.position, inhaleRange, enemyLayer);
        if (hit)
        {
            AbsorbableEnemy e = hit.GetComponent<AbsorbableEnemy>();
            if (e)
            {
                targetEnemy = e;
                e.StartInhale();
            }
        }
    }

    void MoveEnemyToMouth()
    {
        if (targetEnemy == null) return;

        targetEnemy.transform.position = Vector3.MoveTowards(targetEnemy.transform.position, mouthPoint.position, suckSpeed * Time.deltaTime);
        targetEnemy.transform.localScale = Vector3.Lerp(targetEnemy.transform.localScale, Vector3.zero, suckSpeed * Time.deltaTime);

        if (Vector3.Distance(targetEnemy.transform.position, mouthPoint.position) < 0.2f)
            ConsumeEnemy();
    }

    void ConsumeEnemy()
    {
        if (GlobalGameManager.Instance != null)
            GlobalGameManager.Instance.AddScore(targetEnemy.scoreReward);

        if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(targetEnemy.scoreReward);

        List<string> gainedItemsNames = new List<string>();

        foreach (InventoryItem item in targetEnemy.possibleLoot)
        {
            float diceRoll = Random.Range(0f, 100f);
            
            if (diceRoll <= item.rarityPercentage && playerInventory.Count < GetMaxInventorySize())
            {
                InventoryItem newItem = new InventoryItem();
                newItem.itemName = item.itemName;
                newItem.type = item.type;
                newItem.icon = item.icon;
                newItem.description = item.description;
                newItem.rarityPercentage = item.rarityPercentage;
                newItem.price = item.price; 

                playerInventory.Add(newItem);
                gainedItemsNames.Add(item.itemName);
            }
        }

        Destroy(targetEnemy.gameObject);
        targetEnemy = null;

        if (gainedItemsNames.Count > 0)
        {
            string message = $"Obtained: {string.Join(", ", gainedItemsNames)}";
            LogToConsole(message, Color.green);
        }
        else
        {
            LogToConsole("No loot found.", Color.red);
        }
    }

    // ... (Collision, DieSequence, Visuals, Battery - NO CHANGES) ...
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(enemyBulletTag)) StartCoroutine(DieSequence());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(enemyBulletTag)) StartCoroutine(DieSequence());
    }

    System.Collections.IEnumerator DieSequence()
    {
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        if(GetComponent<Rigidbody2D>() != null) GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        if(transitionPanel != null)
        {
            float t = 0;
            while(t < fadeDuration) { t += Time.deltaTime; transitionPanel.color = new Color(0,0,0, t/fadeDuration); yield return null; }
            transitionPanel.color = Color.black;
        }
        SceneManager.LoadScene(gameOverSceneName);
    }
    
    // Hidden standard functions
    void UpdateVisuals(bool inhaling) { if (playerAnimator != null) playerAnimator.SetBool(animParamName, inhaling); if (suckEffect != null) { if (inhaling && !suckEffect.isPlaying) suckEffect.Play(); else if (!inhaling && suckEffect.isPlaying) suckEffect.Stop(); var mainModule = suckEffect.main; float chargePercent = currentCharge / maxCharge; mainModule.startColor = Color.Lerp(emptyChargeColor, fullChargeColor, chargePercent); } if (batteryImage != null) UpdateBatteryUI(); }
    void UpdateBatteryUI() { float percent = currentCharge / maxCharge; if (percent > 0.50f) batteryImage.sprite = bat100; else if (percent > 0.25f) batteryImage.sprite = bat50; else if (percent > 0.0f) batteryImage.sprite = bat25; else batteryImage.sprite = bat0; }

    // --- UPDATED LOGGING FUNCTION ---
    void LogToConsole(string message, Color optionalColor)
    {
        if (consoleContainer == null || textPrefab == null)
        {
            Debug.Log(message);
            return;
        }

        // 1. Destroy the old line if it exists
        if (activeConsoleLine != null)
        {
            Destroy(activeConsoleLine);
        }

        // 2. Instantiate the new line
        activeConsoleLine = Instantiate(textPrefab, consoleContainer);
        
        // 3. Set Text & Color
        TMP_Text tmpText = activeConsoleLine.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = "> " + message;
            if (optionalColor != default) tmpText.color = optionalColor;
        }

        // 4. Set Order
        activeConsoleLine.transform.SetAsLastSibling();

        // 5. Auto Destroy after time
        Destroy(activeConsoleLine, textLifeTime);
    }
}