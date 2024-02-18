using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class GameBuilder
{
    static readonly Dictionary<int, int> AddressableBuildIndices = new()
    {
        {1,3}, // Default build script
        {2,4} // Play Asset Delivery
    };

    public static void BuildAddressableContent()
    {
        var builder = int.Parse(GetCommandLineArg("-builder"));

        if (!AddressableBuildIndices.TryGetValue(builder, out var builderIndex))
        {
            // force use default build
            builderIndex = 3;
        }

        AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = builderIndex;
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);
        if (!success)
        {
            Debug.LogError("Addressables build error encountered: " + result.Error);
        }

        return;
    }

    public static void Build()
    {
        var version = GetCommandLineArg("-versionbuild");
        var versionCode = GetCommandLineArg("-versioncode");
        var platform = GetCommandLineArg("-buildTarget");
        var filename = GetCommandLineArg("-filename");
        var forceResolve = GetCommandLineArg("-forceresolve", false);

#if UNITY_ANDROID
        if (!string.IsNullOrEmpty(forceResolve) && forceResolve != "0")
        {
            GooglePlayServices.PlayServicesResolver.MenuForceResolve();
        }

        PlayerSettings.Android.keystoreName = GetCommandLineArg("-keystore");
        PlayerSettings.Android.keyaliasName = GetCommandLineArg("-keyalias");
        PlayerSettings.Android.keystorePass = GetCommandLineArg("-keypass");
        PlayerSettings.Android.keyaliasPass = PlayerSettings.Android.keystorePass;
        PlayerSettings.Android.bundleVersionCode = int.Parse(versionCode);
#endif

#if UNITY_IOS
        PlayerSettings.iOS.buildNumber = versionCode;
        PlayerSettings.iOS.appleDeveloperTeamID = GetCommandLineArg("-teamid");
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
#endif

        PlayerSettings.bundleVersion = version;

        BuildPlayerOptions buildPlayerOptions = new()
        {
            scenes = EditorBuildSettings.scenes.FindAll(s => s.enabled).Select(s => s.path).ToArray(),
            locationPathName = GetCommandLineArg("-outpath") + filename,
            target = platform == "iOS" ? BuildTarget.iOS : BuildTarget.Android,
            options = BuildOptions.None
        };

#if UNITY_ANDROID
        EditorUserBuildSettings.buildAppBundle = buildPlayerOptions.locationPathName.Contains(".aab");
        EditorUserBuildSettings.androidCreateSymbols = EditorUserBuildSettings.buildAppBundle
            ? AndroidCreateSymbols.Public
            : AndroidCreateSymbols.Disabled;
#endif

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    private static string GetCommandLineArg(string key, bool required = true)
    {
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (key == args[i])
            {
                return args[i + 1];
            }
        }

        if (required)
            throw new Exception($"Require argument \"{key}\"");
        else
            Debug.LogError($"Missing argument \"{key}\"");

        return "";
    }
}