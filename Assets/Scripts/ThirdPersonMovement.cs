using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Sprint Settings")]
    public float maxSprintDuration = 3f;
    public float sprintCooldown = 3f;
    public float sprintRechargeTime = 5f;
    public float minSprintThreshold = 0.5f;
    public Slider sprintSlider;

    [Header("Camera")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private bool isGrounded;
    private float currentSprintTime;
    private float currentCooldown;
    private bool isSprinting;
    private bool isCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentSprintTime = maxSprintDuration;
        UpdateSprintUI();
    }

    void Update()
    {
        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Handle sprint input
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift);
        
        if (wantsToSprint)
        {
            TryStartSprint();
        }
        else
        {
            StopSprint();
        }

        // Force stop sprinting if stamina is depleted
        if (isSprinting && currentSprintTime <= 0)
        {
            StopSprint();
            isCooldown = true;
            currentCooldown = sprintCooldown;
        }

        // Handle cooldown and recharge
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0) 
            {
                isCooldown = false;
            }
        }
        else if (!isSprinting && currentSprintTime < maxSprintDuration)
        {
            currentSprintTime += Time.deltaTime * (maxSprintDuration / sprintRechargeTime);
            currentSprintTime = Mathf.Min(currentSprintTime, maxSprintDuration);
            UpdateSprintUI();
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveDirection = (vertical * cameraForward + horizontal * cameraTransform.right).normalized;

        if (moveDirection != Vector3.zero)
        {
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
            
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    void TryStartSprint()
    {
        // Can't sprint if in cooldown or not enough stamina (now 50%)
        if (isCooldown || currentSprintTime < maxSprintDuration * minSprintThreshold)
        {
            StopSprint();
            return;
        }

        isSprinting = true;
        currentSprintTime -= Time.deltaTime;
        UpdateSprintUI();
    }

    void StopSprint()
    {
        isSprinting = false;
    }

    void UpdateSprintUI()
    {
        if (sprintSlider != null)
        {
            sprintSlider.value = currentSprintTime / maxSprintDuration;
        }
    }
}