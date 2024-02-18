using System;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace moonNest.ads
{
    internal class NativeAdmob : SingletonMono<NativeAdmob>
    {
        internal const string admobAdsHelperClassName = "com.vgames.admob.AdmobAdsHelper";

        internal NativeAdsConfig Config { get; private set; }

        internal event Action OnInitialized = delegate { };

        internal class NativeAdsEvent
        {
            internal static Action<string> OnAdLoaded = delegate { };
            internal static Action<string, string> OnAdLoadedFailed = delegate { };

            public delegate void AdsPaidListener(AdsValue adsValue);
            public static AdsPaidListener onAdsPaidEvent;
        }

        internal class AppOpenAdEvent
        {
            internal static Action<string> OnAdShowed = delegate { };
            internal static Action<string> OnAdLoaded = delegate { };
            internal static Action<string> OnAdLoadedFailed = delegate { };

            public delegate void AdsPaidListener(AdsValue adsValue);
            public static AdsPaidListener onAdsPaidEvent;
        }

        internal void Initialize(NativeAdsConfig nativeConfig)
        {
            Config = nativeConfig;

            var delayRefresh = !nativeConfig.noDelayRefresh;
#if UNITY_EDITOR
            Ads.CallDelay(1f, () => OnInit(""));
#else
#if UNITY_ANDROID
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, 
                "Init", delayRefresh, Config.delayRefreshSeconds);
#elif UNITY_IOS
            NativeAdMob_Init(delayRefresh, Config.delayRefreshSeconds);
#endif // UNITY_ANDROID
#endif // !UNITY_EDITOR
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void NativeAdMob_Init(bool delayRefresh, int delaySeconds);
#endif

        internal class RewardedInterstitialEvent
        {
            internal static Action OnAdsLoaded = delegate { };
            internal static Action OnAdsFailedToLoad = delegate { };
            internal static Action OnAdsFailedToPresentFullScreenContent = delegate { };
            internal static Action OnAdsDidPresentFullScreenContent = delegate { };
            internal static Action OnAdsDidDismissFullScreenContent = delegate { };
            internal static Action OnAdsImpression = delegate { };
            internal static Action<string, int> OnAdsRewarded = delegate { };
            internal static Action<AdsValue> OnAdsPaidEvent = delegate { };
        }

        protected override void Start()
        {
            base.Start();

            name = "VGame_NativeAdmob";
        }

        #region JNI Calls
        // call from native
        void OnInit(string msg)
        {
            MainThreadDispatcher.Enqueue(() => OnInitialized());
        }

        // call from native
        void AppOpenAd_onAdLoaded(string adUnit)
        {
            MainThreadDispatcher.Enqueue(() => AppOpenAdEvent.OnAdLoaded(adUnit));
        }

        // call from native
        void AppOpenAd_onAdLoadFailed(string adUnit)
        {
            MainThreadDispatcher.Enqueue(() => AppOpenAdEvent.OnAdLoadedFailed(adUnit));
        }

        // call from native
        void AppOpenAd_onAdShowed(string adUnit)
        {
            MainThreadDispatcher.Enqueue(() => AppOpenAdEvent.OnAdShowed(adUnit));
        }

        // call from native
        void AppOpenAd_onPaidEvent(string content)
        {
            string[] contents = content.Split('_');
            string adUnit = contents[0];
            string adSourceName = contents[1];
            string valueStr = contents[2];

            if (valueStr.Contains(","))
                valueStr = valueStr.Replace(",", ".");
            double valueMicros = double.Parse(valueStr);

#if UNITY_ANDROID
            valueMicros /= 1000000d;
#endif
            string currencyCode = contents[3];
            int precisionType = int.Parse(contents[4]);

            var adsValue = new AdsValue(adUnit, "app_open", adSourceName, valueMicros, currencyCode, precisionType);
            MainThreadDispatcher.Enqueue(() => AppOpenAdEvent.onAdsPaidEvent(adsValue));
        }

        // call from native
        void OnNativeAdLoaded(string key)
        {
            MainThreadDispatcher.Enqueue(() => NativeAdsEvent.OnAdLoaded(key));
        }

        // call from native
        void OnNativeLoadedAdFailed(string msg)
        {
            string[] contents = msg.Split('_');
            string key = contents[0];
            string adPlacement = contents[1];
            MainThreadDispatcher.Enqueue(() => NativeAdsEvent.OnAdLoadedFailed(key, adPlacement));
        }

        // call from native
        void OnNativeAdPaid(string content)
        {
            string[] contents = content.Split('_');
            string adUnit = contents[0];
            string adSourceName = contents[1];
            string valueStr = contents[2];

            if (valueStr.Contains(","))
                valueStr = valueStr.Replace(",", ".");
            double valueMicros = double.Parse(valueStr);

#if UNITY_ANDROID
            valueMicros /= 1000000d;
#endif
            string currencyCode = contents[3];
            int precisionType = int.Parse(contents[4]);

            var adsValue = new AdsValue(adUnit, "native", adSourceName, valueMicros, currencyCode, precisionType);
            MainThreadDispatcher.Enqueue(() => NativeAdsEvent.onAdsPaidEvent(adsValue));
        }

        void RewardedInterstitial_onPaidEvent(string content)
        {
            string[] contents = content.Split('_');
            string adUnit = contents[0];
            string adSourceName = contents[1];
            string valueStr = contents[2];

            if (valueStr.Contains(","))
                valueStr = valueStr.Replace(",", ".");
            double valueMicros = double.Parse(valueStr);

#if UNITY_ANDROID
            valueMicros /= 1000000d;
#endif
            string currencyCode = contents[3];
            int precisionType = int.Parse(contents[4]);

            var adsValue = new AdsValue(adUnit, "reward_inter", adSourceName, valueMicros, currencyCode, precisionType);
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsPaidEvent(adsValue));
        }

        // call from native
        void RewardedInterstitial_OnRewarded(string content)
        {
            string[] contents = content.Split('_');
            string type = contents[0];
            int amount = int.Parse(contents[1]);

            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsRewarded(type, amount));
        }

        // call from native
        void RewardedInterstitial_onAdLoaded(string content)
        {
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsLoaded());
        }

        // call from native
        void RewardedInterstitial_OnAdFailedToLoad(string content)
        {
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsFailedToLoad());
        }

        // call from native
        void RewardedInterstitial_onAdShowed(string content)
        {
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsDidPresentFullScreenContent());
        }

        // call from native
        void RewardedInterstitial_onAdFailedToShow(string content)
        {
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsFailedToPresentFullScreenContent());
        }

        // call from native
        void RewardedInterstitial_onAdImpression(string content)
        {
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsImpression());
        }

        // call from native
        void RewardedInterstitial_OnDismissed(string content)
        {
            MainThreadDispatcher.Enqueue(() => RewardedInterstitialEvent.OnAdsDidDismissFullScreenContent());
        }
        #endregion
    }
}