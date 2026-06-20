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

    [Header("Maze Rotation")]
    [SerializeField] private Transform mazeToRotate;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;

    private Rigidbody rb;
    private float targetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (mazeToRotate != null)
        {
            targetRotation = mazeToRotate.eulerAngles.z;
        }
    }

    private void Update()
    {
        HandleJump();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 velocity = rb.linearVelocity;
        velocity.x = horizontal * moveSpeed;

        rb.linearVelocity = velocity;
    }

    private void HandleJump()
    {
        bool isGrounded = Physics.CheckSphere(
            transform.position - new Vector3(0f, 0.4f, 0f),
            groundCheckRadius,
            groundLayer
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

        Quaternion targetRot =
            Quaternion.AngleAxis(targetRotation, rotationAxis.normalized);

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
    }
}