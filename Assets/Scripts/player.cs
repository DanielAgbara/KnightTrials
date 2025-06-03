using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 20f;
    public float runSpeed = 30f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;

    [Header("Camera Reference")]
    public Transform cameraTransform;

    [Header("Combat Settings")]
    public bool isBlocking = false;  // Public for enemy to check
    public HealthSystem healthSystem;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = -Mathf.Infinity;
    private bool useAltAttack = false;

    private Rigidbody rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
        healthSystem = GetComponent<HealthSystem>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleActions();
        UpdateAnimatorParameters();
        ToggleCursor();
        SnapToCameraDirection();
    }

    void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
    }

    // ------------------ Movement and Rotation ------------------
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        Quaternion newRotation = Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);
    }

    void HandleMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * inputZ + transform.right * inputX;
        moveDirection = moveDirection.normalized;

        float moveAmount = moveDirection.magnitude;
        bool isTryingToMove = moveAmount > 0.1f;
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift);
        bool shouldRun = isTryingToMove && isShiftHeld;

        float speed = shouldRun ? runSpeed : walkSpeed;

        if (isTryingToMove)
        {
            rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        }
    }

    void CheckGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
        Debug.DrawRay(rayOrigin, Vector3.down * (groundCheckDistance + 0.1f), Color.red);
    }

    // ------------------ Player Actions ------------------
    void HandleActions()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("IsJumping");
        }

        // Attack
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            if (useAltAttack)
                animator.SetTrigger("Attack2");
            else
                animator.SetTrigger("Attack");

            useAltAttack = !useAltAttack;
            lastAttackTime = Time.time;
        }

        // Block
        isBlocking = Input.GetMouseButton(1);
        animator.SetBool("IsBlocking", isBlocking);
    }

    // ------------------ Animator Parameters ------------------
    void UpdateAnimatorParameters()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        bool isMoving = new Vector3(inputX, 0, inputZ).magnitude > 0.1f;

        float speedParam = 0f;

        if (isMoving)
        {
            speedParam = Input.GetKey(KeyCode.LeftShift) ? 2.0f : 0.5f;
        }

        animator.SetFloat("Speed", speedParam);
    }

    // ------------------ Utility ------------------
    void ToggleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void SnapToCameraDirection()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            if (camForward.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(camForward);
            }
        }
    }

    // ------------------ External Damage Interface ------------------
    public void ReceiveDamage(int amount)
    {
        if (isBlocking)
        {
            return;
        }

        if (healthSystem != null)
        {
            healthSystem.TakeDamage(amount);
        }
    }

    // =================== ANIMATION EVENT ===================
    public void DealDamageToEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, 20f, LayerMask.GetMask("Enemy"));
        foreach (Collider hit in hits)
        {
            HealthSystem targetHealth = hit.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(40);
            }
        }
    }
}
