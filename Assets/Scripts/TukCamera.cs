using UnityEngine;

public class TukCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 4, -8);

    public float smoothSpeed = 6f;
    public float rotateSpeed = 6f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }
}