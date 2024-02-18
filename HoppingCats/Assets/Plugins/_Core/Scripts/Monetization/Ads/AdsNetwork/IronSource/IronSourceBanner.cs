#if USE_IRONSRC_ADS
using UnityEngine;
using UnityEngine.Scripting;

#if USE_AMAZON_SDK
using AmazonAds;
#endif

namespace vgame.ads
{
    internal class IronSourceBanner : AutoHideDisplayer
    {
        [Preserve]
        public IronSourceBanner(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.BANNER, adsNetwork, displayConfig)
        {
        }

        internal override Vector2 Size
        {
            get
            {
#if UNITY_EDITOR
                return new Vector2(0, 100f);
#else
                var dpSize = new Vector2(0, 50f);
                return dpSize * Screen.dpi / 160f;
#endif
            }
        }

        internal override void Init()
        {
            IronSourceBannerEvents.onAdLoadedEvent += OnAdsLoaded;
            IronSourceBannerEvents.onAdLoadFailedEvent += OnAdsLoadFailed;
            IronSourceBannerEvents.onAdClickedEvent += OnAdsClicked;
            IronSourceBannerEvents.onAdScreenPresentedEvent += OnAdsScreenPresented;
            IronSourceBannerEvents.onAdScreenDismissedEvent += OnAdsScreenDismissed;
            IronSourceBannerEvents.onAdLeftApplicationEvent += OnAdsLeftApplication;
        }

#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
        APSBannerAdRequest bannerAdRequest;
        public void LoadAPSBanner()
        {
            const int width = 320;
            const int height = 50;
            bannerAdRequest = new APSBannerAdRequest(width, height, Config.amazonSlotId);
            bannerAdRequest.onSuccess += (adResponse) =>
            {
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, APSMediationUtils.GetBannerNetworkData(Config.amazonSlotId, adResponse));
                IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
            };
            bannerAdRequest.onFailedWithError += (adError) =>
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
            };
            bannerAdRequest.LoadAd();
        }
#endif

        internal override void Load()
        {
#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK && !UNITY_EDITOR
            LoadAPSBanner();
#else
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
#endif
        }

        internal override void Show()
        {
            IronSource.Agent.displayBanner();
        }

        internal override void Hide()
        {
            IronSource.Agent.hideBanner();
        }

        void OnAdsLoadFailed(IronSourceError error)
        {
            IsAdReady = false;
            onAdsLoadFailed(error.getErrorCode(), error.getDescription());
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnAdsLoaded(IronSourceAdInfo adInfo)
        {
            IsAdReady = true;
            onAdsLoaded();
        }

        private void OnAdsClicked(IronSourceAdInfo adInfo) { onAdsClicked(); }
        private void OnAdsScreenPresented(IronSourceAdInfo adInfo) { }
        private void OnAdsScreenDismissed(IronSourceAdInfo adInfo) { }
        private void OnAdsLeftApplication(IronSourceAdInfo adInfo) { }
    }
}
#endif // USE_IRONSRC_ADS