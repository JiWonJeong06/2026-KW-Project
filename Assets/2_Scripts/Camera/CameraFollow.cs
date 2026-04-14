using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = Vector3.zero;

    private float fixedZ = -10f;

    void FixedUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        targetPos.z = fixedZ;

        Vector3 smoothPos = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );

        smoothPos.z = fixedZ;

        transform.position = smoothPos;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            Vector3 targetPos = target.position + offset;
            targetPos.z = fixedZ;
            transform.position = targetPos;
        }
    }
}