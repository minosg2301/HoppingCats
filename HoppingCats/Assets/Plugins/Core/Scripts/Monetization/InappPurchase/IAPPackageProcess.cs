#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace moonNest
{
    public class IAPPackageProcess
    {
        public static PaymentVerifier paymentVerifier;

        public static PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args,
            InStorePackage inStorePackage,
            Action<PurchaseProcessingResult> onSuccessed,
            Action<PurchaseFailureReason> onFailed)
        {
            string productId = args.purchasedProduct.definition.id;
            Debug.Log($"IAPConsumer ProcessPurchase(PurchaseEventArgs {productId})");
            
            PurchaseProcessingResult processingResult = PurchaseProcessingResult.Pending;
            if (GlobalConfig.Ins.verifyPurchasingOnline)
            {
                paymentVerifier.Verify(args.purchasedProduct, (result) =>
                {
                    SendTrackingPurchasing(args.purchasedProduct);
                    if (ConsumeProduct(args.purchasedProduct, inStorePackage, onFailed))
                    {
                        onSuccessed(processingResult);
                    }
                }, onFailed);
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

        private static bool ConsumeProduct(Product purchasedProduct, InStorePackage inStorePackage, Action<PurchaseFailureReason> onFailed)
        {
            var productId = inStorePackage ? inStorePackage.AvailableProductId : purchasedProduct.definition.id;
            var packageDetail = IAPPackageAsset.Ins.Find(productId);
            if (!packageDetail)
            {
                Debug.LogError($"Can not find package with product id {purchasedProduct.definition.id} in IAPPackageAsset");
                onFailed(PurchaseFailureReason.ProductUnavailable);
                return false;
            }

            inStorePackage = inStorePackage ? inStorePackage : UserStore.Ins.Find(packageDetail.id);
            if (!inStorePackage)
            {
                Debug.LogError($"Can not find In Store Package with product id {purchasedProduct.definition.id} in User Store");
                onFailed(PurchaseFailureReason.ProductUnavailable);
                return false;
            }

            ConsumePackage(inStorePackage, purchasedProduct.definition.type);

            return true;
        }

        public static void ConsumePackage(InStorePackage inStorePackage, ProductType type = ProductType.Consumable)
        {
            if (type == ProductType.Consumable
                || type == ProductType.NonConsumable
                || type == ProductType.Subscription)
            {
                var detail = inStorePackage.Detail;
                Debug.Log($"{type} Package: {detail.name} - {detail.productId}");

                // update multiply
                int multiply = 1;
                var group = UserStore.Ins.FindGroup(inStorePackage.GroupId);
                if (group != null && group.Detail.x2FirstBuy && inStorePackage.purchaseCount == 0)
                    multiply = 2;

                RewardConsumer.ConsumeIAPPackage(inStorePackage, multiply);
            }

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
        public abstract void Verify(Product purchasedProduct, Action<PurchaseProcessingResult> onSuccessed, Action<PurchaseFailureReason> onFailed);
    }
}
#endif