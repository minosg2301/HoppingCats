using UnityEngine;

public static class Vector4Ext
{
    public static Vector3 ToXYZ(this Vector4 v) => new Vector3(v.x, v.y, v.z);
}

public static class Vector3Ext
{
    public static float SqrDistance(this Vector3 from, Vector3 to) => Mathf.Abs((from - to).sqrMagnitude);
    public static float Distance(this Vector3 from, Vector3 to) => Vector3.Distance(from, to);
    public static float AngleY(this Vector3 from, Vector3 to)
    {
        Vector3 diff = to - from; diff.Normalize();
        return Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
    }
}

public static class Vector2Ext
{
    public static float SqrDistance(this Vector2 from, Vector2 to) => Mathf.Abs((from - to).sqrMagnitude);
    public static float Distance(this Vector2 from, Vector2 to) => Mathf.Abs((from - to).magnitude);
    public static float AngleY(this Vector2 from, Vector2 to)
    {
        Vector3 diff = to - from; diff.Normalize();
        return Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
    }

    public static Vector3 ToVector3(this Vector2 from, float z)
    {
        return new Vector3(from.x, from.y, z);
    }
}