#if USE_GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
using UnityEngine;

namespace vgame.ads
{
    internal class AdmobInterstitial : AutoHideDisplayer
    {
        [Preserve]
        public AdmobInterstitial(AdsNetwork adsNetwork, DisplayConfig displayConfig) : base(AdsType.INTERSTITIAL, adsNetwork, displayConfig)
        {
        }

        internal override bool Available => throw new System.NotImplementedException();

        internal override void Init()
        {
        }

        internal override void Load()
        {
        }

        internal override void Show()
        {
        }
    }
}
#endif