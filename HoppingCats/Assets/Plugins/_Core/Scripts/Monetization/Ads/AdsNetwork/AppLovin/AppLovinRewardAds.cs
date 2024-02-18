#if USE_MAX_SDK
using UnityEngine;
using UnityEngine.Scripting;
using vgame.tracking;

namespace vgame.ads
{
    internal class AppLovinRewardAds : AutoHideDisplayer
    {
        bool videoCompleted = false;

        [Preserve]
        public AppLovinRewardAds(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.REWARDED, adsNetwork, displayConfig)
        {
        }

        internal override bool Available => Debug.isDebugBuild || MaxSdk.IsRewardedAdReady(Placement);

        internal override void Init()
        {
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnAdsLoaded;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnAdLoadFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnAdDisplayFailed;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnAdDisplayed;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnAdClicked;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnAdHidden;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedReward;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaid;

            Load();
        }

        internal override void Load()
        {
            MaxSdk.LoadRewardedAd(Placement);
        }

        internal override void Show()
        {
            MaxSdk.ShowRewardedAd(Placement);
        }

        void OnAdsLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onAdsLoaded();
        }

        void OnAdLoadFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            onAdsLoadFailed((int)errorInfo.Code, errorInfo.Message);
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnAdDisplayed(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            videoCompleted = false;
            onAdsShow();
            Ads.PauseGame();
        }

        void OnAdDisplayFailed(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            onAdsShowFailed();
            Load();
        }

        void OnAdClicked(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            onAdsClicked();
        }

        void OnAdHidden(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (videoCompleted) onAdsShowCompleted();
            else onAdsCancel();

            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, videoCompleted);
            Load();
        }

        void OnAdReceivedReward(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            videoCompleted = true;
        }

        void OnAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            string adNetwork = AdsNetwork.ToString();
            TrackingManager.OnAdsImpress("AppLovin", adNetwork, adInfo.NetworkName, adInfo.AdUnitIdentifier, adInfo.AdFormat, "USD", adInfo.Revenue);
        }
    }
}
#endif