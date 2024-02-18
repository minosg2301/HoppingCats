#if USE_FB_INSTANT_ADS
using UnityEngine.Scripting;

namespace vgame.ads
{
    internal class FBInstantBanner : AutoHideDisplayer
    {
        [Preserve]
        public FBInstantBanner(AdsNetwork network, DisplayConfig display)
            : base(AdsType.BANNER, network, display)
        {
        }

        internal override bool Available => true;

        internal override void Init()
        {
        }

        internal override void Load()
        {
            
        }

        internal override void Show()
        {
            FBInstantPlugin.LoadBanner(Placement);
        }

        internal override void Hide()
        {
            FBInstantPlugin.HideBanner();
        }
    }
}
#endif