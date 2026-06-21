using UnityEngine;

public class BubbleDoor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gravityReference;
    [SerializeField] private Transform doorVisual;

    [Header("Rail Movement")]
    [SerializeField] private float minLocalY = -2f;
    [SerializeField] private float maxLocalY = 2f;
    [SerializeField] private float gravityAcceleration = 8f;
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float damping = 2f;

    [Header("Options")]
    [SerializeField] private bool invertGravityDirection = false;

    [Header("Sine Wave Mode")]
    [SerializeField] private bool useSineWave = false;
    [SerializeField] private float sineSpeed = 1f;
    [SerializeField] private float sineOffset = 0f;

    private float currentVelocityY;

    private void Start()
    {
        if (doorVisual == null)
        {
            doorVisual = transform;
        }
    }

    private void Update()
    {
        if (gravityReference == null || doorVisual == null)
            return;

        // Sine wave mode
        if (useSineWave)
        {
            Vector3 localPosition = doorVisual.localPosition;

            float t = (Mathf.Sin(Time.time * sineSpeed + sineOffset) + 1f) * 0.5f;

            localPosition.y = Mathf.Lerp(
                minLocalY,
                maxLocalY,
                t
            );

            doorVisual.localPosition = localPosition;
            return;
        }

        // Bubble gravity mode
        Vector3 worldGravityDirection = -gravityReference.up.normalized;

        Vector3 railWorldDirection = doorVisual.parent != null
            ? doorVisual.parent.up.normalized
            : Vector3.up;

        float gravityOnRail = Vector3.Dot(worldGravityDirection, railWorldDirection);

        if (invertGravityDirection)
        {
            gravityOnRail *= -1f;
        }

        currentVelocityY += gravityOnRail * gravityAcceleration * Time.deltaTime;

        currentVelocityY = Mathf.Clamp(
            currentVelocityY,
            -maxMoveSpeed,
            maxMoveSpeed
        );

        currentVelocityY = Mathf.Lerp(
            currentVelocityY,
            0f,
            damping * Time.deltaTime
        );

        Vector3 localPositionGravity = doorVisual.localPosition;
        localPositionGravity.y += currentVelocityY * Time.deltaTime;

        if (localPositionGravity.y <= minLocalY)
        {
            localPositionGravity.y = minLocalY;
            currentVelocityY = 0f;
        }
        else if (localPositionGravity.y >= maxLocalY)
        {
            localPositionGravity.y = maxLocalY;
            currentVelocityY = 0f;
        }

        doorVisual.localPosition = localPositionGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Transform visual = doorVisual != null ? doorVisual : transform;

        Vector3 minPosition = visual.localPosition;
        Vector3 maxPosition = visual.localPosition;

        minPosition.y = minLocalY;
        maxPosition.y = maxLocalY;

        Vector3 minWorldPosition = visual.parent != null
            ? visual.parent.TransformPoint(minPosition)
            : minPosition;

        Vector3 maxWorldPosition = visual.parent != null
            ? visual.parent.TransformPoint(maxPosition)
            : maxPosition;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(minWorldPosition, Vector3.one * 0.25f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(maxWorldPosition, Vector3.one * 0.25f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(minWorldPosition, maxWorldPosition);
    }
}