using UnityEngine;

namespace moonNest
{
    public abstract class IAPTriggerListener : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            IAPTriggerController.Ins.RegisterListener(this);
        }

        protected virtual void OnDisable()
        {
            IAPTriggerController.Ins?.UnregisterListener(this);
        }

        internal protected abstract void OnIAPTrigger(IAPPackageId iapPackageId);
    }
}