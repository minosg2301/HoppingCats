using System;
using System.Collections.Generic;

namespace moonNest.ads
{
    [Serializable]
    public class AdsConfig
    {
        public bool maxSDKDebug;
        public float adsReloadInterval = 3f;
        public bool skipAds = true;
        public float skipAdsDuration = 90;

        public bool simulateAdsTest;
        public bool simulateShowFailed;
        public bool simulateNoInterstitial;
        public bool simulateNoRewardAds;
        public bool simulateNoRewardInterAds;

        public NativeAdsConfig nativeAds = new NativeAdsConfig();

        public List<AdsNetworkConfig> networks = new List<AdsNetworkConfig>();
        public AdsPlatformConfig ios = new AdsPlatformConfig();
        public AdsPlatformConfig android = new AdsPlatformConfig();
        public AdsPlatformConfig web = new AdsPlatformConfig();
        public AdsPlatformConfig huawei = new AdsPlatformConfig();

        public AdsNetworkConfig FindAdsNetwork(AdsNetworkID adsNetwork) => networks.Find(a => a.id == adsNetwork);

        public AdsPlatformConfig AdsPlatform =>
#if UNITY_HUAWEI
            huawei;
#elif UNITY_IOS
            ios;
#elif UNITY_WEBGL
            web;
#else // default is android
            android;
#endif
    }

    [Serializable]
    public class NativeAdsConfig
    {
        //public bool delayRefresh;
        public bool noDelayRefresh;
        public int delayRefreshSeconds = 30;
    }

    [Serializable]
    public class AdsNetworkConfig
    {
        public AdsNetworkID id;
        public bool enabled = false;
        public bool enableSDKKey = true;
        public PlatformConfig ios = new();
        public PlatformConfig android = new();
        public PlatformConfig web = new();

        public PlatformConfig PlatformConfig =>
#if UNITY_HUAWEI
            huawei;
#elif UNITY_IOS
            ios;
#elif UNITY_WEBGL
            web;
#else // default is android
            android;
#endif
    }

    [Serializable]
    public class AdsPlatformConfig
    {
        public List<AdsFormatConfig> formats = new List<AdsFormatConfig>();

        // for editor
        public List<NetworkUsage> usages = new List<NetworkUsage>();

        public AdsFormatConfig FindAdsFormat(AdsType type) => formats.Find(a => a.type == type);
    }

    [Serializable]
    public struct AdsRect
    {
        public float width;
        public float height;

        // for android
        public float top;
        public float left;

        // for ios
        public float x;
        public float y;

        public AdsRect(float x, float y, float width, float height)
        {
            this.left = x;
            this.top = y;
            this.width = width;
            this.height = height;
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"(x:{x}, y:{y}, width:{width}, height:{height})";
        }
    }

    [Serializable]
    public class NetworkUsage
    {
        public AdsNetworkID id;
        public List<FormatUsage> formats = new List<FormatUsage>();

        public Dictionary<AdsType, FormatUsage> _dict;
        public Dictionary<AdsType, FormatUsage> Formats
        {
            get
            {
                if (_dict == null || _dict.Count == 0)
                {
                    _dict = new Dictionary<AdsType, FormatUsage>();
                    foreach (var format in formats)
                    {
                        _dict[format.type] = format;
                    }
                }

                return _dict;
            }
        }
    }

    [Serializable]
    public class FormatUsage
    {
        public AdsType type;
        public ORDER order = ORDER.__;

        public enum ORDER { __ = 0, MAIN = 1, BACKUP }
    }

    [Serializable]
    public class AdsFormatConfig : ICloneable
    {
        public AdsType type;
        public List<DisplayConfig> displays = new List<DisplayConfig>();

        public object Clone()
        {
            return null;
        }
    }

    [Serializable]
    public class PlatformConfig
    {
        public string sdkKey = "";
    }

    [Serializable]
    public class AdTypeConfig
    {
        // remove later
        public string placement;
        public int value;
    }

    [Serializable]
    public class AdsActiveConfig
    {
        // remove later
        internal object adsNetwork;
    }

    [Serializable]
    public class DisplayConfig
    {
        public AdsNetworkID networkId;
        public int order = 1;
        public string placement = "";
        public string placement2nd = "";

        public bool useMultiPlacements = false;
        public List<string> placements = new();
        public List<string> placement2nds = new();

#if IS_USE_AMAZON_MEDIATION && USE_AMAZON_SDK
        public string amazonSlotId = "";
#endif

        public override string ToString() => networkId + "|" + order;
    }
}