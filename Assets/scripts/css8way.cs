using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Screen Bounds & Bounce")]
    [Tooltip("Distance from the center of the player to the edge of the sprite.")]
    public float playerRadius = 0.5f; 
    [Tooltip("How strong the bounce back is (0 = Stop at wall, 1 = Full bounce).")]
    public float bounceFactor = 0.5f; 

    [Header("Control Keys")]
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;

    [Header("Animation")]
    public Animator animator;
    public string walkBoolParam = "IsWalking";

    [Header("Trail Spawner")]
    public GameObject dustPrefab; 
    public float distanceBetweenSpawns = 0.5f; 
    public float rotationOffset = 180f;

    [Header("Dust Removal Settings")]
    public float dustLifeTime = 1.0f; 
    public bool clearDustOnStateChange = true;

    // Internal variables
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private Vector3 lastSpawnPosition;
    private Vector2 lastMoveDirection;
    
    private List<GameObject> activeDustList = new List<GameObject>();

    // Cache Camera to save performance
    private Camera mainCamera;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        lastSpawnPosition = transform.position;
    }

    void Update()
    {
        ProcessInputs();
        UpdateAnimation();
        HandleTrailSpawning();
        HandleDustCleanup();
    }

    void FixedUpdate()
    {
        Move();
        // --- NEW: Check bounds after moving ---
        KeepPlayerOnScreen();
    }

    void ProcessInputs()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(moveRight)) moveX = 1f;
        if (Input.GetKey(moveLeft)) moveX = -1f;
        if (Input.GetKey(moveUp)) moveY = 1f;
        if (Input.GetKey(moveDown)) moveY = -1f;

        moveDirection = new Vector2(moveX, moveY);
        if (moveDirection.magnitude > 1) moveDirection = moveDirection.normalized;
    }

    void Move()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    // --- NEW: Screen Restriction Logic ---
    void KeepPlayerOnScreen()
    {
        if (mainCamera == null) return;

        // 1. Calculate the edges of the screen in World Coordinates
        Vector2 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector2 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        Vector3 pos = transform.position;
        Vector2 currentVel = rb.linearVelocity;
        bool bouncedX = false;
        bool bouncedY = false;

        // 2. Check Horizontal Bounds (Left/Right)
        if (pos.x < minScreenBounds.x + playerRadius)
        {
            pos.x = minScreenBounds.x + playerRadius;
            bouncedX = true;
        }
        else if (pos.x > maxScreenBounds.x - playerRadius)
        {
            pos.x = maxScreenBounds.x - playerRadius;
            bouncedX = true;
        }

        // 3. Check Vertical Bounds (Bottom/Top)
        if (pos.y < minScreenBounds.y + playerRadius)
        {
            pos.y = minScreenBounds.y + playerRadius;
            bouncedY = true;
        }
        else if (pos.y > maxScreenBounds.y - playerRadius)
        {
            pos.y = maxScreenBounds.y - playerRadius;
            bouncedY = true;
        }

        // 4. Apply clamped position
        transform.position = pos;

        // 5. Apply Bounce (Reverse Velocity)
        // If we hit a wall, we invert the velocity on that axis to "Bounce"
        if (bouncedX)
        {
            currentVel.x = -currentVel.x * bounceFactor;
            rb.linearVelocity = currentVel;
        }
        if (bouncedY)
        {
            currentVel.y = -currentVel.y * bounceFactor;
            rb.linearVelocity = currentVel;
        }
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(walkBoolParam, moveDirection.sqrMagnitude > 0);
        }
    }

    void HandleTrailSpawning()
    {
        if (dustPrefab == null) return;

        float dist = Vector3.Distance(transform.position, lastSpawnPosition);

        if (dist >= distanceBetweenSpawns && moveDirection.sqrMagnitude > 0)
        {
            SpawnDust();
            lastSpawnPosition = transform.position;
        }
    }

    void SpawnDust()
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0, 0, angle + rotationOffset);

        GameObject newDust = Instantiate(dustPrefab, transform.position, rot);
        activeDustList.Add(newDust);
        Destroy(newDust, dustLifeTime);
    }

    void HandleDustCleanup()
    {
        if (!clearDustOnStateChange) return;

        activeDustList.RemoveAll(item => item == null);

        bool isIdle = moveDirection.sqrMagnitude == 0;
        bool isTurning = false;

        if (!isIdle && lastMoveDirection.sqrMagnitude > 0)
        {
            float angleDiff = Vector2.Angle(lastMoveDirection, moveDirection);
            if (angleDiff > 90f) isTurning = true;
        }

        if (isIdle || isTurning)
        {
            foreach (GameObject dust in activeDustList)
            {
                if (dust != null) Destroy(dust);
            }
            activeDustList.Clear();
        }

        lastMoveDirection = moveDirection;
    }
}