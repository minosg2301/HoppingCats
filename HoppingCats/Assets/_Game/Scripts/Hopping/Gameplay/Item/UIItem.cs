using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class UIItem : MonoBehaviour
{
    public SpriteRenderer icon;
    public List<ParticleSystem> tiggerParticles = new();

    public float animationDuration = .5f;

    protected ItemConfig config;
    public virtual void SetData(ItemConfig config)
    {
        this.config = config;
    }

    public virtual void ItemTrigger()
    {
        if(tiggerParticles.Count > 0)
        {
            foreach(var particle in tiggerParticles)
            {
                particle.Play();
            }
        }

        icon.DOFade(0, animationDuration).OnComplete(()=> 
        {
            gameObject.SetActive(false);
        });
    }
}
