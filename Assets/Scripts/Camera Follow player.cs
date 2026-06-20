using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 5f;

    private void LateUpdate()
    {
        if (player == null)
            return;

        Vector3 targetPosition = new Vector3(
            player.position.x,
            player.position.y,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}