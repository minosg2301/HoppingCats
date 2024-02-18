#if UNITY_IOS
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

public class VGamesLibPostProcessor : IPostprocessBuildWithReport
{
    const string kPostBuildLibPath = "Libraries/Plugins/ThirdParties/VGames/ios/nativetemplates";
    const string kSmallTemplateViewXIBPath = kPostBuildLibPath + "/GADTSmallTemplateView.xib";
    const string kMediumTemplateViewXIBPath = kPostBuildLibPath + "/GADTMediumTemplateView.xib";
    const string kFullscreenTemplateViewXIBPath = kPostBuildLibPath + "/GADTFullScreenTemplateView.xib";

    private static string m_buildPath;
    private static string m_mainTargetGuid;
    private static string m_resourcesBuildPhase;
    private static PBXProject m_pBXProject;

    public int callbackOrder => 100;

    public void OnPostprocessBuild(BuildReport report)
    {
        string projPath = PBXProject.GetPBXProjectPath(report.summary.outputPath);

        m_buildPath = report.summary.outputPath;
        m_pBXProject = new PBXProject();
        m_pBXProject.ReadFromFile(projPath);
        m_mainTargetGuid = m_pBXProject.GetUnityMainTargetGuid();
        m_resourcesBuildPhase = m_pBXProject.GetResourcesBuildPhaseByTarget(m_mainTargetGuid);

        AddFileToBuildPhase(kSmallTemplateViewXIBPath);
        AddFileToBuildPhase(kMediumTemplateViewXIBPath);
        //AddFileToBuildPhase(kFullscreenTemplateViewXIBPath);

        m_pBXProject.WriteToFile(projPath);
    }

    private static void AddFileToBuildPhase(string file)
    {
        string filePath = Path.Combine(m_buildPath, file);
        string fileGuid = m_pBXProject.FindFileGuidByRealPath(filePath);
        m_pBXProject.RemoveFile(fileGuid);
        string resourcesFilesGuid = m_pBXProject.AddFile(filePath, filePath, PBXSourceTree.Source);
        m_pBXProject.AddFileToBuildSection(m_mainTargetGuid, m_resourcesBuildPhase, resourcesFilesGuid);
        
    }
}
#endif