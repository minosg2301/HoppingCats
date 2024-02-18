using UnityEngine;
using moonNest.tracking;

namespace moonNest.ads
{
    internal class RewardAdsController : AdsController
    {
        AdsDisplayer current;

        public override bool AdsAvailable
        {
            get
            {
#if UNITY_EDITOR
                return !Ads.adsConfig.simulateNoRewardAds && base.AdsAvailable;
#else
                return base.AdsAvailable;
#endif
            }
        }

        public RewardAdsController() : base(AdsType.REWARDED)
        {
        }

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

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;
            Ads.NotifyRewardedAdShowed(adsNetwork, placement);
        }

        protected override void OnAdsShowComplete(AdsDisplayer displayer)
        {
            base.OnAdsShowComplete(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;

            Ads.NotifyRewardedAdCompleted(adsNetwork, placement);
        }

        protected override void OnAdsCancel(AdsDisplayer displayer)
        {
            base.OnAdsCancel(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;

            Ads.NotifyRewardedAdCanceled(adsNetwork, placement);
        }

        protected override void OnAdsLoaded(AdsDisplayer displayer)
        {
            base.OnAdsLoaded(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;

            Ads.NotifyRewardedAdLoaded(adsNetwork, placement);
        }

        protected override void OnAdsLoadFailed(AdsDisplayer displayer, int code, string message)
        {
            base.OnAdsLoadFailed(displayer, code, message);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;

            Ads.NotifyRewardedAdLoadFailed(adsNetwork, placement, code, message);
        }

        protected override void OnAdsClicked(AdsDisplayer displayer)
        {
            base.OnAdsClicked(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;

            Ads.NotifyRewardedAdClicked(adsNetwork, placement);
        }
    }
}
