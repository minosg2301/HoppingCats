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
        FixFloatParse();

        if (!CustomRequestConsentInfo())
        {
#if UNITY_EDITOR
            Complete();
#else
#if UNITY_ANDROID || UNITY_IOS
            ConsentInfoUpdator.OnUpdateCompleted += OnUpdateConsentCompleted;
            ConsentInfoUpdator.Ins.UpdateConsentInfo();
#else
            Complete();
#endif
#endif
        } 
    }

    protected virtual bool CustomRequestConsentInfo()
    {
        return false;
    }

    protected void OnUpdateConsentCompleted(bool result)
    {
        ConsentInfoUpdator.OnUpdateCompleted -= OnUpdateConsentCompleted;

#if UNITY_IOS
        StartCoroutine(iOSAuthorizationRequest.RequestAuthorization());
#endif
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

    public static void FixFloatParse()
    {
        try
        {
            var currentCultureInfo = CultureInfo.CurrentCulture.Clone() as CultureInfo;
            currentCultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            currentCultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
            CultureInfo.CurrentCulture = currentCultureInfo;
        }
        catch (Exception) { }
    }
}