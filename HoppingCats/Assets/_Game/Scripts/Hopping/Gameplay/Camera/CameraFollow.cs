using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 0.125f; 
    public Vector3 offset; 
    public float bottomLimit = 0f; 

    public void DoFollow(Transform target)
    {
        this.target = target;
        enabled = true;
    }

    public void DoDisable()
    {
        this.target = null;
        enabled = false;
    }

    void LateUpdate()
    {
        if (target)
        {
            Vector3 desiredPosition = target.position + offset;

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            smoothedPosition.y = Mathf.Max(smoothedPosition.y, bottomLimit);

            smoothedPosition.z = transform.position.z;

            transform.position = smoothedPosition;
        }
    }
}
