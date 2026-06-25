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
    [SerializeField] private float coyoteTime = 0.1f;

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

    [Header("Auto Rotate")]
    [SerializeField] private bool autoRotate = false;
    [SerializeField] private float autoRotateSpeed = 15f;

    [SerializeField] private Transform oppositeRotationObject;

    private Rigidbody rb;
    private float targetRotation;
    private bool isGrounded;

    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    private float coyoteTimeCounter;
    private bool jumpedOfGround;

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
        if (PauseMenu.IsPaused || SettingsMenu.SettingsOpen)
        {
            return;
        }

        CheckGrounded();
        HandleJump();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        if (PauseMenu.IsPaused || SettingsMenu.SettingsOpen)
        {
            return;
        }

        HandleMovement();
        ApplyCustomGravity();
    }

    private void LateUpdate()
    {
        if (PauseMenu.IsPaused || SettingsMenu.SettingsOpen)
        {
            return;
        }

        if (currentPlatform != null)
        {
            Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
            transform.position += platformDelta;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    private Vector3 GetGravityDirection()
    {
        if (Camera.main == null)
            return Vector3.down;

        return -Camera.main.transform.up.normalized;
    }

    public void ResetRotation()
    {
        targetRotation = 0f;

        if (mazeToRotate != null)
            mazeToRotate.rotation = Quaternion.identity;

        if (oppositeRotationObject != null)
            oppositeRotationObject.rotation = Quaternion.identity;
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

        if (isGrounded)
        {
            jumpedOfGround = false;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        float horizontal = 0f;

        if (Input.GetKey(SettingsMenu.LeftKey))
            horizontal -= 1f;

        if (Input.GetKey(SettingsMenu.RightKey))
            horizontal += 1f;

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
        if (!Input.GetKeyDown(SettingsMenu.JumpKey))
            return;

        if ((coyoteTimeCounter <= 0f) || jumpedOfGround)
            return;

        Vector3 gravityDirection = GetGravityDirection();
        Vector3 jumpDirection = GetJumpDirection();

        Vector3 velocity = rb.linearVelocity;
        velocity -= Vector3.Project(velocity, gravityDirection);

        rb.linearVelocity = velocity;

        rb.AddForce(jumpDirection * jumpForce, ForceMode.Impulse);

        jumpedOfGround = true;
        coyoteTimeCounter = 0f;
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
        else if (!Input.GetKey(SettingsMenu.JumpKey))
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

        if (autoRotate)
        {
            targetRotation += autoRotateSpeed * Time.deltaTime;
        }
        else
        {
            float scroll = Input.mouseScrollDelta.y * SettingsMenu.ScrollSensitivity;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                targetRotation += scroll * rotationSpeed;
            }
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

        // Rotate another object in the opposite direction
        if (oppositeRotationObject != null)
        {
            oppositeRotationObject.rotation = mazeToRotate.rotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.transform;
            lastPlatformPosition = currentPlatform.position;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform == currentPlatform)
        {
            currentPlatform = null;
        }
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