using UnityEngine;
using System;
using System.Linq;

public static class ApplicationExt
{
#if UNITY_ANDROID
    const string appStoreUrl = "market://details?id={0}";
#elif UNITY_IOS
    const string appStoreUrl = "itms-apps://itunes.apple.com/app/id{0}";
    const string appStoreReviewUrl = "itms-apps://itunes.apple.com/app/id{0}?action=write-review";
#endif

    public static bool IsJailBroken()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            var paths = new[]
            {
                "/Applications/Cydia.app",
                "/private/var/lib/cydia",
                "/private/var/tmp/cydia.log",
                "/System/Library/LaunchDaemons/com.saurik.Cydia.Startup.plist",
                "/usr/libexec/sftp-server",
                "/usr/bin/sshd",
                "/usr/sbin/sshd",
                "/Applications/FakeCarrier.app",
                "/Applications/SBSettings.app",
                "/Applications/WinterBoard.app",
                "/Library/MobileSubstrate/MobileSubstrate.dylib",
                "/etc/apt",
                "/bin/hash",
                "/private/var/lib/apt/"
            };

            return paths.Any(System.IO.File.Exists);
        }
        catch(Exception e)
        {
            Debug.LogErrorFormat("Can not detect system files. Considering not jailed: {0}", e.Message);
        }
#endif
        return false;
    }

    public static bool IsEmulator()
    {
        return false;
    }

    public static void OpenStore(string identifier = "")
    {
#if UNITY_ANDROID
        Application.OpenURL(string.Format(appStoreUrl, !string.IsNullOrEmpty(identifier) ? identifier : Application.identifier));
#elif UNITY_IOS
        Application.OpenURL(string.Format(appStoreUrl, vgame.GlobalConfig.Ins.iosAppId));
#endif
    }

    public static void OpenStoreReview()
    {
#if UNITY_ANDROID
        Application.OpenURL(string.Format(appStoreUrl, Application.identifier));
#elif UNITY_IOS
        if(!UnityEngine.iOS.Device.RequestStoreReview())
            Application.OpenURL(string.Format(appStoreReviewUrl, vgame.GlobalConfig.Ins.iosAppId));
#endif
    }
}