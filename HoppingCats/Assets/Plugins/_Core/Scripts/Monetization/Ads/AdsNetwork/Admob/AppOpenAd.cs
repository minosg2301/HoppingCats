#if USE_NATIVE_ADMOB
using System;
using UnityEngine;
using UnityEngine.Scripting;
using vgame.tracking;
using static vgame.ads.NativeAdmob;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace vgame.ads
{
    internal class AppOpenAd : AdsDisplayer
    {
        [Preserve]
        public AppOpenAd(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.APP_OPEN, adsNetwork, displayConfig) { }

        static int delayForSeconds = 90;

        internal override void Init()
        {
            AppOpenAdEvent.OnAdShowed += OnAdShowed;
            AppOpenAdEvent.OnAdLoadedFailed += OnAdLoadedFailed;
            AppOpenAdEvent.onAdsPaidEvent += OnPaidEvent;

#if !UNITY_EDITOR
#if UNITY_ANDROID
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "InitAppOpenAd", Placement, delayForSeconds);
#elif UNITY_IOS
            InitAppOpenAd_iOS(Placement, delayForSeconds);
#endif // UNITY_ANDROID
#endif // UNITY_EDITOR
        }

        private void OnAdShowed(string adPlacement)
        {
            TrackingManager.OnAppOpenAdShow();
        }

        void OnAdLoadedFailed(string adPlacement)
        {
            TrackingManager.OnAppOpenAdNoFill("Admob", adPlacement);
        }

        void OnPaidEvent(AdsValue adsValue)
        {
            TrackingManager.OnAdmobAdRevenue(adsValue);
        }

        public static void SetDisplayable(bool displayable)
        {
            int value = displayable ? 1 : 0;
#if !UNITY_EDITOR
#if UNITY_ANDROID
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "SetAppOpenAdsDisplayable", value);
#elif UNITY_IOS
            SetAppOpenAdsDisplayable_iOS(value);
#endif // UNITY_ANDROID
#endif // UNITY_EDITOR
        }

        public static void UpdateNextShowTime()
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            AndroidUtil.CallJavaStaticMethod(admobAdsHelperClassName, "UpdateAppOpenAdsTime");
#elif UNITY_IOS
            UpdateAppOpenAdsTime_iOS();
#endif // UNITY_ANDROID
#endif // UNITY_EDITOR
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void InitAppOpenAd_iOS(string placement, int delayForSeconds);

        [DllImport("__Internal")]
        private static extern void UpdateAppOpenAdsTime_iOS();
        
        [DllImport("__Internal")]
        private static extern void SetAppOpenAdsDisplayable_iOS(int value);
#endif

        internal override void Hide() { }
        internal override void Load() { }
        internal override void Show() { }
    }
}
#else
namespace moonNest.ads
{
    internal class AppOpenAd : AdsDisplayer
    {
        public AppOpenAd(AdsNetwork adsNetwork, DisplayConfig displayConfig)
            : base(AdsType.APP_OPEN, adsNetwork, displayConfig)
        {
        }

        public static void SetDisplayable(bool displayable) { }
        public static void UpdateNextShowTime() { }

        internal override void Init() { }
        internal override void Hide() { }
        internal override void Load() { }
        internal override void Show() { }
    }
}

#endif