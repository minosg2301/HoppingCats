#if USE_IRONSRC_ADS
using UnityEngine;
using UnityEngine.Scripting;
using vgame.tracking;

#if USE_AMAZON_SDK
using AmazonAds;
#endif

namespace vgame.ads
{
    internal class IronSourceRewardAds : AutoHideDisplayer
    {
        bool videoCompleted = false;

        [Preserve]
        public IronSourceRewardAds(AdsNetwork adsNetwork, DisplayConfig displayConfig) : base(AdsType.REWARDED, adsNetwork, displayConfig)
        {
        }

        internal override bool Available => Debug.isDebugBuild || IronSource.Agent.isRewardedVideoAvailable();

        internal override void Init()
        {
            IronSourceRewardedVideoEvents.onAdOpenedEvent += OnAdsOpened;
            IronSourceRewardedVideoEvents.onAdClickedEvent += OnAdsClicked;
            IronSourceRewardedVideoEvents.onAdClosedEvent += OnAdsClosed;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += OnAdAvailable;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += OnAdsRewarded;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += OnAdsShowFailed;

#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
            Load();
#endif
        }

#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR // ONLY AVAILABLE ON LATEST IRONSOURCE
        APSVideoAdRequest rewardedVideoAdRequest;
        public void LoadAPSRewardVideo()
        {
            var width = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown ? 320 : 480;
            var height = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown ? 480 : 320;
            rewardedVideoAdRequest = new APSVideoAdRequest(width, height, Config.amazonSlotId);
            rewardedVideoAdRequest.onSuccess += (adResponse) =>
            {
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, APSMediationUtils.GetRewardedNetworkData(Config.amazonSlotId, adResponse));
                IronSource.Agent.loadRewardedVideo(); // If manual rewarded mode
            };
            rewardedVideoAdRequest.onFailedWithError += (adError) =>
            {
                IronSource.Agent.loadRewardedVideo(); // If manual rewarded mode
            };
            rewardedVideoAdRequest.LoadAd();
        }
#endif

        internal override void Load()
        {
#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
            LoadAPSRewardVideo();
#endif
        }

        internal override void Show()
        {
            if (Placement.Length == 0) IronSource.Agent.showRewardedVideo();
            else IronSource.Agent.showRewardedVideo(Placement);

#if UNITY_EDITOR
            if (Ads.adsConfig.simulateShowFailed)
            {
                Ads.CallDelay(0.2f, () => OnAdsShowFailed(null, null));
            }
            else
            {
                // simulate ad display success
                OnAdsOpened(null);
                OnAdsRewarded(null, null);
                Ads.CallDelay(0.2f, () => OnAdsClosed(null));
            }
#endif
        }

        void OnAdAvailable(IronSourceAdInfo adInfo)
        {
            onAdsLoaded();
        }

        void OnAdsClicked(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            onAdsClicked();
        }

        void OnAdsOpened(IronSourceAdInfo adInfo)
        {
            videoCompleted = false;
            onAdsShow();
            Ads.PauseGame();
        }

        void OnAdsClosed(IronSourceAdInfo adInfo)
        {
#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR // ONLY AVAILABLE ON LATEST IRONSOURCE
            Load();
#endif
            if (videoCompleted) onAdsShowCompleted();
            else onAdsCancel();

            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, videoCompleted);
        }

        void OnAdsShowFailed(IronSourceError obj, IronSourceAdInfo adInfo)
        {
            onAdsShowFailed();
            onAdsHide();            
            Ads.UpdateShowAdsComplete(Type, false);
        }

        void OnAdsRewarded(IronSourcePlacement obj, IronSourceAdInfo adInfo)
        {
            videoCompleted = true;
        }
    }
}
#endif // USE_IRON_SRC