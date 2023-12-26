using UnityEngine;
using UnityEngine.Purchasing;

namespace moonNest
{
    public abstract class BaseIAPPurchaseHandler : MonoBehaviour, IPurchaseListener
    {
        protected virtual void OnEnable()
        {
            IAPManager.Ins.AddListener(this);
        }

        protected virtual void OnDisable()
        {
            if (IAPManager.Ins) IAPManager.Ins.RemoveListener(this);
        }

        public abstract void OnPurchaseBegin();
        public abstract void OnPurchaseCompleted(Product product, PurchaseEventArgs args);
        public abstract void OnPurchaseFailed(Product product, PurchaseFailureReason reason);

        public virtual void OnRestorePurchaseBegin() { }
        public virtual void OnRestorePurchaseSuccess(int restoredTransactionCount) { }
        public virtual void OnRestorePurchaseFailed() { }
    }
}