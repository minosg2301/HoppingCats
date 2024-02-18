#if USE_GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using System;
using UnityEngine;

namespace vgame.ads
{
    internal class AdmobBanner : AutoHideDisplayer
    {
        private AdSize adaptiveSize;
        private BannerView bannerView;
        private bool isAdReady;

        [Preserve]
        public AdmobBanner(AdsNetwork network, DisplayConfig display) : base(AdsType.BANNER, network, display)
        {
        }

        internal override Vector2 Size => new Vector2(0, adaptiveSize.Height);

        internal override bool Available => isAdReady;

        internal override void Init()
        {
            adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            bannerView = new BannerView(Placement, adaptiveSize, AdPosition.Bottom);
            bannerView.OnAdLoaded += HandleOnAdLoaded;
            bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            //bannerView.OnAdOpening += HandleOnAdOpened;
            //bannerView.OnAdClosed += HandleOnAdClosed;
            //bannerView.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        }

        internal override void Load()
        {
            AdRequest adRequest = new AdRequest.Builder().Build();
            bannerView.LoadAd(adRequest);
        }

        internal override void Show()
        {

        }

        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
        {
            isAdReady = false;
        }
        private void HandleOnAdLoaded(object sender, EventArgs e)
        {
            isAdReady = true;
        }
    }
}
#endif