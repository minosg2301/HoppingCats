using System.Collections.Generic;
using UnityEngine;

namespace moonNest.ads
{
    internal abstract class AdsController
    {
        internal delegate void AdsControllerEvent(AdsDisplayer displayer);
        internal AdsControllerEvent onAdsShow = delegate { };
        internal AdsControllerEvent onAdsHide = delegate { };

        internal AdsType Type { get; }
        internal virtual Vector2 DisplaySize => Vector2.zero;

        protected List<AdsDisplayer> displayers = new List<AdsDisplayer>();

        /// <summary>
        /// Default is True
        /// </summary>
        internal virtual bool ShouldShowAds => true;

        /// <summary>
        /// Get Ads with avaliable Ads to show
        /// </summary>
        internal AdsDisplayer AvailableDisplayer => displayers.Find(d => d.Available);

        /// <summary>
        /// Short hand to get AdsPlatformConfig
        /// </summary>
        internal AdsPlatformConfig AdsPlatform => ThirdPartyConfig.Ins.adsConfig.AdsPlatform;

        /// <summary>
        /// Keep track ads is showing or not
        /// </summary>
        internal bool IsShowing { get; set; }

        /// <summary>
        /// Get have any ads is available
        /// </summary>
        public virtual bool AdsAvailable
        {
            get
            {
                return AvailableDisplayer && AvailableDisplayer.Available;
            }
        }

        public virtual bool Displayable { get; set; } = true;

        internal AdsController(AdsType type)
        {
            Type = type;
        }

        /// <summary>
        /// Add display in list sorted by order
        /// </summary>
        /// <param name="displayer"></param>
        internal virtual void AddDisplayer(AdsDisplayer displayer)
        {
            displayer.onAdsLoaded += () => OnAdsLoaded(displayer);
            displayer.onAdsShow += () => OnAdsShow(displayer);
            displayer.onAdsShowFailed += () => OnAdsShowFailed(displayer);
            displayer.onAdsShowCompleted += () => OnAdsShowComplete(displayer);
            displayer.onAdsClicked += () => OnAdsClicked(displayer);
            displayer.onAdsCancel += () => OnAdsCancel(displayer);
            displayer.onAdsLoadFailed += (code, message) => OnAdsLoadFailed(displayer, code, message);

            displayers.Add(displayer);
            displayers.SortAsc(d => d.Config.order);
        }

        /// <summary>
        /// Handle when a displayer have ads loaded
        /// </summary>
        /// <param name="displayer"></param>
        protected virtual void OnAdsLoaded(AdsDisplayer displayer) { Ads.Notify(displayer.Type); }
        protected virtual void OnAdsShow(AdsDisplayer displayer) { }
        protected virtual void OnAdsShowFailed(AdsDisplayer displayer) { }
        protected virtual void OnAdsShowComplete(AdsDisplayer displayer) { }
        protected virtual void OnAdsClicked(AdsDisplayer displayer) { }
        protected virtual void OnAdsCancel(AdsDisplayer displayer) { }
        protected virtual void OnAdsLoadFailed(AdsDisplayer displayer, int code, string message) { }

        internal abstract void Init();
        internal abstract void ShowAds();
        internal abstract void HideAds();

        public static implicit operator bool(AdsController exists) => exists != null;
    }
}