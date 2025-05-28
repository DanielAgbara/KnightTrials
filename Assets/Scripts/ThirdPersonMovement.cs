using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Mouse Rotation Settings")]
    public float mouseSensitivity = 2f;

    [Header("Camera Reference")]
    public Transform cameraTransform; // Assign to FreeLook Camera

    private Rigidbody rb;
    private bool isGrounded;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Handle rotation with smoothing
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        Quaternion newRotation = Quaternion.Euler(0f, transform.eulerAngles.y + mouseX, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 10f * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("IsJumping", true);
        }

        // Toggle mouse lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                ? CursorLockMode.None
                : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }

        // Snap to camera
        if (Input.GetKeyDown(KeyCode.R))
        {
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            if (camForward.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(camForward);
                transform.rotation = lookRotation;
            }
        }

        // Animation speed control
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float inputMagnitude = Mathf.Clamp01(new Vector3(horizontal, 0, vertical).magnitude);
        animator.SetFloat("Speed", inputMagnitude);
    }

    void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        // Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection = moveDirection.normalized;

        if (moveDirection != Vector3.zero)
        {
            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;
            rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
        }
    }
}
