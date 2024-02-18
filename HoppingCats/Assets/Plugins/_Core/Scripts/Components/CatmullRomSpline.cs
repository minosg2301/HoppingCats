using UnityEngine;
using System.Linq;

//Interpolation between points with a Catmull-Rom spline
[ExecuteInEditMode]
public class CatmullRomSpline : MonoBehaviour
{
    //Has to be at least 4 points
    private Transform[] _controlPoints;
    public Transform[] ControlPoints { get { if (_controlPoints == null) _controlPoints = GetComponentsInChildren<Point>().Select(p => p.transform).ToArray(); return _controlPoints; } }

    //Are we making a line or a loop?
    public bool isLooping = true;
    public float resolution = 0.2f;

    public int MaxStep => Mathf.FloorToInt(1f / resolution);

    //Display without having to press play
    void OnDrawGizmos()
    {
        var _controlPoints = GetComponentsInChildren<Point>().Select(p => p.transform).ToArray();
        if (_controlPoints == null || _controlPoints.Length < 4) return;

        Gizmos.color = Color.white;

        //Draw the Catmull-Rom spline between the points
        for (int i = 0; i < _controlPoints.Length; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if ((i == 0 || i == _controlPoints.Length - 2 || i == _controlPoints.Length - 1) && !isLooping)
            {
                continue;
            }

            GizmosDisplayCatmullRomSpline(_controlPoints, i);
        }
    }

    //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
    void GizmosDisplayCatmullRomSpline(Transform[] controlPoints, int pos)
    {
        int length = controlPoints.Length;
        //The 4 points we need to form a spline between p1 and p2
        Vector3 p0 = controlPoints[ClampListPos(pos - 1, length)].position;
        Vector3 p1 = controlPoints[pos].position;
        Vector3 p2 = controlPoints[ClampListPos(pos + 1, length)].position;
        Vector3 p3 = controlPoints[ClampListPos(pos + 2, length)].position;

        //The start position of the line
        Vector3 lastPos = p1;

        //The spline's resolution
        //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
        //float resolution = 0.2f;

        //How many times should we loop?
        int loops = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= loops; i++)
        {
            //Which t position are we at?
            float t = i * resolution;

            //Find the coordinate between the end points with a Catmull-Rom spline
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

            //Draw this line segment
            Gizmos.DrawLine(lastPos, newPos);

            //Save this pos so we can draw the next line segment
            lastPos = newPos;
        }
    }

    public int GetNextIdx(int idx)
    {
        if (idx == ControlPoints.Length - 1)
        {
            return !isLooping ? -1 : 0;
        }
        else
        {
            return idx + 1;
        }
    }

    public Vector3 GetPosition(int idx, float t)
    {
        if ((idx == 0 || idx == ControlPoints.Length - 2 || idx == ControlPoints.Length - 1) && !isLooping)
        {
            return Vector3.positiveInfinity;
        }

        int length = ControlPoints.Length;

        //The 4 points we need to form a spline between p1 and p2
        Vector3 p0 = ControlPoints[ClampListPos(idx - 1, length)].position;
        Vector3 p1 = ControlPoints[idx].position;
        Vector3 p2 = ControlPoints[ClampListPos(idx + 1, length)].position;
        Vector3 p3 = ControlPoints[ClampListPos(idx + 2, length)].position;

        return GetCatmullRomPosition(t, p0, p1, p2, p3);
    }

    //Clamp the list positions to allow looping
    int ClampListPos(int pos, int length)
    {
        if (pos < 0) pos = length - 1;

        if (pos > length) pos = 1;
        else if (pos > length - 1) pos = 0;

        return pos;
    }

    //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
    //http://www.iquilezles.org/www/articles/minispline/minispline.htm
    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}