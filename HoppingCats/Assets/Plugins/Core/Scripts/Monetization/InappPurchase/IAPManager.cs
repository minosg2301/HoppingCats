using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace moonNest
{
    public class IAPManager : SingletonMono<IAPManager>, IStoreListener
    {
        public IStoreController StoreController { get; private set; }
        public IExtensionProvider ExtensionProvider { get; private set; }

        public ITransactionHistoryExtensions TransactionHistoryExtensions { get; private set; }
        public IAppleExtensions AppleExtensions { get; private set; }
        public IMicrosoftExtensions MicrosoftExtensions { get; private set; }
        public IGooglePlayStoreExtensions GooglePlayStoreExtensions { get; private set; }
        public bool InitializationComplete { get; private set; }
        public bool IsGooglePlayStoreSelected { get; private set; }
        public bool PurchaseRestoring { get; private set; } = false;


        public int RestoredTransactionCount { get; private set; } = 0;
        public int RestoredTransactionSuccess { get; private set; } = 0;

        InStorePackage processingPackage;
        bool purchaseInProgress = false;
        List<SubscriptionInfo> subscriptions;

        readonly List<IPurchaseListener> listeners = new List<IPurchaseListener>();

        [NonSerialized] public UnityEvent onInitialized = new UnityEvent();

        #region initialization
        public void Init()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer) return;

            var module = StandardPurchasingModule.Instance();
            IsGooglePlayStoreSelected = Application.platform == RuntimePlatform.Android && module.appStore == AppStore.GooglePlay;

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
            if (Debug.isDebugBuild)
            {
                builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
                // Write out our Amazon Sandbox JSON file.
                // This has no effect when the Amazon billing service is not in use.
                builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);
            }

            if (GlobalConfig.Ins.appStorePromotionPurchase)
            {
                // On iOS and tvOS we can intercept promotional purchases that come directly from the App Store.
                // On other platforms this will have no effect; OnPromotionalPurchase will never be called.
                builder.Configure<IAppleConfiguration>().SetApplePromotionalPurchaseInterceptorCallback(OnPromotionalPurchase);
                Debug.Log("Setting Apple promotional purchase interceptor callback");
            }

            // Populate IAP Packages
            var packages = IAPPackageAsset.Ins.Packages;
            foreach (var package in packages)
            {
                builder.AddProduct(package.productId, package.type);
                if (package.promotionProductId.Length > 0)
                    builder.AddProduct(package.promotionProductId, package.type);
            }

            if (packages.Count > 0)
                UnityPurchasing.Initialize(Ins, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            InitializationComplete = true;
            StoreController = controller;
            ExtensionProvider = extensions;
            AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            MicrosoftExtensions = extensions.GetExtension<IMicrosoftExtensions>();
            TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
            GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();

            // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
            // On non-Apple platforms this will have no effect; OnDeferred will never be called.
            AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

            foreach (var item in controller.products.all)
            {
                if (item.availableToPurchase)
                {
                    if (GlobalConfig.Ins.appStorePromotionPurchase)
                    {
                        AppleExtensions.SetStorePromotionVisibility(item, AppleStorePromotionVisibility.Show);
                    }
                }
            }

            GetSubscriptionInfos();

            onInitialized.Invoke();
        }

        #region subscriptions
        void GetSubscriptionInfos()
        {
            subscriptions = new List<SubscriptionInfo>();
#if UNITY_IOS
            Dictionary<string, string> introductory_info_dict = AppleExtensions.GetIntroductoryPriceDictionary();
#endif
            Product[] purchasedProducts = StoreController.products.all;
            foreach (Product item in purchasedProducts)
            {
                if (item.receipt != null)
                {
                    if (item.definition.type == ProductType.Subscription)
                    {
                        if (CheckIfProductIsAvailableForSubscriptionManager(item.receipt))
                        {
                            string intro_json = null;
#if UNITY_IOS
                            intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
#endif
                            SubscriptionManager p = new SubscriptionManager(item, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            subscriptions.Add(info);
                        }
                    }
                }
            }
        }

        bool CheckIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null)
            {
                switch (store)
                {
                    case GooglePlay.Name:
                        {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json"))
                            {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name:
                        {
                            return true;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
            return false;
        }
        #endregion

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError(string.Format("Purchasing failed to initialize. Reason: {0}", error.ToString()));
        }
        #endregion

        #region helper
        public Product GetProduct(string productId)
        {
            if (StoreController != null && StoreController.products != null && !string.IsNullOrEmpty(productId))
            {
                return StoreController.products.WithID(productId);
            }
            //Debug.LogError("IAPManager attempted to get unknown product " + productId);
            return null;
        }

        public SubscriptionInfo GetSubscriptionInfo(string productId)
        {
            return subscriptions.Find(s => s.getProductId() == productId);
        }

        public void AddListener(IPurchaseListener listener) => listeners.Add(listener);

        public void RemoveListener(IPurchaseListener listener) => listeners.Remove(listener);
        #endregion

        #region perform purchase and restore
        public void DoPurchase(InStorePackage purchasePackage)
        {
            if (purchaseInProgress)
            {
                Debug.Log("Please wait, purchase in progress");
                return;
            }

            string productId = purchasePackage.Detail.productId;
            processingPackage = purchasePackage;
            DoPurchase(productId);
        }

        public void DoPurchase(string productId)
        {
            if (StoreController.products.WithID(productId) == null)
            {
                Debug.LogError("No product has id " + productId);
                return;
            }

            purchaseInProgress = true;
            foreach (var listener in listeners)
                listener.OnPurchaseBegin();
            StoreController.InitiatePurchase(productId);
        }

        public void RestoreTransaction()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                     Application.platform == RuntimePlatform.OSXPlayer ||
                     Application.platform == RuntimePlatform.tvOS)
            {
                RestoredTransactionCount = 0;
                PurchaseRestoring = true;
                foreach (var listener in listeners)
                    listener.OnRestorePurchaseBegin();
                AppleExtensions.RestoreTransactions(OnTransactionsRestored);
            }
            else
            {
                Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
            }
        }

#if !UNITY_WEBGL
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("ProcessPurchase: " + e.purchasedProduct.definition.id);
            //Debug.Log("Receipt: " + e.purchasedProduct.receipt);

            var productId = e.purchasedProduct.definition.id;
            var _package = processingPackage;
            processingPackage = null;

            if (PurchaseRestoring)
            {
                RestoredTransactionCount++;
                if (e.purchasedProduct.definition.type != ProductType.NonConsumable)
                {
                    return PurchaseProcessingResult.Complete;
                }
            }

            PurchaseProcessingResult result;
            result = IAPPackageProcess.ProcessPurchase(e, _package,
            (processingResult) =>
            {
                foreach (var listener in listeners)
                    listener.OnPurchaseCompleted(e.purchasedProduct, e);

                purchaseInProgress = false;

                if (processingResult == PurchaseProcessingResult.Pending)
                    StoreController.ConfirmPendingPurchase(e.purchasedProduct);

                if (e.purchasedProduct.definition.type == ProductType.Subscription)
                {
                    GetSubscriptionInfos();
                }
            },
            (reason) => OnPurchaseFailed(e.purchasedProduct, reason));

            return result;
        }
