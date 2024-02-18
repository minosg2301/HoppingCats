using UnityEngine;
using UnityEngine.Scripting;
using moonNest.tracking;

#if USE_MAX_SDK
namespace vgame.ads
{
    internal class AppLovinInterstitial : AutoHideDisplayer
    {
        [Preserve]
        public AppLovinInterstitial(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.INTERSTITIAL, adsNetwork, displayConfig)
        {
        }

        internal override bool Available =>
#if UNITY_EDITOR
            Debug.isDebugBuild ||
#endif
            MaxSdk.IsInterstitialReady(Placement) ||
            (Placement2nd.Length > 0 && MaxSdk.IsInterstitialReady(Placement2nd));

        internal override void Init()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnLoaded;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnLoadFailed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnDisplayed;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnDisplayFailed;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnHidden;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaid;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnAdClicked;

            // Load the first interstitial
            Load();
        }

        internal override void Load()
        {
            MaxSdk.LoadInterstitial(Placement);
            if (Placement2nd.Length > 0)
            {
                MaxSdk.LoadInterstitial(Placement2nd);
            }
        }

        internal override void Show()
        {
            if (MaxSdk.IsInterstitialReady(Placement))
            {
                MaxSdk.ShowInterstitial(Placement);
            }
            else if (MaxSdk.IsInterstitialReady(Placement2nd))
            {
                MaxSdk.ShowInterstitial(Placement2nd);
            }
            else
            {
                Ads.UpdateShowAdsComplete(Type, false);
            }
        }

        void OnLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onAdsLoaded();
        }

        void OnLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            onAdsLoadFailed((int)errorInfo.Code, errorInfo.Message);
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onAdsShow();
            Ads.PauseGame();
        }

        void OnDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            onAdsShowFailed();
            Load();
            Ads.UpdateShowAdsComplete(Type, false);
        }

        void OnAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onAdsClicked();
        }

        void OnHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Load();
            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, true);
        }

        private void OnAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            string adNetwork = AdsNetwork.ToString();
            TrackingManager.OnAdsImpress("AppLovin", adNetwork, adInfo.NetworkName, adInfo.AdUnitIdentifier, adInfo.AdFormat, "USD", adInfo.Revenue);
        }
    }
}
#endif