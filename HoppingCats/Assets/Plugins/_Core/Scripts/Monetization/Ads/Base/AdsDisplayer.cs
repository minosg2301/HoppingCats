using System;
using UnityEngine;

namespace moonNest.ads
{
    internal abstract class AdsDisplayer
    {
        internal protected bool IsAdReady { get; protected set; }
#if UNITY_EDITOR
        = true;
#else
        = false;
#endif
        internal AdsType Type { get; }
        internal AdsNetwork AdsNetwork { get; }
        internal DisplayConfig Config { get; }
        internal virtual Vector2 Size => Vector2.zero;

        internal delegate void AdsDisplayerEvent();
        internal AdsDisplayerEvent onAdsClicked = delegate { };
        internal AdsDisplayerEvent onAdsLoaded = delegate { };
        internal AdsDisplayerEvent onAdsShow = delegate { };
        internal AdsDisplayerEvent onAdsShowFailed = delegate { };
        internal AdsDisplayerEvent onAdsShowCompleted = delegate { };
        internal AdsDisplayerEvent onAdsCancel = delegate { };
        internal AdsDisplayerEvent onAdsHide = delegate { };

        internal delegate void AdsDisplayerEventError(int code, string message);
        internal AdsDisplayerEventError onAdsLoadFailed = delegate { };

        internal AdsDisplayer(AdsType type, AdsNetwork network, DisplayConfig display)
        {
            Type = type;
            AdsNetwork = network;
            Config = display;
        }

        #region abstract methods
        internal virtual bool Available => IsAdReady;

        internal abstract void Init();
        internal abstract void Load();
        internal abstract void Show();
        internal abstract void Hide();
        #endregion

        public string Placement => Config.placement;
        public string Placement2nd => Config.placement2nd;

        public string[] Placements => Config.useMultiPlacements
                                        ? Config.placements.ToArray()
                                        : string.IsNullOrEmpty(Config.placement)
                                            ? new string[] { }
                                            : new string[] { Config.placement };

        public string[] Placement2nds => Config.useMultiPlacements
                                            ? Config.placement2nds.ToArray()
                                            : string.IsNullOrEmpty(Config.placement2nd)
                                                ? new string[] { }
                                                : new string[] { Config.placement2nd };


        public static implicit operator bool(AdsDisplayer exists) => exists != null;
    }
}