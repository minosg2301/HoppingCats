using UnityEngine;

namespace moonNest.ads
{
    public struct NativeAdsData
    {
        public NativeAdsType type;
        public AdsRect rect;
        public Color textColor;
        public Color actionColor;

        public NativeAdsData(NativeAdsType type)
        {
            this.type = type;
            this.rect = new AdsRect();
            this.textColor = Color.white;
            this.actionColor = Color.white;
        }

        public NativeAdsData(NativeAdsType type, AdsRect rect, Color textColor, Color actionColor)
        {
            this.type = type;
            this.rect = rect;
            this.textColor = textColor;
            this.actionColor = actionColor;
        }
    }
}