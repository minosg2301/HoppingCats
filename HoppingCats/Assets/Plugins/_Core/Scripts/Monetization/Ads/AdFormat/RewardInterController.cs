namespace moonNest.ads
{
    internal class RewardInterController : AdsController
    {
        private AdsDisplayer current;

        public override bool AdsAvailable
        {
            get
            {
#if UNITY_EDITOR
                return !Ads.adsConfig.simulateNoRewardInterAds && base.AdsAvailable;
#else
                return base.AdsAvailable;
#endif
            }
        }

        public RewardInterController() : base(AdsType.RWD_INTER) { }

        internal override void Init() { }

        internal override void ShowAds()
        {
            if (IsShowing) return;

            current = AvailableDisplayer;
            if (!current)
            {
                return;
            }

            current.onAdsHide += OnAdsHide;
            current.Show();
            onAdsShow(current);
            IsShowing = true;
        }

        void OnAdsHide()
        {
            current.onAdsHide -= OnAdsHide;
            onAdsHide(current);
            IsShowing = false;
        }

        internal override void HideAds() { }

        protected override void OnAdsShow(AdsDisplayer displayer)
        {
            base.OnAdsShow(displayer);

            var adsNetwork = current.AdsNetwork.ToString();
            var placement = current.Placement;
        }

        protected override void OnAdsShowComplete(AdsDisplayer displayer)
        {
            base.OnAdsShowComplete(displayer);

            var adsNetwork = current.AdsNetwork.ToString();
            var placement = current.Placement;
        }

        protected override void OnAdsCancel(AdsDisplayer displayer)
        {
            base.OnAdsCancel(displayer);

            var adsNetwork = current.AdsNetwork.ToString();
            var placement = current.Placement;
        }
    }
}
