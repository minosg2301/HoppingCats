#if UNITY_WEBGL && USE_FB_INSTANT && !UNITY_EDITOR
#define USE_FBIG_INAPP
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using moonNest.tracking;

#if USE_FBIG_INAPP
using vgame.fbig;
#endif

namespace moonNest
{
    public class IAPManager : SingletonMono<IAPManager>, IDetailedStoreListener
    {
        public IStoreController StoreController { get; private set; }
        public IExtensionProvider ExtensionProvider { get; private set; }

        public ITransactionHistoryExtensions TransactionHistoryExtensions { get; private set; }
#if !USE_FBIG_INAPP
        public IAppleExtensions AppleExtensions { get; private set; }
        public IMicrosoftExtensions MicrosoftExtensions { get; private set; }
        public IGooglePlayStoreExtensions GooglePlayStoreExtensions { get; private set; }
#endif
        public bool InitializationComplete { get; private set; }
        public bool PurchaseRestoring { get; private set; } = false;

        public int RestoredTransactionCount { get; private set; } = 0;
        public int RestoredTransactionSuccess { get; private set; } = 0;
        public InStorePackage PurchasedPackage => purchasedPackage;

        InStorePackage purchasedPackage;
        InStorePackage processingPackage;
        bool purchaseInProgress = false;
        List<SubscriptionInfo> subscriptions;

        readonly List<IPurchaseListener> listeners = new List<IPurchaseListener>();

        [NonSerialized] public UnityEvent onInitialized = new UnityEvent();

        void OnApplicationFocus(bool focus)
        {
            if (focus && InitializationComplete)
            {
                GetSubscriptionInfos();
            }
        }

        #region initialization
        public void Init()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer) return;

#if USE_FBIG_INAPP
            var module = FBIGPurchasingModule.Instance();
#else
            var module = StandardPurchasingModule.Instance();
            module.useFakeStoreUIMode = GlobalConfig.Ins.fakeStoreMode;
#endif

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);

#if !USE_FBIG_INAPP
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
#endif

            // Populate IAP Packages
            var packages = IAPPackageAsset.Ins.Packages;
            foreach (var package in packages)
            {
                builder.AddProduct(package.productId, package.type);
                if (package.promotionProductId.Length > 0)
                    builder.AddProduct(package.promotionProductId, package.type);
            }

            if (packages.Count > 0)
            {
                try
                {
                    UnityPurchasing.Initialize(Ins, builder);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("IAPManager Initialized");

            InitializationComplete = true;
            StoreController = controller;
            ExtensionProvider = extensions;

            TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
#if !USE_FBIG_INAPP
            AppleExtensions = extensions.GetExtension<IAppleExtensions>();
            GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();


            // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
            // On non-Apple platforms this will have no effect; OnDeferred will never be called.
            AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);
            foreach (var product in controller.products.all)
            {
                DebugProduct(product);
                if (product.availableToPurchase && GlobalConfig.Ins.appStorePromotionPurchase)
                {
                    AppleExtensions.SetStorePromotionVisibility(product, AppleStorePromotionVisibility.Show);
                }
            }
#endif

            GetSubscriptionInfos();

            onInitialized.Invoke();
        }

        private void DebugProduct(Product product)
        {
            var hasReceipt = product.receipt != null;
            if (product.definition.type == ProductType.Subscription)
            {
                if (hasReceipt)
                    Debug.Log($"{product.definition.storeSpecificId}|{hasReceipt}|{CanCreateSubscriptionManager(product.receipt)}");
                else
                    Debug.Log($"{product.definition.storeSpecificId}|{hasReceipt}");
            }
            else
            {
                Debug.Log($"{product.definition.storeSpecificId}");
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"Purchasing failed to initialize. Reason: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogError($"Purchasing failed to initialize. Reason: {error} - {message}");
        }
        #endregion

        #region subscriptions
        public void GetSubscriptionInfos()
        {
            subscriptions = new List<SubscriptionInfo>();
#if UNITY_IOS
            Dictionary<string, string> introductory_info_dict = AppleExtensions.GetIntroductoryPriceDictionary();
#endif
            Product[] purchasedProducts = StoreController.products.all;
            foreach (Product product in purchasedProducts)
            {
                if (product.receipt != null)
                {
                    if (product.definition.type == ProductType.Subscription)
                    {
                        if (CanCreateSubscriptionManager(product.receipt))
                        {
                            string intro_json = null;
#if UNITY_IOS
                            intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(product.definition.storeSpecificId))
                                ? null
                                : introductory_info_dict[product.definition.storeSpecificId];
#endif
                            SubscriptionManager p = new SubscriptionManager(product, intro_json);
                            SubscriptionInfo info = p.getSubscriptionInfo();
                            subscriptions.Add(info);
                        }
                    }
                }
            }
        }

        bool CanCreateSubscriptionManager(string receipt)
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

            string productId = purchasePackage.AvailableProductId;
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
            foreach (var listener in listeners.ToList())
                listener.OnPurchaseBegin();
            StoreController.InitiatePurchase(productId);
        }

