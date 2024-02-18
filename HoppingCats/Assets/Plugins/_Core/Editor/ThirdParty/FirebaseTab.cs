namespace moonNest.ads
{
    internal class FirebaseTab : TabContent
    {
        public override void DoDraw()
        {
            Draw.BeginVertical(Draw.SubContentStyle);

            var thirdPartyConfig = ThirdPartyConfig.Ins.firbaseConfig;
            thirdPartyConfig.dynamicLinkDomain = Draw.TextField("Link Domain", thirdPartyConfig.dynamicLinkDomain, 250);
            thirdPartyConfig.dynamicLinkPrefix = Draw.TextField("Link Prefix", thirdPartyConfig.dynamicLinkPrefix, 250);

            Draw.EndVertical();
        }
    }
}