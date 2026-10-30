using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public float verticalOffset = 0f;
    public float minY = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        float targetY = Mathf.Max(target.position.y + verticalOffset, minY);

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}