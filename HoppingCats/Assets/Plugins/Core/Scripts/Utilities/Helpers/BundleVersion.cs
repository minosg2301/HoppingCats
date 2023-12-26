using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class BundleVersion
{
    public static int Code => GetVersionCode();

#if UNITY_EDITOR
#if UNITY_ANDROID
    static int GetVersionCode() => PlayerSettings.Android.bundleVersionCode;
#elif UNITY_IOS
    static int GetVersionCode() => int.Parse(PlayerSettings.iOS.buildNumber);
#else
    static int GetVersionCode() => -1;
#endif
#else
#if UNITY_ANDROID
    static AndroidJavaClass androidNativeClass;
    static int GetVersionCode()
    {
        if (androidNativeClass == null)
            androidNativeClass = new AndroidJavaClass("com.vgames.AndroidNative");
        return androidNativeClass.CallStatic<int>("GetVersionCode");
    }
#elif UNITY_IOS
    static int GetVersionCode()
    {
        return int.Parse(GetVersionCode_iOS());
    }
    [DllImport("__Internal")]
    private static extern string GetVersionCode_iOS();
#else
    static int GetVersionCode() => -1;
#endif
#endif
}