using System;
using UnityEngine;

[ExecuteAlways]
public class CameraFacingBillboard : MonoBehaviour
{
    public bool selfFacing;

    [NonSerialized] public new Camera camera;

    public void OnEnable()
    {
        if (camera == null)
            camera = Camera.main;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void Update()
    {
        if (camera == null)
            camera = Camera.current;

        if (camera != null)
        {
            if (selfFacing)
            {
                transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(transform.parent.forward, camera.transform.position - transform.position);
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
    }
#endif
}