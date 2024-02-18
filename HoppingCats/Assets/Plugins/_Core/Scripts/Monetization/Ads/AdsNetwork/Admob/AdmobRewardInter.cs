#if USE_NATIVE_ADMOB
using UnityEngine;
using UnityEngine.Scripting;
using vgame.tracking;
using static vgame.ads.NativeAdmob;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace vgame.ads
{
    internal class AdmobRewardInter : AutoHideDisplayer
    {
        bool isRewarded;

        [Preserve]
        public AdmobRewardInter(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.RWD_INTER, adsNetwork, displayConfig)
        {
        }

        internal override void Init()
        {
            RewardedInterstitialEvent.OnAdsLoaded += OnAdsLoaded;
            RewardedInterstitialEvent.OnAdsDidDismissFullScreenContent += OnAdDidDismiss;
            RewardedInterstitialEvent.OnAdsDidPresentFullScreenContent += OnAdDidPresent;
            RewardedInterstitialEvent.OnAdsFailedToPresentFullScreenContent += OnAdFailedToPresent;
            RewardedInterstitialEvent.OnAdsFailedToLoad += OnAdFailedToLoad;
            RewardedInterstitialEvent.OnAdsRewarded += OnAdRewarded;
            RewardedInterstitialEvent.OnAdsPaidEvent += OnPaidEvent;

            Load();
        }

        void OnAdsLoaded()
        {
            IsAdReady = true;
            onAdsLoaded();
        }

        void OnAdFailedToLoad()
        {
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnAdDidPresent()
        {
            isRewarded = false;
            onAdsShow();
            Ads.PauseGame();
        }

        void OnAdDidDismiss()
        {
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, isRewarded);

            if (isRewarded) onAdsShowCompleted();
            else onAdsCancel();

            onAdsHide();

            IsAdReady = false;
            Load();
        }

        void OnAdFailedToPresent()
        {
            IsAdReady = false;
            Ads.UpdateShowAdsComplete(Type, false);
            Load();
        }

        void OnAdRewarded(string type, int amount)
        {
            isRewarded = true;
        }

        void OnPaidEvent(AdsValue adsValue)
        {
            TrackingManager.OnAdmobAdRevenue(adsValue);
        }

#if UNITY_EDITOR
        internal override void Load() { IsAdReady = true; }
        internal override void Show()
        {
            var showSuccess = !Ads.adsConfig.simulateShowFailed;
            Ads.CallDelay(0.2f, () => Ads.UpdateShowAdsComplete(Type, showSuccess));
        }
#else
#if UNITY_ANDROID

        internal override void Load()
        {        
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "LoadRewardedInterstitial", Placement);
        }

        internal override void Show()
        {
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "ShowRewardedInterstitial");
        }

#elif UNITY_IOS

        internal override void Load()
        {
            LoadRewardedInterstitial_iOS(Placement);
        }

        internal override void Show()
        {
            ShowRewardedInterstitial_iOS();
        }

        [DllImport("__Internal")]
        private static extern void LoadRewardedInterstitial_iOS(string placement);
        [DllImport("__Internal")]
        private static extern void ShowRewardedInterstitial_iOS();

#endif // UNITY_ANDROID
#endif // UNITY_EDITOR
    }
}
#endif