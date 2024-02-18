#if UNITY_ANDROID
using System.IO;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

/*
 * This class process when NOT define symbol "USE_GOOGLE_MOBILE_ADS"
 * _ Generate folder VGames.androidlib in Plugins/Android
 * _ Add uses-permission: com.google.android.gms.permission.AD_ID
 */

#if !USE_GOOGLE_MOBILE_ADS
public class VGamesLibPreProcessor_Android
{
    private const string PluginAndroid = "Plugins/Android";
    private const string VGamesManifestPath = PluginAndroid + "/VGames.androidlib/AndroidManifest.xml";
    private const string GoogleMobileAdsManifestPath = PluginAndroid + "/GoogleMobileAdsPlugin.androidlib/AndroidManifest.xml";

    private const string AD_ID_PERMISSION_ATTR = "com.google.android.gms.permission.AD_ID";
    private const string USES_PERMISSION = "uses-permission";
    private static XNamespace ns = "http://schemas.android.com/apk/res/android";

    public int callbackOrder => 1;

    [InitializeOnLoadMethod]
    static void OnInit()
    {
        // Delete GoogleMobileAdsPlugin.androidlib
        var googleMobileAdsManifestFullPath = Path.Combine(Application.dataPath, GoogleMobileAdsManifestPath);
        var googleMobileAdsLibDirectory = Path.GetDirectoryName(googleMobileAdsManifestFullPath);
        if (Directory.Exists(googleMobileAdsLibDirectory))
        {
            var di = new DirectoryInfo(googleMobileAdsLibDirectory);
            di.GetFiles().ForEach(f => f.Delete());
            Directory.Delete(googleMobileAdsLibDirectory);
            File.Delete(Path.Combine(Application.dataPath, PluginAndroid, "GoogleMobileAdsPlugin.androidlib.meta"));
        }

        // Create VGames.androidlib
        var manifestFilePath = Path.Combine(Application.dataPath, VGamesManifestPath);
        var vGamesLibDirectory = Path.GetDirectoryName(manifestFilePath);
        if (!Directory.Exists(vGamesLibDirectory))
        {
            Directory.CreateDirectory(vGamesLibDirectory);
        }

        //if (File.Exists(manifestFilePath)) return;

        CreateManifest(manifestFilePath);
        CreatePropertiesFile(vGamesLibDirectory);
    }

    void StopBuildWithMessage(string message)
    {
        string prefix = "[VGamesLibPostProcessor] ";
        EditorUtility.DisplayDialog("VGamesLibPostProcessor", "Error: " + message, "", "");
        throw new System.OperationCanceledException(prefix + message);
    }

    private static XElement CreatePermissionElement(string name)
    {
        return new XElement(USES_PERMISSION, new XAttribute(ns + "name", name));
    }

    private static XElement CreateMetadataElement(string name, string value)
    {
        return new XElement("meta-data",
            new XAttribute(ns + "name", name),
            new XAttribute(ns + "value", value));
    }

    private static void CreatePropertiesFile(string directory)
    {
        using var wrp = new StreamWriter(directory + "/project.properties", false);
        wrp.WriteLine("target=android-19");
        wrp.WriteLine("android.library=true");
    }

    private static XDocument CreateManifest(string manifestPath)
    {
        var manifest = new XDocument();
        manifest.Add(new XElement("manifest"));

        var elemManifest = manifest.Element("manifest");
        elemManifest.SetAttributeValue(XNamespace.Xmlns.GetName("android"), ns);
        elemManifest.SetAttributeValue("package", "com.vgames.lib");
        elemManifest.SetAttributeValue(ns + "versionCode", "1");
        elemManifest.SetAttributeValue(ns + "versionName", "1.0");
        elemManifest.Add(CreatePermissionElement(AD_ID_PERMISSION_ATTR));
        elemManifest.Add(new XElement("application"));
        
        var applicationManifest = elemManifest.Element("application");
        applicationManifest.Add(CreateMetadataElement("com.google.android.gms.ads.flag.OPTIMIZE_AD_LOADING", "true"));
        applicationManifest.Add(CreateMetadataElement("com.google.android.gms.ads.flag.OPTIMIZE_INITIALIZATION", "true"));

        manifest.Save(manifestPath);
        return manifest;
    }
}
#endif // USE_GOOGLE_MOBILE_ADS
#endif // UNITY_ANDROID