namespace moonNest.ads
{
    internal abstract class AutoHideDisplayer : AdsDisplayer
    {
        internal AutoHideDisplayer(AdsType type, AdsNetwork network, DisplayConfig display)
            : base(type, network, display)
        {
        }

        internal override void Hide()
        {
            //Do Nothing
        }
    }
}