#if USE_FBIG_INAPP
        public void RestoreTransaction()
        {
        }

        public void UpdateSubscription(string newProductId, string oldProductId)
        {
        }
#else
        public void RestoreTransaction()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                     Application.platform == RuntimePlatform.OSXPlayer ||
                     Application.platform == RuntimePlatform.tvOS)
            {
                RestoredTransactionCount = 0;
                PurchaseRestoring = true;
                foreach (var listener in listeners.ToList())
                    listener.OnRestorePurchaseBegin();
                AppleExtensions.RestoreTransactions(OnTransactionsRestored);
            }
            else
            {
                Debug.LogWarning(Application.platform.ToString() + " is not a supported platform for the Codeless IAP restore button");
            }
        }

        public void UpdateSubscription(string newProductId, string oldProductId)
        {
            Debug.Log($"Update Subscription from {oldProductId} to {newProductId}");
            Product oldProduct = StoreController.products.WithID(oldProductId);
            Product newProduct = StoreController.products.WithID(newProductId);
#if UNITY_ANDROID
            SubscriptionManager.UpdateSubscriptionInGooglePlayStore(
                    oldProduct,
                    newProduct,
                    (productInfos, newProductId) =>
                    {
                        Debug.Log($"Current Production Info {productInfos}");
                        GooglePlayStoreExtensions.UpgradeDowngradeSubscription(
                            oldProduct.definition.id,
                            newProduct.definition.id);
                    });
#else
            SubscriptionManager.UpdateSubscriptionInAppleStore(newProduct, "", StoreController.InitiatePurchase);
#endif
        }
#endif


        #endregion

        #region handle process purchase

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("ProcessPurchase: " + e.purchasedProduct.definition.id);

            var productId = e.purchasedProduct.definition.id;
            var _package = processingPackage;
            processingPackage = null;
            purchasedPackage = null;

            if (PurchaseRestoring)
            {
                RestoredTransactionCount++;
                if (e.purchasedProduct.definition.type != ProductType.NonConsumable)
                {
                    return PurchaseProcessingResult.Complete;
                }
            }

#if UNITY_WEBGL && USE_FB_INSTANT
            // prevent process verify many times
            if (e.purchasedProduct.definition.type == ProductType.NonConsumable
                && PurchasedProducts.ExistProduct(e.purchasedProduct.definition.id))
            {
                return PurchaseProcessingResult.Complete;
            }
#endif

            PurchaseProcessingResult result;
            result = IAPPackageProcess.ProcessPurchase(e, _package,
            (processingResult) =>
            {
                purchasedPackage = _package;

                foreach (var listener in listeners.ToList())
                    listener.OnPurchaseCompleted(e.purchasedProduct, e);

                purchaseInProgress = false;

                if (processingResult == PurchaseProcessingResult.Pending)
                    StoreController.ConfirmPendingPurchase(e.purchasedProduct);

                if (e.purchasedProduct.definition.type == ProductType.Subscription)
                {
                    GetSubscriptionInfos();
                }

#if UNITY_WEBGL && USE_FB_INSTANT
                // cached non-consumable product to check later
                if (e.purchasedProduct.definition.type == ProductType.NonConsumable)
                {
                    PurchasedProducts.AddProduct(e.purchasedProduct.definition.id);
                }
#endif
            },
            (reason, message) =>
            {
                PurchaseFailureDescription description = new(e.purchasedProduct.definition.id, reason, "Unknown");
                OnPurchaseFailed(e.purchasedProduct, description);
            });

            return result;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError("Should not be called");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase Failed: {failureDescription.productId}|{failureDescription.reason}|{failureDescription.message}");

            // Detailed debugging information
            Debug.Log("Store specific error code: " + TransactionHistoryExtensions.GetLastStoreSpecificPurchaseErrorCode());
            if (TransactionHistoryExtensions.GetLastPurchaseFailureDescription() != null)
            {
                Debug.Log($"Purchase failure description message: {TransactionHistoryExtensions.GetLastPurchaseFailureDescription().message}");
            }

            foreach (var listener in listeners.ToList())
                listener.OnPurchaseFailed(product, failureDescription.reason);

            purchaseInProgress = false;

            return;
        }
        #endregion

#if !USE_FBIG_INAPP
        #region Apple Store handler
        /// <summary>
        /// This will be called after a call to IAppleExtensions.RestoreTransactions().
        /// </summary>
        void OnTransactionsRestored(bool success, string message)
        {
            Debug.Log($"Transactions restored: {success} - {message} - {RestoredTransactionCount}");

            PurchaseRestoring = false;
            if (success)
            {
                foreach (var listener in listeners.ToList())
                    listener.OnRestorePurchaseSuccess(RestoredTransactionCount);
            }
            else
            {
                foreach (var listener in listeners.ToList())
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
        #endregion
#endif

    }
}