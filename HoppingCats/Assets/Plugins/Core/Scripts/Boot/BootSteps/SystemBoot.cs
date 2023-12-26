using I2.Loc;
using System;
using System.Globalization;
using UnityEngine;
using moonNest;

public class SystemBoot : BootStep
{
    public override string ToString() => "System Boot";

    void Reset()
    {
        CalledOnlyOnce = true;
    }

    public override void OnStep()
    {
        LocalizationManager.Callback_AllowSyncFromGoogle = AllowSyncFromGoogle;
        LocalizationManager.InitializeIfNeeded();
        FixThaiCalendar();

//        FirebaseManager.Init();
//        TrackingManager.Init();
//#if USE_FACEBOOK_SDK
//        FBManager.Ins.Init();
//#endif
//        Ads.Init();
//#if UNITY_IOS && !UNITY_EDITOR
//        iOSAdsSetting.Init();
//#endif

//        PushNotification.Init();
//        GameRating.Init();

        Complete();
    }

    bool AllowSyncFromGoogle(LanguageSourceData source)
    {
#if UNITY_WEBGL
        return false;
#else
        return true;
#endif
    }

    /// <summary>
    /// On devices use Thai Calendar, MonthNames Length is less than 13. It cause IndexOutOfRangeException.
    /// To fix this, we force game use en-US calendar for instead
    /// </summary>
    public static void FixThaiCalendar()
    {
        if (CultureInfo.CurrentCulture.Name == "th-TH")
        {
            try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US"); }
            catch (Exception e) { Debug.LogError(e.Message); }
        }
    }
}
