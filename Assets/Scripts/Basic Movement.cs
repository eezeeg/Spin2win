using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Feel")]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Custom Gravity")]
    [SerializeField] private float startGravity = -10f;
    [SerializeField] private float gravityIncreasePerSecond = -20f;
    [SerializeField] private float maxFallSpeed = -35f;

    [Header("Wall Fix")]
    [SerializeField] private float wallCheckDistance = 0.45f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Maze Rotation")]
    [SerializeField] private Transform mazeToRotate;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    private Rigidbody rb;
    private float targetRotation;
    private float currentGravity;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        currentGravity = startGravity;

        if (mazeToRotate != null)
        {
            targetRotation = mazeToRotate.eulerAngles.z;
        }
    }

    private void Update()
    {
        CheckGrounded();
        HandleJump();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        ApplyCustomGravity();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(
            transform.position - new Vector3(0f, 0.4f, 0f),
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            currentGravity = startGravity;
        }
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        bool pushingLeftWall = horizontal < 0f && Physics.Raycast(transform.position, Vector3.left, wallCheckDistance, wallLayer);
        bool pushingRightWall = horizontal > 0f && Physics.Raycast(transform.position, Vector3.right, wallCheckDistance, wallLayer);

        if (pushingLeftWall || pushingRightWall)
        {
            horizontal = 0f;
        }

        Vector3 velocity = rb.linearVelocity;
        velocity.x = horizontal * moveSpeed;
        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void ApplyCustomGravity()
    {
        Vector3 velocity = rb.linearVelocity;

        // Falling
        if (velocity.y < 0)
        {
            velocity.y += startGravity * fallMultiplier * Time.fixedDeltaTime;
        }
        // Rising but jump button released
        else if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            velocity.y += startGravity * lowJumpMultiplier * Time.fixedDeltaTime;
        }
        // Normal ascent
        else
        {
            velocity.y += startGravity * Time.fixedDeltaTime;
        }

        velocity.y = Mathf.Max(velocity.y, maxFallSpeed);

        rb.linearVelocity = velocity;
    }

    private void HandleRotation()
    {

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetRotation += scroll * rotationSpeed;
        }

        Quaternion targetRot = Quaternion.AngleAxis(targetRotation, rotationAxis.normalized);

        mazeToRotate.rotation = Quaternion.Slerp(
            mazeToRotate.rotation,
            targetRot,
            Time.deltaTime * 8f
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0f, 0.4f, 0f), groundCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
    }
}