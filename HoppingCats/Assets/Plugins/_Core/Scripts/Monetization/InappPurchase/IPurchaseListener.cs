using UnityEngine.Purchasing;

namespace moonNest
{
    public interface IPurchaseListener
    {
        void OnPurchaseBegin();
        void OnPurchaseFailed(Product product, PurchaseFailureReason reason);
        void OnPurchaseCompleted(Product product, PurchaseEventArgs args);

        void OnRestorePurchaseBegin();
        void OnRestorePurchaseFailed();
        void OnRestorePurchaseSuccess(int restoredTransactionCount);
    }
}