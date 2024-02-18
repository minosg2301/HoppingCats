#if USE_NATIVE_ADMOB
using UnityEngine;
using System.Collections.Generic;
using vgame.tracking;
using static vgame.ads.NativeAdmob;
using UnityEngine.Scripting;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace vgame.ads
{
    internal class AdmobNative : NativeAdsDisplayer
    {
        [Preserve]
        public AdmobNative(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.NATIVE, adsNetwork, displayConfig) { }

        readonly Dictionary<NativeAdsType, bool> adsReady = new Dictionary<NativeAdsType, bool>()
        {{NativeAdsType.SMALL, false },
        {NativeAdsType.MEDIUM, false }};

        void AddEvents()
        {
            NativeAdsEvent.OnAdLoaded += OnAdLoaded;
            NativeAdsEvent.OnAdLoadedFailed += OnAdLoadedFailed;
            NativeAdsEvent.onAdsPaidEvent += OnPaidEvent;
        }

        void OnAdLoadedFailed(string key, string adPlacement)
        {
            var adsType = (NativeAdsType)int.Parse(key);
            adsReady[adsType] = false;
            TrackingManager.OnNativeLoadFailed(AdsNetwork.ToString(), adPlacement);
        }

        void OnAdLoaded(string key)
        {
            var adsType = (NativeAdsType)int.Parse(key);
            adsReady[adsType] = true;
            onAdsLoaded();
        }

        void OnPaidEvent(AdsValue adsValue)
        {
            TrackingManager.OnAdmobAdRevenue(adsValue);
        }

        internal override void Load() { } // not used

#if UNITY_EDITOR
        internal override bool IsAdsReady(NativeAdsType adsType) => adsReady[adsType];
        internal override void Init() { adsReady[NativeAdsType.SMALL] = true; adsReady[NativeAdsType.MEDIUM] = true; }
        internal override void Load(NativeAdsType adsType) { adsReady[adsData.type] = true; }
        internal override void Show() { adsReady[adsData.type] = false; }
        internal override void Hide() { adsReady[adsData.type] = true; }
#else
#if UNITY_ANDROID

        internal override bool IsAdsReady(NativeAdsType adsType)
        {
            return AndroidUtil.CallJavaStaticMethod<bool>(admobAdsHelperClassName, "IsNativeAdsReady", (int)adsType);
        }

        internal override void Init()
        {
            AddEvents();

            if(Ads.adsConfig.simulateAdsTest)
            {
                AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "InitNativeAds",
                    new string[] { "ca-app-pub-3940256099942544/2247696110" },
                    new string[] { "ca-app-pub-3940256099942544/2247696110" });
                return;
            }

            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "InitNativeAds", Placements, Placement2nds);
        }

        internal override void Load(NativeAdsType adsType)
        {
            //AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "LoadNativeAds", (int)adsType);
        }

        internal override void Show()
        {
            TrackingManager.OnNativeShow();

            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "ShowNativeAds",
                (int)adsData.type,
                Mathf.FloorToInt(adsData.rect.top),
                Mathf.FloorToInt(adsData.rect.left),
                Mathf.FloorToInt(adsData.rect.width),
                Mathf.FloorToInt(adsData.rect.height),
                "#" + ColorUtility.ToHtmlStringRGB(adsData.textColor),
                "#" + ColorUtility.ToHtmlStringRGB(adsData.actionColor));
        }

        internal override void Hide()
        {
            adsReady[adsData.type] = false;
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "HideNativeAds", (int)adsData.type);
        }

#elif UNITY_IOS
        
        internal override bool IsAdsReady(NativeAdsType adsType) => IsNativeAdsReady_iOS(((int)adsType).ToString());

        internal override void Init()
        {
            AddEvents();

            if (Ads.adsConfig.simulateAdsTest)
            {
                InitNativeAds_iOS("ca-app-pub-3940256099942544/3986624511", "ca-app-pub-3940256099942544/3986624511");
                return;
            }

            InitNativeAds_iOS(Placement, Placement2nd);
        }

        internal override void Load(NativeAdsType adsType)
        {
            LoadNativeAds_iOS(((int)adsType).ToString());
        }

        internal override void Show()
        {
            TrackingManager.OnNativeShow();

            string posX = adsData.rect.x.ToString().Replace(",",".");
            string posY = adsData.rect.y.ToString().Replace(",",".");
            string width = adsData.rect.width.ToString().Replace(",",".");
            string height = adsData.rect.height.ToString().Replace(",",".");

            ShowNativeAds_iOS(((int)adsData.type).ToString(), posX, posY, width, height,
                "#" + ColorUtility.ToHtmlStringRGB(adsData.textColor),
                "#" + ColorUtility.ToHtmlStringRGB(adsData.actionColor));
        }

        internal override void Hide()
        {
            adsReady[adsData.type] = false;
            HideNativeAds_iOS(((int)adsData.type).ToString());
        }

        [DllImport("__Internal")]
        private static extern void InitNativeAds_iOS(string adUnitIdSmall, string adUnitIdBig);
        [DllImport("__Internal")]
        private static extern void ShowNativeAds_iOS(string nativeType, string nativeTop, string nativeLeft, string nativeWidth, string nativeHeight, string nativeTextColor, string nativeActionColor);
        [DllImport("__Internal")]
        private static extern void HideNativeAds_iOS(string nativeType);
        [DllImport("__Internal")]
        private static extern void LoadNativeAds_iOS(string nativeType);
        [DllImport("__Internal")]
        private static extern bool IsNativeAdsReady_iOS(string nativeType);

#endif // UNITY_ANDROID
#endif // UNITY_EDITOR
    }
}
#endif