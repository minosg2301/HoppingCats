#if UNITY_ANDROID
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/*
 * This class copy or create AndroidManifest.xml file to Plugins/Android as .androidlib
 * These lib content com.google.android.gms.ads.APPLICATION_ID for google admob
 * 
 * When define symbol "USE_GOOGLE_MOBILE_ADS"
 * _ Copy template GoogleMobileAdsPlugin.androidlib to Plugins/Android folder
 * _ ThirdParties/GoogleMobileAds will update com.google.android.gms.ads.APPLICATION_ID in PostProcessBuild of its lib
 * 
 * NOT define symbol "USE_GOOGLE_MOBILE_ADS"
 * _ Generate folder GoogleAdmobs.androidlib in Plugins/Android
 * _ Copy "_Core/Editor/PostProcess/template-AndroidManifest.txt" to Plugins/Android/GoogleAdmobs.androidlib/AndroidManifest.xml
 * _ Update com.google.android.gms.ads.APPLICATION_ID into the Manifest.xml
 *   According to use "USE_IRONSRC_ADS" or MaxSdk
 */

#if USE_GOOGLE_MOBILE_ADS

[InitializeOnLoad]
public class GoogleMobileAdsProcessor
{
    private const string PluginAndroid = "Plugins/Android";
    private const string VGamesManifestPath = PluginAndroid + "/VGames.androidlib/AndroidManifest.xml";
    private const string GoogleMobileAdsManifestPath = PluginAndroid + "/GoogleMobileAdsPlugin.androidlib/AndroidManifest.xml";

    private const string GoogleMobileAdsManifestTemplateDirectory = "Plugins/_Core/Editor/PostProcess/Android/GoogleMobileAdsPlugin.androidlib";

    static GoogleMobileAdsProcessor()
    {
        var path = Path.Combine(Application.dataPath, VGamesManifestPath);
        var directory = Path.GetDirectoryName(path);
        if(Directory.Exists(directory))
        {
            var di = new DirectoryInfo(directory);
            di.GetFiles().ForEach(f => f.Delete());
            Directory.Delete(directory);
            File.Delete(Path.Combine(Application.dataPath, PluginAndroid, "VGames.androidlib.meta"));
        }

        path = Path.Combine(Application.dataPath, GoogleMobileAdsManifestPath);
        if(!File.Exists(path))
        {
            directory = Path.GetDirectoryName(path);
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var templateDirectory = Path.Combine(Application.dataPath, GoogleMobileAdsManifestTemplateDirectory);
            foreach(var file in Directory.GetFiles(templateDirectory))
                File.Copy(file, Path.Combine(directory, Path.GetFileName(file)));
        }
    }
}

#endif // USE_GOOGLE_MOBILE_ADS

#endif