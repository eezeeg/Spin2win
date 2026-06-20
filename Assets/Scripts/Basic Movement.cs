using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BasicMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 9f;
    [SerializeField] private float groundCheckDistance = 0.45f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Feel")]
    [SerializeField] private float fallMultiplier = 3.5f;
    [SerializeField] private float lowJumpMultiplier = 2.5f;

    [Header("Custom Gravity")]
    [SerializeField] private float gravityStrength = 25f;
    [SerializeField] private float maxFallSpeed = 35f;

    [Header("Wall Fix")]
    [SerializeField] private float wallCheckDistance = 0.45f;
    [SerializeField] private LayerMask wallLayer;

    [Header("Maze / Camera Rotation")]
    [SerializeField] private Transform mazeToRotate;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    private Rigidbody rb;
    private float targetRotation;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

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

    private Vector3 GetGravityDirection()
    {
        if (Camera.main == null)
            return Vector3.down;

        return -Camera.main.transform.up.normalized;
    }

    private Vector3 GetJumpDirection()
    {
        return -GetGravityDirection();
    }

    private Vector3 GetMoveDirection()
    {
        if (Camera.main == null)
            return Vector3.right;

        return Camera.main.transform.right.normalized;
    }

    private void CheckGrounded()
    {
        Vector3 gravityDirection = GetGravityDirection();

        isGrounded = Physics.CheckSphere(
            transform.position + gravityDirection * groundCheckDistance,
            groundCheckRadius,
            groundLayer
        );
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 moveDirection = GetMoveDirection();

        bool pushingLeftWall = horizontal < 0f &&
            Physics.Raycast(transform.position, -moveDirection, wallCheckDistance, wallLayer);

        bool pushingRightWall = horizontal > 0f &&
            Physics.Raycast(transform.position, moveDirection, wallCheckDistance, wallLayer);

        if (pushingLeftWall || pushingRightWall)
        {
            horizontal = 0f;
        }

        Vector3 velocity = rb.linearVelocity;

        Vector3 currentMoveVelocity = Vector3.Project(velocity, moveDirection);
        Vector3 wantedMoveVelocity = moveDirection * horizontal * moveSpeed;

        velocity += wantedMoveVelocity - currentMoveVelocity;

        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        if (!isGrounded)
            return;

        Vector3 gravityDirection = GetGravityDirection();
        Vector3 jumpDirection = GetJumpDirection();

        Vector3 velocity = rb.linearVelocity;
        velocity -= Vector3.Project(velocity, gravityDirection);

        rb.linearVelocity = velocity;

        rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);
    }

    private void ApplyCustomGravity()
    {
        Vector3 gravityDirection = GetGravityDirection();

        float fallingSpeed = Vector3.Dot(rb.linearVelocity, gravityDirection);

        float multiplier = 1f;

        if (fallingSpeed > 0f)
        {
            multiplier = fallMultiplier;
        }
        else if (!Input.GetKey(KeyCode.Space))
        {
            multiplier = lowJumpMultiplier;
        }

        rb.AddForce(
            gravityDirection * gravityStrength * multiplier,
            ForceMode.Acceleration
        );

        fallingSpeed = Vector3.Dot(rb.linearVelocity, gravityDirection);

        if (fallingSpeed > maxFallSpeed)
        {
            Vector3 excessFallVelocity = gravityDirection * (fallingSpeed - maxFallSpeed);
            rb.linearVelocity -= excessFallVelocity;
        }
    }

    private void HandleRotation()
    {
        if (mazeToRotate == null)
            return;

        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetRotation += scroll * rotationSpeed;
        }

        Quaternion targetRot = Quaternion.AngleAxis(
            targetRotation,
            rotationAxis.normalized
        );

        mazeToRotate.rotation = Quaternion.Slerp(
            mazeToRotate.rotation,
            targetRot,
            Time.deltaTime * 8f
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 gravityDirection = Camera.main != null
            ? -Camera.main.transform.up.normalized
            : Vector3.down;

        Vector3 moveDirection = Camera.main != null
            ? Camera.main.transform.right.normalized
            : Vector3.right;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position + gravityDirection * groundCheckDistance,
            groundCheckRadius
        );

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            transform.position,
            transform.position + gravityDirection * 2f
        );

        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            transform.position,
            transform.position + moveDirection * wallCheckDistance
        );
        Gizmos.DrawLine(
            transform.position,
            transform.position - moveDirection * wallCheckDistance
        );
    }
}