using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 0.125f; 
    public Vector3 offset; 
    public float bottomLimit = 0f; 

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        smoothedPosition.y = Mathf.Max(smoothedPosition.y, bottomLimit);

        smoothedPosition.z = transform.position.z;

        transform.position = smoothedPosition;
    }
}
