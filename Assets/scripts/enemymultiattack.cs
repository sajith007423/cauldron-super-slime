using UnityEngine;
using System.Collections;

public class EnemyMultiAttack : MonoBehaviour
{
    private enum AttackType { Slash, Shoot, Summon }

    [Header("AI Timing")]
    public float minDelay = 1.5f;
    public float maxDelay = 3.5f;

    [Header("1. Slash Attack")]
    [Tooltip("How far LEFT from the enemy center the slash happens")]
    public float slashOffsetX = 1.0f; 
    public float slashRadius = 1.5f; 
    public LayerMask playerLayer;    
    public GameObject slashVisualPrefab; 
    public float slashDuration = 0.5f; 

    [Header("2. Shooting Attack")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 6f;
    public bool useShotgunPattern = false;
    public float bulletLifetime = 3.0f; 

    [Header("3. Summon Minion")]
    public GameObject minionPrefab;
    public Transform[] summonSpawnPoints; 

    // Internal Flags
    private Transform selfTransform; 
    private bool isVisible = false;
    private bool isDead = false; // --- NEW: Track death state

    void Start()
    {
        selfTransform = this.transform;
        StartCoroutine(AttackRoutine());
    }

    // --- NEW: Public function to stop the enemy ---
    // Call this from your Health Script when HP reaches 0
    public void Die()
    {
        if (isDead) return; // Already dead, do nothing

        isDead = true;
        
        // 1. Immediately stop the attack loop
        StopAllCoroutines();

        // 2. Disable physics (Optional: prevents player taking touch damage from dead body)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 3. Optional: Play death animation here if you have one
        // GetComponent<Animator>().SetTrigger("Die");

        Debug.Log("Enemy died! Stopping attacks.");
    }

    void OnBecameVisible() { isVisible = true; }
    void OnBecameInvisible() { isVisible = false; }

    IEnumerator AttackRoutine()
    {
        // Loop runs only while NOT dead
        while (!isDead)
        {
            // 1. Wait for visibility
            yield return new WaitUntil(() => isVisible && !isDead);

            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            // Double check: Did we die or go off-screen during the wait?
            if (!isVisible || isDead) continue;

            AttackType randAttack = (AttackType)Random.Range(0, 3);

            switch (randAttack)
            {
                case AttackType.Slash: PerformSlash(); break;
                case AttackType.Shoot: PerformShoot(); break;
                case AttackType.Summon: PerformSummon(); break;
            }
        }
    }

    // --- SLASH ---
    void PerformSlash()
    {
        if (isDead) return; // Safety check

        Vector3 slashPos = selfTransform.position + (Vector3.left * slashOffsetX);

        if (slashVisualPrefab) 
        {
            GameObject slashObj = Instantiate(slashVisualPrefab, slashPos, Quaternion.identity);
            Destroy(slashObj, slashDuration);
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(slashPos, slashRadius, playerLayer);
        if (hitPlayer != null)
        {
            Debug.Log("SLASH HIT SOMETHING!");
        }
    }

    // --- SHOOT ---
    void PerformShoot()
    {
        if (isDead) return;

        if (useShotgunPattern)
        {
            SpawnProjectile(0);   
            SpawnProjectile(15);  
            SpawnProjectile(-15); 
        }
        else
        {
            SpawnProjectile(0);
        }
    }

    void SpawnProjectile(float angleOffset)
    {
        if (projectilePrefab == null) return;

        Vector2 direction = Vector2.left; 

        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float finalAngle = baseAngle + angleOffset;
        Quaternion rotation = Quaternion.Euler(0, 0, finalAngle);

        GameObject bullet = Instantiate(projectilePrefab, selfTransform.position, rotation);
        Destroy(bullet, bulletLifetime);

        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D myCol = GetComponent<Collider2D>();
        if (bulletCol != null && myCol != null) Physics2D.IgnoreCollision(bulletCol, myCol);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = bullet.transform.right * projectileSpeed;
        }
    }

    // --- SUMMON ---
    void PerformSummon()
    {
        if (isDead) return;
        if (minionPrefab == null || summonSpawnPoints.Length == 0) return;

        int randomIndex = Random.Range(0, summonSpawnPoints.Length);
        Instantiate(minionPrefab, summonSpawnPoints[randomIndex].position, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 gizmoPos = transform.position + (Vector3.left * slashOffsetX);
        Gizmos.DrawWireSphere(gizmoPos, slashRadius);
    }
}