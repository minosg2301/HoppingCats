using DG.Tweening;
using UnityEngine;

public class CatmullRomSplineController : MonoBehaviour
{
    public float speed = 10;
    public float smoothTime = 1;
    public CatmullRomSpline catmullRomSpline;
    public UpdateType updateType;

    private int maxStep = 0;
    private int idx = 0;
    private int step = 0;
    private Vector3 target = Vector3.positiveInfinity;
    private Vector3 vel;

    private void Awake()
    {
        if (catmullRomSpline != null)
        {
            maxStep = catmullRomSpline.MaxStep;
            UpdateTarget();
        }
    }

    void Update() { if (updateType == UpdateType.Normal) UpdatePosition(); }
    void FixedUpdate() { if (updateType == UpdateType.Fixed) UpdatePosition(); }
    void LateUpdate() { if (updateType == UpdateType.Late) UpdatePosition(); }

    private void UpdatePosition()
    {
        if (target != Vector3.positiveInfinity)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, smoothTime, speed);
            if ((target - transform.position).magnitude < 0.002)
            {
                UpdateTarget();
            }
        }
    }

    public void SetCatmullRomSpline(CatmullRomSpline catmullRomSpline)
    {
        this.catmullRomSpline = catmullRomSpline;
        maxStep = catmullRomSpline.MaxStep;
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        if (++step > maxStep)
        {
            step = 0;
            idx = catmullRomSpline.GetNextIdx(idx);
            if (idx == -1)
                target = Vector3.positiveInfinity;
        }

        target = catmullRomSpline.GetPosition(idx, step / (float)maxStep);
    }
}