using UnityEngine;

namespace moonNest.ads
{
    internal class BannerController : AdsController
    {
        AdsDisplayer current;

        public BannerController() : base(AdsType.BANNER) { }

        internal override Vector2 DisplaySize => current ? current.Size : Vector2.zero;

#if USE_IRONSRC_ADS
        private IronSourceBanner _ironSourceBanner;
        public IronSourceBanner IronSourceBanner
        {
            get
            {
                if (!_ironSourceBanner)
                {
                    _ironSourceBanner = displayers.Find(d => d.AdsNetwork.Id == AdsNetworkID.IRONSRC) as IronSourceBanner;
                }
                return _ironSourceBanner;
            }
        }
#endif

        internal override void Init()
        {
            onAdsShow += d => Ads.onBannerVisibilityChanged(true);
            onAdsHide += d => Ads.onBannerVisibilityChanged(false);
        }

        internal override void ShowAds()
        {
            if (!ShouldShowAds) return;

            // cached prev displayer
            var prev = current;

            // get available displayer
            current = AvailableDisplayer;

            // hide previvous displayer
            if (prev && prev != current)
                prev.Hide();

            // flag banner showing for show later
            IsShowing = true;

#if USE_IRONSRC_ADS
            // force use ironsource ads if no backup
            IronSourceBanner.Load();
#endif

            // not available for showing banner
            if (!current)
            {
#if !UNITY_EDITOR
                TrackingManager.OnBannerNoFill();
#endif
                return;
            }

            // show new banner
            current.Show();
            onAdsShow(current);
        }

        internal override void HideAds()
        {
            if (!current) return;

            current.Hide();
            onAdsHide(current);
            IsShowing = false;
            current = null;
        }

        protected override void OnAdsLoaded(AdsDisplayer displayer)
        {
            base.OnAdsLoaded(displayer);

            //  Current displayer showing
            if (current)
            {
                // if displayer with lower order has ads loaded, use this displayer to show
                if (displayer.Config.order < current.Config.order)
                {
                    current.Hide();
                    current = displayer;
                    current.Show();
                    onAdsShow(current);
                }

                // call show current displayer anyway
                // current.Show();
                // onAdsShow(current);
            }
            else // if there is no showing displayer
            {
                // if showing flag is on, show banner
                if (IsShowing)
                {
                    current = displayer;
                    current.Show();
                    onAdsShow(current);
                }
                else
                {
                    // try hide banner
                    displayer.Hide();
                }
            }
        }

        protected override void OnAdsLoadFailed(AdsDisplayer displayer, int code, string message)
        {
            base.OnAdsLoadFailed(displayer, code, message);
        }
    }
}