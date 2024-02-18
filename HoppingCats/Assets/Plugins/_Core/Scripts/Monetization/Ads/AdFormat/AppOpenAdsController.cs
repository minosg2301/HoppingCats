namespace moonNest.ads
{
    internal class AppOpenAdsController : AdsController
    {
        public AppOpenAdsController() : base(AdsType.APP_OPEN) { }

        public bool CachedDisplayable { get; private set; }

        public void UpdateNextShowTime()
        {
            if (Ads.AppOpenAdSkipEnabled)
            {
                AppOpenAd.UpdateNextShowTime();
            }
        }

        public override bool Displayable
        {
            get => base.Displayable;
            set
            {
                base.Displayable = value;
                AppOpenAd.SetDisplayable(value);
            }
        }

        internal override void Init()
        {
            var rewardAds = Ads.GetAdsController(AdsType.REWARDED);
            if (rewardAds)
            {
                rewardAds.onAdsShow += OnRewardedAdsShow;
                rewardAds.onAdsHide += OnRewardedAdsHide;
            }

            var interstitial = Ads.GetAdsController(AdsType.INTERSTITIAL);
            if (interstitial)
            {
                interstitial.onAdsShow += OnInterstitialAdsShow;
                interstitial.onAdsHide += OnInterstitialAdsHide;
            }
        }

        private void OnInterstitialAdsHide(AdsDisplayer displayer)
        {
            Displayable = CachedDisplayable;
            UpdateNextShowTime();
        }

        private void OnInterstitialAdsShow(AdsDisplayer displayer)
        {
            CachedDisplayable = Displayable;
            Displayable = false;
            UpdateNextShowTime();
        }

        private void OnRewardedAdsHide(AdsDisplayer displayer)
        {
            Displayable = CachedDisplayable;
            UpdateNextShowTime();
        }

        private void OnRewardedAdsShow(AdsDisplayer displayer)
        {
            CachedDisplayable = Displayable;
            Displayable = false;
            UpdateNextShowTime();
        }

        internal override void ShowAds() { }
        internal override void HideAds() { }
    }
}
