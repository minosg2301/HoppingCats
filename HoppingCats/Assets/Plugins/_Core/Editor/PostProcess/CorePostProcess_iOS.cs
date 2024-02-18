#if UNITY_IOS
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;
using vgame;

public class CorePostProcess_iOS : IPostprocessBuildWithReport
{
    public int callbackOrder => 49; // //must be between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and that it's added before "pod install" (50)

    public void OnPostprocessBuild(BuildReport report)
    {
        string pbxProjectPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);
        UpdatePBXProject(pbxProjectPath);
        UpdatePodfile(report.summary.outputPath);
    }

    static void UpdatePBXProject(string pbxProjectPath)
    {
        PBXProject pbxProject = new();
        pbxProject.ReadFromString(File.ReadAllText(pbxProjectPath));

        string mainTarget = pbxProject.GetUnityMainTargetGuid();
        string unityFrameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();

        var entitlementsFileName = pbxProject.GetBuildPropertyForAnyConfig(mainTarget, "CODE_SIGN_ENTITLEMENTS");
        if (entitlementsFileName == null)
        {
            var bundleIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            entitlementsFileName = string.Format("{0}.entitlements", bundleIdentifier.Substring(bundleIdentifier.LastIndexOf(".") + 1));
        }

        foreach (var target in new[] { mainTarget, unityFrameworkTarget })
        {
            pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        }

        pbxProject.SetBuildProperty(mainTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        pbxProject.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO");

        pbxProject.SetBuildProperty(mainTarget, "CURRENT_PROJECT_VERSION", PlayerSettings.iOS.buildNumber);
        pbxProject.SetBuildProperty(mainTarget, "MARKETING_VERSION", PlayerSettings.bundleVersion);
        File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());

        ProjectCapabilityManager manager = new(pbxProjectPath, entitlementsFileName, "Unity-iPhone");
        if (GlobalConfig.Ins.useGameCenter) manager.AddGameCenterCustom();
        if (GlobalConfig.Ins.useSignInWithApple) manager.AddSignInWithApple();
        manager.WriteToFile();
    }

    static void UpdatePodfile(string pathToBuiltProject)
    {
        var destPodfilePath = Path.Combine(pathToBuiltProject, "Podfile");
        var contentToAppend = @"
        post_install do |installer|
          installer.pods_project.targets.each do |target|
            if target.respond_to?(:product_type) and target.product_type == ""com.apple.product-type.bundle""
              target.build_configurations.each do |config|
                  config.build_settings['CODE_SIGNING_ALLOWED'] = 'NO'
              end
            end
          end
        end";

        if (File.Exists(destPodfilePath))
        {
            var content = File.ReadAllText(destPodfilePath);
            if (!content.Contains(contentToAppend))
            {
                content = content + "\n" + contentToAppend;
                File.WriteAllText(destPodfilePath, content);
            }
        }
    }
}

static class ProjectCapabilityManagerExt
{
    const BindingFlags NonPublicInstanceBinding = BindingFlags.NonPublic | BindingFlags.Instance;
    const string GameCenterKey = "com.apple.developer.game-center";

    public static void AddGameCenterCustom(this ProjectCapabilityManager manager)
    {
        var managerType = typeof(ProjectCapabilityManager);
        var projectField = managerType.GetField("project", NonPublicInstanceBinding);
        var targetGuidField = managerType.GetField("m_TargetGuid", NonPublicInstanceBinding);
        var entitlementFilePathField = managerType.GetField("m_EntitlementFilePath", NonPublicInstanceBinding);
        var getOrCreateEntitlementDocMethod = managerType.GetMethod("GetOrCreateEntitlementDoc", NonPublicInstanceBinding);

        if (projectField == null || targetGuidField == null
            || entitlementFilePathField == null || getOrCreateEntitlementDocMethod == null)
            throw new Exception("Can't Add Sign In With Apple programatically in this Unity version");

        if (getOrCreateEntitlementDocMethod.Invoke(manager, new object[] { }) is PlistDocument entitlementDoc)
        {
            entitlementDoc.root[GameCenterKey] = new PlistElementBoolean(true);
        }

        if (projectField.GetValue(manager) is PBXProject project)
        {
            var mainTargetGuid = targetGuidField.GetValue(manager) as string;
            project.AddCapability(mainTargetGuid, PBXCapabilityType.GameCenter);
        }
    }
}
#endif