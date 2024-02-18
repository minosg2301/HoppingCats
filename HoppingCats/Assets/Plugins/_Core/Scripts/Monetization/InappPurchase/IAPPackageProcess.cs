using System;
using UnityEngine;
using UnityEngine.Purchasing;
using DG.Tweening;
using System.Collections.Generic;

namespace moonNest
{
    public class IAPPackageProcess
    {
        public static PaymentVerifier paymentVerifier;

        public static PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args,
            InStorePackage inStorePackage,
            Action<PurchaseProcessingResult> onSuccessed,
            Action<PurchaseFailureReason, string> onFailed)
        {
            string productId = args.purchasedProduct.definition.id;
            Debug.Log($"IAPConsumer ProcessPurchase(PurchaseEventArgs {productId})");

            PurchaseProcessingResult processingResult = PurchaseProcessingResult.Pending;
            if (GlobalConfig.Ins.verifyPurchasingOnline)
            {
                paymentVerifier.Verify(
                    args.purchasedProduct,
                    (result) =>
                    {
                        if (!result.sandbox)
                        {
                            SendTrackingPurchasing(args.purchasedProduct);
                        }

                        if (ConsumeProduct(args.purchasedProduct, inStorePackage, onFailed))
                        {
                            onSuccessed(processingResult);
                        }
                    },
                    (reason) => onFailed(reason, "Verify Failed"));
            }
            else
            {
                SendTrackingPurchasing(args.purchasedProduct);
                if (ConsumeProduct(args.purchasedProduct, inStorePackage, onFailed))
                {
                    processingResult = PurchaseProcessingResult.Complete;
                    onSuccessed(processingResult);
                }
            }

            return processingResult;
        }

        private static bool ConsumeProduct(Product purchasedProduct, InStorePackage inStorePackage, Action<PurchaseFailureReason, string> onFailed)
        {
            var productId = inStorePackage ? inStorePackage.AvailableProductId : purchasedProduct.definition.id;
            var packageDetail = IAPPackageAsset.Ins.Find(productId);
            if (!packageDetail)
            {
                onFailed(
                    PurchaseFailureReason.ProductUnavailable,
                    $"Can not find package with product id {purchasedProduct.definition.id} in IAPPackageAsset");
                return false;
            }

            inStorePackage = inStorePackage ? inStorePackage : UserStore.Ins.Find(packageDetail.id);
            if (!inStorePackage)
            {
                onFailed(
                    PurchaseFailureReason.ProductUnavailable,
                    $"Can not find In Store Package with product id {purchasedProduct.definition.id} in User Store");
                return false;
            }

            ConsumePackage(inStorePackage, purchasedProduct.definition.type);

            return true;
        }

        public static void ConsumePackage(InStorePackage inStorePackage, ProductType type = ProductType.Consumable)
        {
            var detail = inStorePackage.Detail;
            Debug.Log($"{type} Package: {detail.name} - {detail.productId}");

            // update multiply
            int multiply = 1;
            var group = UserStore.Ins.FindGroup(inStorePackage.GroupId);
            if (group != null && group.Detail.x2FirstBuy && inStorePackage.purchaseCount == 0)
                multiply = 2;

            RewardConsumer.ConsumeIAPPackage(inStorePackage, multiply);

            UserStore.Ins.UpdateBuyPackage(inStorePackage);
        }

        static void SendTrackingPurchasing(Product purchasedProduct)
        {
            string productId = purchasedProduct.definition.id;
            decimal price = purchasedProduct.metadata.localizedPrice;
            string currency = purchasedProduct.metadata.isoCurrencyCode;
            string transactionID = purchasedProduct.transactionID;
        }
    }

    public abstract class PaymentVerifier
    {
        public abstract void Verify(Product purchasedProduct,
            Action<VerifyResult> onSuccessed,
            Action<PurchaseFailureReason> onFailed);
    }

    class DummyVerifier : PaymentVerifier
    {
        public override void Verify(Product purchasedProduct,
            Action<VerifyResult> onSuccessed,
            Action<PurchaseFailureReason> onFailed)
        {
            DOVirtual.DelayedCall(0.3f, () =>
            {
                onSuccessed(VerifyResult.SandboxSuccess);
            });
        }
    }

    [Serializable]
    public class VerifyResult
    {
        public bool result;
        public bool sandbox;

        public static VerifyResult Success => new() { result = true, sandbox = false };
        public static VerifyResult SandboxSuccess => new() { result = true, sandbox = true };
    }
}