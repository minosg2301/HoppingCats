using UnityEngine;

namespace moonNest
{
    public class TouchPlane : SingletonMono<TouchPlane>
    {
        protected override void Awake()
        {
            base.Awake();

            float size = 40f;

            var boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(size, 0.1f, size);
        }
    }
}