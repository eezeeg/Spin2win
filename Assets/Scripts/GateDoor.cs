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

        Vector3 localPosition = doorVisual.localPosition;
        localPosition.y += currentVelocityY * Time.deltaTime;

        if (localPosition.y <= minLocalY)
        {
            localPosition.y = minLocalY;
            currentVelocityY = 0f;
        }
        else if (localPosition.y >= maxLocalY)
        {
            localPosition.y = maxLocalY;
            currentVelocityY = 0f;
        }

        doorVisual.localPosition = localPosition;
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