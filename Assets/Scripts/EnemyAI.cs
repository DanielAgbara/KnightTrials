using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class SimpleEnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed;
    public float moveDistance;
    public float idleDuration;
    public float stuckCheckTime;
    public float stuckDistanceThreshold;

    [Header("Field of View Settings")]
    public float viewRadius = 6f;
    [Range(0, 360)]
    public float viewAngle = 120f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    private float lastAttackTime = -Mathf.Infinity;
    private bool useAltAttack = false;

    private Rigidbody rb;
    private Animator animator;

    private Vector3 startPos;
    private Vector3 moveDirection = Vector3.right;
    private bool isIdling = false;
    private float idleTimer = 0f;
    private float movedDistance = 0f;

    private float stuckTimer = 0f;
    private Vector3 previousCheckedPosition;
    private bool awaitingStuckCheck = true;

    private bool playerDetected = false;
    private Transform playerTransform;

    private bool isDead = false;  // 🛑 Flag to lock out all behavior after death

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        startPos = transform.position;
        previousCheckedPosition = startPos;

        FaceDirection(moveDirection);
    }

    void FixedUpdate()
    {
        if (isDead) return;  // ⛔ Stop all behavior if dead

        DetectPlayer();

        if (playerDetected)
        {
            float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distToPlayer <= attackRange)
                AttackPlayer();
            else
                ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // ========== PLAYER DETECTION ==========
    void DetectPlayer()
    {
        if (isDead) return;

        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        foreach (Collider target in targetsInRadius)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (angleToTarget < viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, dirToTarget, distanceToTarget, obstacleMask))
                {
                    playerDetected = true;
                    playerTransform = target.transform;
                    return;
                }
            }
        }

        playerDetected = false;
        playerTransform = null;
    }

    // ========== PATROL BEHAVIOR ==========
    void Patrol()
    {
        if (isDead) return;

        if (isIdling)
        {
            animator.SetFloat("Speed", 0f);
            idleTimer += Time.fixedDeltaTime;

            if (idleTimer >= idleDuration)
            {
                isIdling = false;
                idleTimer = 0f;

                moveDirection = -moveDirection;
                FaceDirection(moveDirection);

                startPos = transform.position;
                movedDistance = 0f;

                awaitingStuckCheck = true;
                stuckTimer = 0f;
                previousCheckedPosition = transform.position;
            }
            return;
        }

        Vector3 move = moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
        animator.SetFloat("Speed", moveSpeed > 0.1f ? 0.5f : 0f);

        movedDistance = Vector3.Distance(startPos, transform.position);
        if (movedDistance >= moveDistance)
        {
            isIdling = true;
            idleTimer = 0f;
            return;
        }

        if (awaitingStuckCheck)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer >= stuckCheckTime)
            {
                float distanceMoved = Vector3.Distance(transform.position, previousCheckedPosition);
                Debug.DrawRay(transform.position, Vector3.up * 2, Color.red);

                if (distanceMoved <= stuckDistanceThreshold)
                {
                    moveDirection = -moveDirection;
                    FaceDirection(moveDirection);

                    startPos = transform.position;
                    movedDistance = 0f;
                }

                stuckTimer = 0f;
                previousCheckedPosition = transform.position;
            }
        }
    }

    // ========== CHASE BEHAVIOR ==========
    void ChasePlayer()
    {
        if (isDead || playerTransform == null) return;

        Vector3 dir = (playerTransform.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        animator.SetFloat("Speed", 2f);
        FaceDirection(dir);
    }

    // ========== ATTACK BEHAVIOR ==========
    void AttackPlayer()
    {
        if (isDead) return;

        animator.SetFloat("Speed", 0f);

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (useAltAttack)
                animator.SetTrigger("Attack2");
            else
                animator.SetTrigger("Attack");

            useAltAttack = !useAltAttack;
            lastAttackTime = Time.time;
        }

        FaceDirection(playerTransform.position - transform.position);
    }

    // ========== DAMAGE APPLICATION ==========
    public void DealDamageToPlayer()
    {
        if (isDead || playerTransform == null) return;

        Player playerScript = playerTransform.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.ReceiveDamage(10);
        }
    }

    // ========== DEATH (CALLED BY HealthSystem) ==========
    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Lock movement and animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.SetFloat("Speed", 0f);
        }

        // Stop physics movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true; // Stop further physics
        }

        // Disable collider if needed
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Optional: disable all children colliders (e.g., hitboxes)
        foreach (Collider childCol in GetComponentsInChildren<Collider>())
        {
            childCol.enabled = false;
        }

        // Optional: disable scripts or components
        this.enabled = false; // disables SimpleEnemyAI script

        // Destroy after animation delay
        Destroy(gameObject, 3.5f); // ensure this matches animation length
    }


    // ========== ROTATION ==========
    void FaceDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = lookRotation;
        }
    }
}
