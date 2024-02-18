using UnityEditor;
using moonNest.editor;

namespace moonNest.ads
{
    internal class DefineSymbolsTab : TabContent
    {
        private TabContainer tabContainer;

        public DefineSymbolsTab()
        {
            var mobiles = new BuildTarget[] { BuildTarget.Android, BuildTarget.iOS };
            var web = new BuildTarget[] { BuildTarget.WebGL };
            tabContainer = new TabContainer();
            tabContainer.AddTab("Mobile", new DefineSymbolTab(ThirdPartyConfig.Ins.mobileDefine, mobiles));
            tabContainer.AddTab("Web GL", new DefineSymbolTab(ThirdPartyConfig.Ins.webGLDefine, web));

#if UNITY_WEBGL
            tabContainer.SetSelectedTab(1);
#endif
        }

        public override void DoDraw()
        {
            tabContainer.DoDraw();
        }
    }

    internal class DefineSymbolTab : TabContent
    {
        readonly SymbolDefined defined;
        readonly BuildTarget[] targets;

        public DefineSymbolTab(SymbolDefined defined, BuildTarget[] targets)
        {
            this.defined = defined;
            this.targets = targets;
        }

        public override void DoDraw()
        {
            Draw.BeginVertical(Draw.SubContentStyle);
            DrawDefineGoogleMobileAds();
            DrawDefineIronSourceAds();
            DrawDefineUnityAds();
            DrawDefineNativeAdmob();
            DrawDefineAppFlyer();
            DrawDefineAdjust();
            DrawDefineGameAnalytics();
            DrawDefineMaxSDK();
            DrawDefineFacebookSDK();
            DrawDefineFBInstantAds();
            DrawDefineFBInstant();
            DrawDefineLionSDK();
            DrawDefineAmazonSDK();
            DrawDefineISAmazonMediation();
            Draw.EndVertical();
        }

        bool ToggleField(string name, bool value, int maxWidth)
        {
            Draw.BeginHorizontal();
            Draw.Label(name, 200);
            bool val = Draw.Toggle(value, maxWidth);
            Draw.EndHorizontal();
            return val;
        }

        void DrawDefineGoogleMobileAds()
        {
            Draw.BeginChangeCheck();
            defined.useGoogleMobileAds = ToggleField("USE_GOOGLE_MOBILE_ADS", defined.useGoogleMobileAds, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useGoogleMobileAds) DrawUtilities.AddSymbols(targets, "USE_GOOGLE_MOBILE_ADS");
                else DrawUtilities.RemoveSymbols(targets, "USE_GOOGLE_MOBILE_ADS");
            }
        }

