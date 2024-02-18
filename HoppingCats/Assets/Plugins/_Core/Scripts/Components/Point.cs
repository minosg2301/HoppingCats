using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Point : MonoBehaviour
{
    public bool GizmoShowNodes = true;
    public Color color = Color.white;
    public Color radiusColor = Color.cyan;
    public bool useNoise = false;
    public float noiseRadius = 0;
    public bool noiseX = false;
    public bool noiseY = false;
    public bool noiseZ = false;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!GizmoShowNodes) return;

        float size = HandleUtility.GetHandleSize(transform.position) * 0.1f;
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, size);

        if (useNoise)
        {
            Gizmos.color = radiusColor;
            Gizmos.DrawWireSphere(transform.position, noiseRadius);
        }
    }
#endif

    public Vector3 GetPosition()
    {
        Vector3 position = transform.position;
        if (useNoise)
        {
            if (noiseX) position.x += Random.Range(-noiseRadius, noiseRadius);
            if (noiseY) position.y += Random.Range(-noiseRadius, noiseRadius);
            if (noiseZ) position.z += Random.Range(-noiseRadius, noiseRadius);
        }
        return position;
    }

    public float Distance(Point point) => transform.position.Distance(point.transform.position);
}
