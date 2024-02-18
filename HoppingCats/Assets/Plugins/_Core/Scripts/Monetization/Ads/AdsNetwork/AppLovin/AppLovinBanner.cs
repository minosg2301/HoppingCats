using UnityEngine;
using UnityEngine.Scripting;
using moonNest.tracking;

#if USE_MAX_SDK
using static MaxSdkBase;

namespace vgame.ads
{
    internal class AppLovinBanner : AutoHideDisplayer
    {
        [Preserve]
        public AppLovinBanner(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.BANNER, adsNetwork, displayConfig)
        {
        }

        internal override Vector2 Size
        {
            get
            {
                var dpSize = new Vector2(0, MaxSdkUtils.GetAdaptiveBannerHeight());
#if UNITY_EDITOR
                return dpSize;
#else
                return dpSize * Screen.dpi / 160f;
#endif
            }
        }

        #region override methods
        internal override void Init()
        {
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnAdClicked;
            MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnAdCollapsed;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnAdExpanded;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnAdLoaded;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnAdLoadFailed;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaid;

            MaxSdk.CreateBanner(Placement, BannerPosition.BottomCenter);
            MaxSdk.SetBannerExtraParameter(Placement, "adaptive_banner", "true");
            MaxSdk.SetBannerBackgroundColor(Placement, Color.black);
        }

        internal override void Load()
        {
        }

        internal override void Show()
        {
            MaxSdk.ShowBanner(Placement);
        }

        internal override void Hide()
        {
            MaxSdk.HideBanner(Placement);
        }
        #endregion

        #region callback
        void OnAdRevenuePaid(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            string adNetwork = AdsNetwork.ToString();
            TrackingManager.OnAdsImpress("AppLovin", adNetwork, adInfo.NetworkName, adInfo.AdUnitIdentifier, adInfo.AdFormat, "USD", adInfo.Revenue);
        }

        void OnAdLoadFailed(string arg1, ErrorInfo arg2)
        {
            IsAdReady = false;
            onAdsLoadFailed((int)arg2.Code, arg2.Message);
            Ads.CallDelay(Ads.ReloadInterval, Load);
        }

        void OnAdLoaded(string arg1, AdInfo arg2)
        {
            IsAdReady = true;
            onAdsLoaded();
        }

        void OnAdExpanded(string arg1, AdInfo arg2)
        {
        }

        void OnAdCollapsed(string arg1, AdInfo arg2)
        {
        }

        void OnAdClicked(string arg1, AdInfo arg2)
        {
            onAdsClicked();
        }
        #endregion
    }
}
#endif