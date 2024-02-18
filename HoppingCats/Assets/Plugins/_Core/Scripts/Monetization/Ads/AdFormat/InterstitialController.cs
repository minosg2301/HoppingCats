using System;

namespace moonNest.ads
{
    internal class InterstitialController : AdsController
    {
        int passCount = 0;
        DateTime nextShowTime;
        AdsDisplayer current;

        internal bool forceShow = false;

        public override bool AdsAvailable
        {
            get
            {
#if UNITY_EDITOR
                return !Ads.adsConfig.simulateNoInterstitial && base.AdsAvailable;
#else
                return base.AdsAvailable;
#endif
            }
        }

        public InterstitialController() : base(AdsType.INTERSTITIAL)
        {
            UpdateNextShowTime();
        }

        void UpdateNextShowTime()
        {
            nextShowTime = DateTime.Now.AddSeconds(InterAdSkipDuration);
        }

        internal float InterAdSkipDuration => Ads.InterAdSkipDuration;
        internal int InterstitialCount => 5;

        /// <summary>
        /// 1: Update showable by InterstitialCount
        /// 2: Update showable by InterstitialInterval
        /// </summary>
        internal int InterstitialType => 2;

        internal override bool ShouldShowAds
        {
            get
            {
                if (!Ads.InterAdSkipEnabled)
                    return true;

                if (InterstitialType == 1)
                {
                    return ++passCount >= InterstitialCount;
                }
                else
                {
                    return DateTime.Compare(nextShowTime, DateTime.Now) <= 0;
                }
            }
        }

        internal override void Init()
        {
            var rewardAds = Ads.GetAdsController(AdsType.REWARDED);
            if (rewardAds) rewardAds.onAdsHide += OnRewardedAdsHide;
        }

        internal override void ShowAds()
        {
            if (!forceShow && !ShouldShowAds)
            {
                Ads.UpdateShowAdsComplete(Type, false);
                return;
            }

            current = AvailableDisplayer;
            if (!current)
            {
                Ads.UpdateShowAdsComplete(Type, false);
                return;
            }

            current.onAdsHide += OnAdsHide;
            current.Show();
            onAdsShow(current);
            IsShowing = true;

            passCount = 0;
            UpdateNextShowTime();
        }

        internal override void HideAds() { }

        void OnAdsHide()
        {
            current.onAdsHide -= OnAdsHide;
            onAdsHide(current);
            IsShowing = false;
        }

        void OnRewardedAdsHide(AdsDisplayer displayer)
        {
            passCount = 0;
            UpdateNextShowTime();
        }

        protected override void OnAdsShow(AdsDisplayer displayer)
        {
            base.OnAdsShow(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;
            Ads.NotifyInterAdShowed(adsNetwork, placement);
        }

        protected override void OnAdsLoaded(AdsDisplayer displayer)
        {
            base.OnAdsLoaded(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;
            Ads.NotifyInterAdLoaded(adsNetwork, placement);
        }

        protected override void OnAdsLoadFailed(AdsDisplayer displayer, int code, string message)
        {
            base.OnAdsLoadFailed(displayer, code, message);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;
            Ads.NotifyInterAdLoadFailed(adsNetwork, placement, code, message);
        }

        protected override void OnAdsClicked(AdsDisplayer displayer)
        {
            base.OnAdsClicked(displayer);

            var adsNetwork = displayer.AdsNetwork.ToString();
            var placement = displayer.Placement;
            Ads.NotifyInterAdClicked(adsNetwork, placement);
        }
    }
}
