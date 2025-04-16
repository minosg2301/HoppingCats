using DG.Tweening;
using UnityEngine;

public class UICloudPlatform : UIPlatform
{
    [Header("Temp Properties")]
    public ParticleSystem particle;
    public SpriteRenderer image;
    public float hideDuration = .3f;

    public override void Trigger()
    {
        base.Trigger();
        DoBreak();
    }

    public void DoBreak()
    {
        image.transform.DOScale(0, hideDuration);
        if (particle) particle.Play();
    }
}
