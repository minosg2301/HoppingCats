#if USE_GOOGLE_MOBILE_ADS
namespace vgame.ads
{
    internal class AdmobRewardAds : AutoHideDisplayer
    {
        [Preserve]
        public AdmobRewardAds(AdsNetwork adsNetwork, DisplayConfig displayConfig) : base(AdsType.REWARDED, adsNetwork, displayConfig)
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