        void DrawDefineIronSourceAds()
        {
            Draw.BeginChangeCheck();
            defined.useIronSrcAds = ToggleField("USE_IRONSRC_ADS", defined.useIronSrcAds, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useIronSrcAds) DrawUtilities.AddSymbols(targets, "USE_IRONSRC_ADS");
                else DrawUtilities.RemoveSymbols(targets, "USE_IRONSRC_ADS");
            }
        }

        void DrawDefineUnityAds()
        {
            Draw.BeginChangeCheck();
            defined.useUnityAds = ToggleField("USE_UNITY_ADS", defined.useUnityAds, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useUnityAds) DrawUtilities.AddSymbols(targets, "USE_UNITY_ADS");
                else DrawUtilities.RemoveSymbols(targets, "USE_UNITY_ADS");
            }
        }

        void DrawDefineNativeAdmob()
        {
            Draw.BeginChangeCheck();
            defined.useNativeAdmob = ToggleField("USE_NATIVE_ADMOB", defined.useNativeAdmob, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useNativeAdmob) DrawUtilities.AddSymbols(targets, "USE_NATIVE_ADMOB");
                else DrawUtilities.RemoveSymbols(targets, "USE_NATIVE_ADMOB");
            }
        }

        void DrawDefineGameAnalytics()
        {
            Draw.BeginChangeCheck();
            defined.useGameAnalytic = ToggleField("USE_GAME_ANALYTICS", defined.useGameAnalytic, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useGameAnalytic) DrawUtilities.AddSymbols(targets, "USE_GAME_ANALYTICS");
                else DrawUtilities.RemoveSymbols(targets, "USE_GAME_ANALYTICS");
            }
        }

        void DrawDefineAppFlyer()
        {
            Draw.BeginChangeCheck();
            defined.useAppFlyer = ToggleField("USE_APPS_FLYER", defined.useAppFlyer, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useAppFlyer) DrawUtilities.AddSymbols(targets, "USE_APPS_FLYER");
                else DrawUtilities.RemoveSymbols(targets, "USE_APPS_FLYER");
            }
        }

        void DrawDefineAdjust()
        {
            Draw.BeginChangeCheck();
            defined.useAdjust = ToggleField("USE_ADJUST", defined.useAdjust, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useAdjust) DrawUtilities.AddSymbols(targets, "USE_ADJUST");
                else DrawUtilities.RemoveSymbols(targets, "USE_ADJUST");
            }
        }

        void DrawDefineMaxSDK()
        {
            Draw.BeginChangeCheck();
            defined.useMaxSDK = ToggleField("USE_MAX_SDK", defined.useMaxSDK, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useMaxSDK) DrawUtilities.AddSymbols(targets, "USE_MAX_SDK");
                else DrawUtilities.RemoveSymbols(targets, "USE_MAX_SDK");
            }
        }

        void DrawDefineFacebookSDK()
        {
            Draw.BeginChangeCheck();
            defined.useFacebookSDK = ToggleField("USE_FACEBOOK_SDK", defined.useFacebookSDK, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useFacebookSDK) DrawUtilities.AddSymbols(targets, "USE_FACEBOOK_SDK");
                else DrawUtilities.RemoveSymbols(targets, "USE_FACEBOOK_SDK");
            }
        }

        void DrawDefineFBInstantAds()
        {
            Draw.BeginChangeCheck();
            defined.useFBInstantAds = ToggleField("USE_FB_INSTANT_ADS", defined.useFBInstantAds, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useFBInstantAds) DrawUtilities.AddSymbols(targets, "USE_FB_INSTANT_ADS");
                else DrawUtilities.RemoveSymbols(targets, "USE_FB_INSTANT_ADS");
            }
        }

        void DrawDefineFBInstant()
        {
            Draw.BeginChangeCheck();
            defined.useFBInstant = ToggleField("USE_FB_INSTANT", defined.useFBInstant, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useFBInstantAds) DrawUtilities.AddSymbols(targets, "USE_FB_INSTANT");
                else DrawUtilities.RemoveSymbols(targets, "USE_FB_INSTANT");
            }
        }

        void DrawDefineLionSDK()
        {
            Draw.BeginChangeCheck();
            defined.useLionSDK = ToggleField("USE_LION_SDK", defined.useLionSDK, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useLionSDK) DrawUtilities.AddSymbols(targets, "USE_LION_SDK");
                else DrawUtilities.RemoveSymbols(targets, "USE_LION_SDK");
            }
        }

        void DrawDefineAmazonSDK()
        {
            Draw.BeginChangeCheck();
            defined.useAmazonSDK = ToggleField("USE_AMAZON_SDK", defined.useAmazonSDK, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.useAmazonSDK) DrawUtilities.AddSymbols(targets, "USE_AMAZON_SDK");
                else DrawUtilities.RemoveSymbols(targets, "USE_AMAZON_SDK");
            }
        }

        void DrawDefineISAmazonMediation()
        {
            Draw.BeginChangeCheck();
            defined.isUseAmazonMediation = ToggleField("IS_USE_AMAZON_MEDIATION", defined.isUseAmazonMediation, 60);
            if (Draw.EndChangeCheck())
            {
                if (defined.isUseAmazonMediation) DrawUtilities.AddSymbols(targets, "IS_USE_AMAZON_MEDIATION");
                else DrawUtilities.RemoveSymbols(targets, "IS_USE_AMAZON_MEDIATION");
            }
        }

        /// <summary>
        /// Ironsource Ad Quality Deprecated on IS v750+
        /// 
        /// </summary>
        //void DrawDefineUseIronsrc750()
        //{
        //    Draw.BeginChangeCheck();
        //    defined.isUseIronsrc750 = ToggleField("USE_IRONSRC_750", defined.isUseIronsrc750, 60);
        //    if (Draw.EndChangeCheck())
        //    {
        //        if (defined.isUseIronsrc750) DrawUtilities.AddSymbols(targets, "USE_IRONSRC_750");
        //        else DrawUtilities.RemoveSymbols(targets, "USE_IRONSRC_750");
        //    }
        //}
    }
}