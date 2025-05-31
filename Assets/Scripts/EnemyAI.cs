using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target Settings")]
    public string targetTag = "Knight";
    public float viewRadius = 15f;
    [Range(0, 360)] public float viewAngle = 120f;

    [Header("Combat Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("Patrol Settings")]
    public float patrolDistance = 5f;
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;
    public float walkIdleDelay = 2f;

    [Header("Layer Masks")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private Rigidbody rb;
    private Animator animator;
    private Transform target;
    private Vector3 patrolStart;
    private Vector3 patrolEnd;
    private bool goingForward = true;
    private float idleUntilTime;
    private float lastAttackTime;
    private float lostSightTimer;
    private float lostSightThreshold = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        patrolStart = transform.position;
        patrolEnd = transform.position + transform.forward * patrolDistance;
        idleUntilTime = Time.time;
    }

    void Update()
    {
        target = FindVisibleTarget();

        if (target != null)
        {
            EngageTarget();
        }
        else
        {
            lostSightTimer += Time.deltaTime;
            if (lostSightTimer >= lostSightThreshold)
            {
                Patrol();
            }
        }
    }

    void EngageTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            animator.SetFloat("Speed", 0f);
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            MoveEnemy(direction * runSpeed);
            animator.SetFloat("Speed", 2f);
        }

        RotateTo(direction);
        lostSightTimer = 0f;
    }

    void Patrol()
    {
        if (Time.time < idleUntilTime)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector3 destination = goingForward ? patrolEnd : patrolStart;
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0;

        float distance = Vector3.Distance(transform.position, destination);
        if (distance < 0.3f)
        {
            goingForward = !goingForward;
            idleUntilTime = Time.time + walkIdleDelay;
        }
        else
        {
            MoveEnemy(direction * walkSpeed);
            animator.SetFloat("Speed", 0.5f);
            RotateTo(direction);
        }
    }

    void MoveEnemy(Vector3 velocity)
    {
        rb.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    void RotateTo(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
        }
    }

    Transform FindVisibleTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider col in colliders)
        {
            if (!col.CompareTag(targetTag)) continue;

            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            float distance = Vector3.Distance(transform.position, col.transform.position);

            if (angle < viewAngle / 2f && !Physics.Raycast(transform.position, dirToTarget, distance, obstacleMask))
                return col.transform;
        }

        return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }
}
