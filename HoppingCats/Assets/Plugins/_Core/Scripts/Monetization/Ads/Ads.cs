using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace moonNest.ads
{
    public class Ads : SingletonMono<Ads>, IObservable
    {
        #region public interface
        public static Action<bool> onBannerVisibilityChanged = delegate { };

        public delegate void AdsEvent(string adsNetwork, string adPlacement);
        public delegate void AdsEventError(string adsNetwork, string adPlacement, int code, string message);

        public static event AdsEvent OnRewardedAdLoaded = delegate { };
        public static event AdsEvent OnRewardedAdShowed = delegate { };
        public static event AdsEvent OnRewardedAdShowFailed = delegate { };
        public static event AdsEvent OnRewardedAdClicked = delegate { };
        public static event AdsEvent OnRewardedAdCompleted = delegate { };
        public static event AdsEvent OnRewardedAdCanceled = delegate { };
        public static event AdsEventError OnRewardedAdLoadFailed = delegate { };

        public static event AdsEvent OnInterAdLoaded = delegate { };
        public static event AdsEvent OnInterAdShowed = delegate { };
        public static event AdsEvent OnInterAdClicked = delegate { };
        public static event AdsEventError OnInterAdLoadFailed = delegate { };

        public static float InterAdSkipDuration { get; set; }
        public static bool InterAdSkipEnabled { get; set; }
        public static bool AppOpenAdSkipEnabled { get; set; }
        public static bool ForceShowInterstitial { get; set; }

        internal static bool DebugMode { get; private set; }
        internal static bool GDPRConsent { get; set; }

        /// <summary>
        /// Short cut for init ads
        /// </summary>
        public static void Init() { Ins.Init_Internal(); }

        /// <summary>
        /// Show Banner Ads
        /// </summary>
        public static void ShowBanner() => Ins.ShowAds(AdsType.BANNER);

        /// <summary>
        /// Show Interstitial Ads
        /// </summary>
        /// <param name="onCompleted"></param>
        public static void ShowInterstitial()
        {
            CustomAction = "default";
            Ins.ShowInterstitial_Internal(null);
        }

        /// <summary>
        /// Show Interstitial Ads
        /// </summary>
        /// <param name="onCompleted"></param>
        public static void ShowInterstitial(Action<bool> onDone)
        {
            CustomAction = "default";
            Ins.ShowInterstitial_Internal(onDone);
        }

        /// <summary>
        /// Show Interstitial Ads
        /// </summary>
        /// <param name="action">tracking action</param>
        /// <param name="force">force show with no condition</param>
        public static void ShowInterstitial(string action, Action<bool> onDone = null)
        {
            CustomAction = action;
            Ins.ShowInterstitial_Internal(onDone);
        }

        /// <summary>
        /// Show Rewarded Ads
        /// </summary>
        /// <param name="onCompleted"></param>
        public static void ShowRewardAds(Action<bool> onCompleted = null)
        {
            CustomAction = "default";
            Ins.ShowAds(AdsType.REWARDED, onCompleted);
        }

        /// <summary>
        /// Show Rewarded Ads
        /// </summary>
        /// <param name="onCompleted"></param>
        public static void ShowRewardAds(string action, Action<bool> onCompleted = null)
        {
            CustomAction = action;
            Ins.ShowAds(AdsType.REWARDED, onCompleted);
        }

        /// <summary>
        /// Show Reward Inter
        /// </summary>
        /// <param name="onCompleted"></param>
        public static void ShowRewardInter(Action<bool> onCompleted = null)
        {
            CustomAction = "default";
            Ins.ShowAds(AdsType.RWD_INTER, onCompleted);
        }

        /// <summary>
        /// Show Reward Inter
        /// </summary>
        /// <param name="onCompleted"></param>
        public static void ShowRewardInter(string action, Action<bool> onCompleted = null)
        {
            CustomAction = action;
            Ins.ShowAds(AdsType.RWD_INTER, onCompleted);
        }

        /// <summary>
        /// Show Native Ads
        /// </summary>
        public static void ShowNative(NativeAdsData data) => Ins.ShowNative_Internal(data);

        /// <summary>
        /// Load ads methods
        /// </summary>
        //public static void LoadNative(NativeAdsType adsType) => Ins.LoadNative_Internal(adsType);

        /// <summary>
        /// Check native ads is ready
        /// </summary>
        /// <param name="adsType"></param>
        /// <returns></returns>
        public static bool IsNativeAdsReady(NativeAdsType adsType) => Ins.IsNativeAdsRead_Internal(adsType);

        // displayable methods
        public static void SetDisplayable(bool displayable)
        {
            if (!GlobalConfig.Ins.adsEnabled) return;

            adsDisplayable = displayable;
            if (Ins) observerProvider.Notify(Ins);
        }

        public static void SetDisplayable(AdsType adsType, bool displayable)
        {
            if (!GlobalConfig.Ins.adsEnabled) return;

            var controller = GetAdsController(adsType);
            controller.Displayable = displayable;
            Notify(adsType);
        }

        public static bool IsDisplayable() => adsDisplayable;
        public static bool IsDisplayable(AdsType adsType)
        {
            var controller = GetAdsController(adsType);
            return controller != null && controller.Displayable;
        }

        /// <summary>
        /// Get banner size in px
        /// </summary>
        public static float BannerSize => Ins.GetBannerSize();

        /// <summary>
        /// Hide ads methods
        /// </summary>
        public static void HideBanner() { Ins.HideAds(AdsType.BANNER); }

        /// <summary>
        /// Hide native ads
        /// </summary>
        public static void HideNative(NativeAdsData data) { Ins.HideNative_Internal(data); }

        /// <summary>
        /// Check ads is available by ads type
        /// </summary>
        /// <param name="adsType"></param>
        public static bool IsAvailable(AdsType adsType)
        {
            return Ins.IsAvailable_Internal(adsType);
        }

        /// <summary>
        /// Check Ads is showing
        /// </summary>
        /// <param name="adsType"></param>
        /// <returns></returns>
        public static bool IsShowing(AdsType adsType)
        {
            return Ins.IsShowing_Internal(adsType);
        }

        // observers methods
        public static void Subscribe(Action<Ads> handler, bool notifyInstant = true)
        {
            observerProvider.Subcribe("", handler);
            if (Ins && notifyInstant) observerProvider.Notify(Ins);
        }

        public static void Subscribe(AdsType type, Action<Ads> handler, bool notifyInstant = true)
        {
            observerProvider.Subcribe(type.ToString(), handler);
            if (Ins && notifyInstant) observerProvider.Notify(Ins, type.ToString());
        }

        public static void Unsubscribe(Action<Ads> handler)
        {
            observerProvider.Unsubscribe("", handler);
        }

        public static void Unsubscribe(AdsType type, Action<Ads> handler)
        {
            observerProvider.Unsubscribe(type.ToString(), handler);
        }
        #endregion

        #region internal interface

        #region static fields
        internal static AdsConfig adsConfig;
        internal static float ReloadInterval => adsConfig.adsReloadInterval;
        internal static string CustomAction { get; set; } = "default";

        static float currentTimeScale;

        internal static Action<bool> onApplicationPause;
        internal static Dictionary<AdsType, Action<bool>> showAdsCallbacks = new Dictionary<AdsType, Action<bool>>();
        internal static Action showInterCallback;
        internal static bool adsDisplayable = true;
        #endregion

        #region static methods
        internal static AdsNetworkConfig GetAdsNetworkConfig(AdsNetworkID networkId)
        {
            return ThirdPartyConfig.Ins.adsConfig.networks.Find(n => n.id == networkId);
        }

        internal static AdsNetwork GetAdsNetwork(AdsNetworkID networkId)
        {
            return Ins.adsNetworks.TryGetValue(networkId, out var adsNetwork) ? adsNetwork : null;
        }

        internal static AdsController GetAdsController(AdsType adsType)
        {
            return Ins.adsControllers.TryGetValue(adsType, out var controller) ? controller : null;
        }

        internal static void PauseGame()
        {
            MainThreadDispatcher.Enqueue(() =>
            {
                currentTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                AudioListener.pause = true;
            });
        }

        internal static void ResumeGame()
        {
            MainThreadDispatcher.Enqueue(() =>
            {
                Time.timeScale = currentTimeScale;
                AudioListener.pause = false;
            });
        }

        internal static void UpdateShowAdsComplete(AdsType adsType, bool completed = true)
        {
            if (showAdsCallbacks.TryGetValue(adsType, out var callback) && callback != null)
                showAdsCallbacks[adsType](completed);
        }

        // observers
        static readonly ObserverProvider<Ads> observerProvider = new ObserverProvider<Ads>();
        internal static void Notify(AdsType type)
        {
            if (Ins) observerProvider.Notify(Ins, type.ToString());
        }
        #endregion

        #region fields
        readonly Dictionary<AdsNetworkID, AdsNetwork> adsNetworks = new Dictionary<AdsNetworkID, AdsNetwork>();
        readonly Dictionary<AdsType, AdsController> adsControllers = new Dictionary<AdsType, AdsController>();
        #endregion

        #region methods
        void OnApplicationPause(bool pause)
        {
            onApplicationPause?.Invoke(pause);
        }

        internal void Init_Internal()
        {
            if (!GlobalConfig.Ins.adsEnabled)
            {
                adsDisplayable = false;
                return;
            }

            DebugMode = GlobalConfig.Ins.DebugLog;
            adsConfig = ThirdPartyConfig.Ins.adsConfig;

            InterAdSkipDuration = adsConfig.skipAdsDuration;
            InterAdSkipEnabled = adsConfig.skipAds;
            AppOpenAdSkipEnabled = adsConfig.skipAds;

            // create ads networks we enable in config
            foreach (var network in adsConfig.networks)
            {
                if (!network.enabled) continue;

                try { adsNetworks.GetOrCreate(network.id, id => AdsFactory.CreateAdsNetwork(network)); }
                catch (KeyNotFoundException e) { Debug.LogWarning(e.Message); }
                catch (InvalidCastException e) { Debug.LogError(e.Message); }
            }

            // init ads network
            adsNetworks.Values.ForEach(ads => ads.Init());

            // get formats from exactly platform
            var adsFormatConfigs = adsConfig.AdsPlatform.formats;

            foreach (var formatConfig in adsFormatConfigs)
            {
                AdsController controller = null;
                try { controller = adsControllers.GetOrCreate(formatConfig.type, t => AdsFactory.CreateAdsController(t)); }
                catch (KeyNotFoundException e) { Debug.LogWarning(e.Message); }
                catch (InvalidCastException e) { Debug.LogError(e.Message); }

                if (controller == null) continue;

                foreach (var displayConfig in formatConfig.displays)
                {
                    if (adsNetworks.TryGetValue(displayConfig.networkId, out var adsNetwork))
                    {
                        try
                        {
                            // register displayer to network
                            var adsDisplayer = adsNetwork.RegisterDisplayer(formatConfig.type, displayConfig);

                            // add displayer to controller
                            controller.AddDisplayer(adsDisplayer);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.Message);
                        }
                    }
                }
            }
        }

        internal void ShowAds(AdsType adsType, Action<bool> onCompleted = null)
        {
            if (!GetAdsControllerIfDisplayable(adsType, out var controller))
                return;

            // add callback
            showAdsCallbacks[adsType] = onCompleted;

            // use controller to show ads
            controller.ShowAds();
        }

        bool IsNativeAdsRead_Internal(NativeAdsType adsType)
        {
            // check ads controller valid
            if (!adsControllers.TryGetValue(AdsType.NATIVE, out var controller)) return false;

            // show with ads data
            return (controller as NativeAdsController).IsAdReady(adsType);
        }

        //void LoadNative_Internal(NativeAdsType adsType)
        //{
        //    // check ads controller valid
        //    if (!adsControllers.TryGetValue(AdsType.NATIVE, out var controller)) return;

        //    (controller as NativeAdsController).LoadAds(adsType);
        //}

        void ShowNative_Internal(NativeAdsData data)
        {
            if (!GetAdsControllerIfDisplayable(AdsType.NATIVE, out var controller))
                return;

            // show with ads data
            (controller as NativeAdsController).ShowAds(data);
        }

        void HideNative_Internal(NativeAdsData data)
        {
            var adsType = AdsType.NATIVE;

            // check ads controller valid
            if (!adsControllers.TryGetValue(adsType, out var controller)) return;

            // show with ads data
            (controller as NativeAdsController).HideAds(data);
        }

        void ShowInterstitial_Internal(Action<bool> onCompleted)
        {
            var adsType = AdsType.INTERSTITIAL;

            if (!GetAdsControllerIfDisplayable(adsType, out var controller))
            {
                onCompleted?.Invoke(false);
                return;
            }

            // add callback
            showAdsCallbacks[adsType] = onCompleted;

            // show with ads data
            var c = controller as InterstitialController;
            c.forceShow = ForceShowInterstitial;
            (controller as InterstitialController).ShowAds();
            c.forceShow = ForceShowInterstitial = false;
        }

        internal void HideAds(AdsType adsType)
        {
            if (!adsControllers.TryGetValue(adsType, out var controller)) return;

            controller.HideAds();
        }

        internal bool IsAvailable_Internal(AdsType adsType)
        {
            return adsControllers.TryGetValue(adsType, out var controller) && controller.AdsAvailable;
        }

        internal bool IsShowing_Internal(AdsType adsType)
        {
            return adsControllers.TryGetValue(adsType, out var controller) && controller.IsShowing;
        }

        internal float GetBannerSize()
        {
            return adsControllers.TryGetValue(AdsType.BANNER, out var controller) ? controller.DisplaySize.y : 0;
        }

        bool GetAdsControllerIfDisplayable(AdsType adsType, out AdsController controller)
        {
            controller = null;

            // check global flag can show ads
            if (!GlobalConfig.Ins.adsEnabled || !adsDisplayable) return false;

            // check ads controller valid
            return adsControllers.TryGetValue(adsType, out controller) && controller.Displayable;
        }
        #endregion

        #region corountine
        internal static Coroutine CallDelay(float delay, Action action)
        {
            IEnumerator CallDelayInstance()
            {
                yield return new WaitForSecondsRealtime(delay);
                action();
            }

            return Ins.StartCoroutine(CallDelayInstance());
        }
        #endregion

        #endregion

        #region Event Notify
        internal static void NotifyRewardedAdLoaded(string adNetwork, string adPlacement)
            => OnRewardedAdLoaded(adNetwork, adPlacement);
        internal static void NotifyRewardedAdLoadFailed(string adNetwork, string adPlacement, int code, string message)
            => OnRewardedAdLoadFailed(adNetwork, adPlacement, code, message);
        internal static void NotifyRewardedAdShowed(string adNetwork, string adPlacement)
            => OnRewardedAdShowed(adNetwork, adPlacement);
        internal static void NotifyRewardedAdShowFailed(string adNetwork, string adPlacement)
            => OnRewardedAdShowFailed(adNetwork, adPlacement);
        internal static void NotifyRewardedAdClicked(string adNetwork, string adPlacement)
            => OnRewardedAdClicked(adNetwork, adPlacement);
        internal static void NotifyRewardedAdCompleted(string adNetwork, string adPlacement)
            => OnRewardedAdCompleted(adNetwork, adPlacement);
        internal static void NotifyRewardedAdCanceled(string adNetwork, string adPlacement)
            => OnRewardedAdCanceled(adNetwork, adPlacement);

        internal static void NotifyInterAdLoaded(string adNetwork, string adPlacement)
            => OnInterAdLoaded(adNetwork, adPlacement);
        internal static void NotifyInterAdLoadFailed(string adNetwork, string adPlacement, int code, string message)
            => OnInterAdLoadFailed(adNetwork, adPlacement, code, message);
        internal static void NotifyInterAdShowed(string adNetwork, string adPlacement)
            => OnInterAdShowed(adNetwork, adPlacement);
        internal static void NotifyInterAdClicked(string adNetwork, string adPlacement)
            => OnInterAdClicked(adNetwork, adPlacement);
        #endregion

        #region Android
#if !UNITY_EDITOR && UNITY_ANDROID
        internal static void CallJavaFunc(string func, params object[] obj)
        {
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call("runOnUiThread", new AndroidJavaRunnable(() => activity.Call(func, obj)));
        }
#endif
        #endregion
    }
}