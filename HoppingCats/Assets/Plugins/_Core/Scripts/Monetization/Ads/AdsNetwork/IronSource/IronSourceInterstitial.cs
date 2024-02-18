#if USE_IRONSRC_ADS
using UnityEngine;
using UnityEngine.Scripting;

#if USE_AMAZON_SDK
using AmazonAds;
#endif

namespace vgame.ads
{
    internal class IronSourceInterstitial : AutoHideDisplayer
    {
        [Preserve]
        public IronSourceInterstitial(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.INTERSTITIAL, adsNetwork, displayConfig)
        {
        }

        internal override bool Available => Debug.isDebugBuild || IronSource.Agent.isInterstitialReady();

        internal override void Init()
        {
            IronSourceInterstitialEvents.onAdReadyEvent += OnAdsReady;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += OnAdsLoadFailed;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += OnAdsShowSucceeded;
            IronSourceInterstitialEvents.onAdShowFailedEvent += OnAdsShowFailed;
            IronSourceInterstitialEvents.onAdClickedEvent += OnAdsClicked;
            IronSourceInterstitialEvents.onAdOpenedEvent += OnAdsOpened;
            IronSourceInterstitialEvents.onAdClosedEvent += OnAdsClosed;

            Load();
        }

#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
        APSVideoAdRequest interstitialVideoAdRequest;
        public void LoadAPSInterVideo()
        {
            var width = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown ? 320 : 480;
            var height = Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown ? 480 : 320;
            interstitialVideoAdRequest = new APSVideoAdRequest(width, height, Config.amazonSlotId);
            interstitialVideoAdRequest.onSuccess += (adResponse) =>
            {
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, APSMediationUtils.GetInterstitialNetworkData(Config.amazonSlotId, adResponse));
                IronSource.Agent.loadInterstitial();
            };
            interstitialVideoAdRequest.onFailedWithError += (adError) =>
            {
                IronSource.Agent.loadInterstitial();
            };
            interstitialVideoAdRequest.LoadAd();
        }
#endif

        internal override void Load()
        {
#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
            LoadAPSInterVideo();
#else
            IronSource.Agent.loadInterstitial();
#endif
        }

        internal override void Show()
        {
            IronSource.Agent.showInterstitial();

#if UNITY_EDITOR
            // simulate ad display success
            OnAdsOpened(null);
            OnAdsClosed(null);
            OnAdsShowSucceeded(null);
#endif
        }

        void OnAdsReady(IronSourceAdInfo adInfo)
        {
            onAdsLoaded();
        }

        void OnAdsLoadFailed(IronSourceError error)
        {
            onAdsLoadFailed(error.getErrorCode(), error.getDescription());
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnAdsOpened(IronSourceAdInfo adInfo)
        {
            //onAdsShow();
            Ads.PauseGame();
        }

        void OnAdsClosed(IronSourceAdInfo adInfo)
        {
            Load();
            onAdsHide();
            Ads.ResumeGame();
            Ads.UpdateShowAdsComplete(Type, true);
        }

        void OnAdsClicked(IronSourceAdInfo adInfo)
        {
            onAdsClicked();
        }

        void OnAdsShowFailed(IronSourceError obj, IronSourceAdInfo adInfo)
        {
            onAdsShowFailed();
            Ads.UpdateShowAdsComplete(Type, false);
        }

        void OnAdsShowSucceeded(IronSourceAdInfo adInfo)
        {
            onAdsShow();
        }
    }
}
#endif // USE_IRONSRC_ADS