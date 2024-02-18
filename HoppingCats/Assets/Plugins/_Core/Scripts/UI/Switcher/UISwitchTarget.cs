using UnityEngine;

namespace moonNest
{
    public abstract class UISwitchTarget : MonoBehaviour
    {
        protected bool _on;
        public virtual bool On { get { return _on; } set { _on = value; } }
    }
}