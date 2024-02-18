using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using moonNest;

public static class FBIGPostBuild
{
    private static UnityWebRequestAsyncOperation asyncOp;
    private static string zipFileName;

    //#if USE_FB_INSTANT_ADS
    //    [PostProcessBuildAttribute]
    //#endif
    public static void OnPostProcessBuildAsync(BuildTarget target, string pathToBuildProject)
    {
        var directoryInfo = Directory.CreateDirectory(pathToBuildProject);

        zipFileName = directoryInfo.Name;

        var zipFile = $"{pathToBuildProject}/../{zipFileName}.zip";
        if (File.Exists(zipFile))
            File.Delete(zipFile);

        //ZipFile.CreateFromDirectory(pathToBuildProject, zipFile);
        UploadBuild(zipFile);
    }

    private static void UploadBuild(string zipFile)
    {
        var appId = PlayerPrefs.GetString("fb_app_id", "");
        var accessToken = PlayerPrefs.GetString("fb_access_token", "");
        try
        {
            var wwform = WebRequestBuilder.BuilForm(
                "access_token", accessToken,
                "type", "BUNDLE",
                "asset", $"@{zipFile}",
                "comment", "Build ");

            var requestUrl = $"https://graph-video.facebook.com/{appId}/assets";
            using var webRequest = WebRequestBuilder.Post(requestUrl, wwform);
            asyncOp = webRequest.SendWebRequest();
            EditorApplication.update += OnUpdate;
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Upload build failed with Error\n{e.Message}", "Close");
        }
    }

    private static void OnUpdate()
    {
        if (EditorUtility.DisplayCancelableProgressBar("Upload Build", $"Uploading {zipFileName} ...", asyncOp.progress))
        {
            EditorApplication.update -= OnUpdate;
            EditorUtility.ClearProgressBar();
            return;
        }

        if (asyncOp.isDone)
        {
            EditorApplication.update -= OnUpdate;
            EditorUtility.ClearProgressBar();
        }
    }
}