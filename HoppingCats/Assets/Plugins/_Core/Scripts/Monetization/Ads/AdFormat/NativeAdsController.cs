using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest.ads
{
    internal class NativeAdsController : AdsController
    {
        Dictionary<NativeAdsType, bool> showings = new Dictionary<NativeAdsType, bool>()
        {
            {NativeAdsType.SMALL, false },
            {NativeAdsType.MEDIUM, false }
        };

        NativeAdsDisplayer Displayer => (NativeAdsDisplayer)(displayers.Count > 0 ? displayers[0] : null);

        public NativeAdsController() : base(AdsType.NATIVE) { }

        internal override void Init() { }

        internal void ShowAds(NativeAdsData adsData)
        {
            if (showings[adsData.type] || !Displayer) return;

            Displayer.adsData = adsData;
            Displayer.Show();
            showings[adsData.type] = true;
            onAdsShow(Displayer);
        }

        internal void HideAds(NativeAdsData adsData)
        {
            if (!showings[adsData.type]) return;

            showings[adsData.type] = false;
            Displayer.adsData = adsData;
            Displayer.Hide();
            onAdsHide(Displayer);
        }

        internal override void ShowAds() { }
        internal override void HideAds() { }

        internal override void AddDisplayer(AdsDisplayer displayer)
        {
            if (!(displayer is NativeAdsDisplayer))
            {
                Debug.LogError("NativeAdsController only accept NativeAdsDisplayer");
                return;
            }

            base.AddDisplayer(displayer);
        }

        internal bool IsAdReady(NativeAdsType adsType)
        {
            return Displayer && Displayer.IsAdsReady(adsType);
        }

        //internal void LoadAds(NativeAdsType adsType)
        //{
        //    if (Displayer) Displayer.Load(adsType);
        //}
    }
}