#else
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.LogError("IAP not support on webGL");
            return PurchaseProcessingResult.Pending;
        }
#endif

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            Debug.Log("Purchase failed: " + product.definition.id);
            Debug.Log(reason);

            // Detailed debugging information
            Debug.Log("Store specific error code: " + TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
            if (TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
            {
                Debug.Log($"Purchase failure description message: {TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message}");
            }

            foreach (var listener in listeners)
                listener.OnPurchaseFailed(product, reason);

            purchaseInProgress = false;

            return;
        }

        /// <summary>
        /// This will be called after a call to IAppleExtensions.RestoreTransactions().
        /// </summary>
        void OnTransactionsRestored(bool success)
        {
            Debug.Log($"Transactions restored: {success} - {RestoredTransactionCount}");

            PurchaseRestoring = false;
            if (success)
            {
                foreach (var listener in listeners)
                    listener.OnRestorePurchaseSuccess(RestoredTransactionCount);
            }
            else
            {
                foreach (var listener in listeners)
                    listener.OnRestorePurchaseFailed();
            }
        }

        /// <summary>
        /// iOS Specific.
        /// This is called as part of Apple's 'Ask to buy' functionality,
        /// when a purchase is requested by a minor and referred to a parent
        /// for approval.
        ///
        /// When the purchase is approved or rejected, the normal purchase events
        /// will fire.
        /// </summary>
        /// <param name="item">Item.</param>
        private void OnDeferred(Product item)
        {
            Debug.Log("Purchase deferred: " + item.definition.id);
        }

        private void OnPromotionalPurchase(Product item)
        {
            Debug.Log("Attempted promotional purchase: " + item.definition.id);

            // Promotional purchase has been detected. Handle this event by, e.g. presenting a parental gate.
            // Here, for demonstration purposes only, we will wait five seconds before continuing the purchase.
            StartCoroutine(ContinuePromotionalPurchases());
        }

        private IEnumerator ContinuePromotionalPurchases()
        {
            Debug.Log("Continuing promotional purchases in 5 seconds");
            yield return new WaitForSeconds(5);
            Debug.Log("Continuing promotional purchases now");
            AppleExtensions.ContinuePromotionalPurchases(); // iOS and tvOS only; does nothing on Mac
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}