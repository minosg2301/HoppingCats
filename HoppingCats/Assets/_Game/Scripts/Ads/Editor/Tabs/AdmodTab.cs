using GoogleMobileAds.Editor;
using moonNest;
using UnityEditor;
using UnityEngine;

public class AdmodTab : TabContent
{
    private AdsConfig adsConfig;
    private GoogleMobileAdsSetAppId gmasai;

    public AdmodTab()
    {
        adsConfig = AdsConfig.Ins;
        gmasai = new GoogleMobileAdsSetAppId();
    }

    public override void DoDraw()
    {
        Undo.RecordObject(adsConfig, "Admod Config");
        DoDrawContent();
        if (GUI.changed) Draw.SetDirty(adsConfig);
    }

    private void DoDrawContent()
    {
        gmasai.GoogleMobileAdsAndroidAppId = Draw.TextField("AppID", gmasai.GoogleMobileAdsAndroidAppId, 500);
        Draw.Space(50);
        adsConfig.admodInterID = Draw.TextField("Inter ID", adsConfig.admodInterID, 500);
        adsConfig.admodRewardID = Draw.TextField("Reward ID", adsConfig.admodRewardID, 500);
    }
}
