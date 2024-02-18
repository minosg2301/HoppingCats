using Doozy.Engine.Progress;
using UnityEngine;
using UnityEngine.UI;

public class OptimizedProgressTargetImage : ProgressTarget
{
    public Image slideImage;
    public float width;
    public float maxLength;
    public float minLength;
    public TargetProgress TargetProgress;

    private RectTransform _imageRectTranform;
    public RectTransform ImageRectTransform { get { if (!_imageRectTranform) _imageRectTranform = slideImage.GetComponent<RectTransform>(); return _imageRectTranform; } }


    bool initialized;
    float minThreshold;
    float ratioMinify;

    private void Reset() { UpdateReference(); }

    private void UpdateReference()
    {
        if (slideImage == null)
            slideImage = GetComponent<Image>();
    }

    void Start()
    {
        if (!initialized)
            Init();
    }

    private void Init()
    {
        initialized = true;
        minThreshold = minLength / maxLength;
        ratioMinify = minLength / width;
    }

    public override void UpdateTarget(Progressor progressor)
    {
        base.UpdateTarget(progressor);

        if (!slideImage) return;
        if (!initialized) Init();

        var progressValue = TargetProgress == TargetProgress.Progress
            ? progressor.Progress : progressor.InverseProgress;

        var sizeDelta = ImageRectTransform.sizeDelta;
        sizeDelta.x = progressValue * maxLength;
        if (progressValue < minThreshold)
            sizeDelta.y = sizeDelta.x / ratioMinify;
        else
            sizeDelta.y = width;

        ImageRectTransform.sizeDelta = sizeDelta;
    }
}