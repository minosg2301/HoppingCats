#if UNITY_ANDROID
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using moonNest;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CorePostProcess : IPostprocessBuildWithReport
{
    public int callbackOrder => 50;

    public static bool AutoUploadSymbol { get; private set; }
    public static bool BuildAAB { get; private set; }
    public static string BuildFileName { get; private set; }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayerHandler);
    }

    private static void BuildPlayerHandler(BuildPlayerOptions options)
    {
        AutoUploadSymbol = false;
        BuildAAB = options.locationPathName.Contains(".aab");
        if (BuildAAB && !EditorConfigAsset.Ins.neverAskUploadSymbol)
        {
            var fileName = Path.GetFileName(options.locationPathName);
            BuildFileName = fileName[0..fileName.LastIndexOf(".aab")];

            int code = EditorUtility.DisplayDialogComplex("Debug Symbol", "Auto upload symbol after build?\nOnly YES when build for RELEASE new version", "Yes", "No", "No/Never Ask");
            switch (code)
            {
                case 0:
                    AutoUploadSymbol = true;
                    break;
                case 2:
                    {
                        EditorConfigAsset.Ins.neverAskUploadSymbol = true;
                        Draw.SetDirty(EditorConfigAsset.Ins);
                    }
                    break;
            }
        }

        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        string projPath = report.summary.outputPath;
        var outpath = Path.GetDirectoryName(report.summary.outputPath);
        if (BuildAAB && AutoUploadSymbol)
        {
            var uploadsymbolFile = $"{Application.dataPath}/../uploadsymbol.bat";
            if (!System.IO.File.Exists(uploadsymbolFile))
            {
                EditorUtility.DisplayDialog("Upload Symbol", "Can NOT upload symbol due to missing uploadsymbol.bat?", "Close");
                return;
            }

            var googleServiceJsonText = File.ReadAllText($"{Application.dataPath}/google-services.json");
            var googleServiceJson = JsonHelper.Deserialize(googleServiceJsonText);

            if ((googleServiceJson as Dictionary<string, object>).TryGetValue("client", out var clientobject))
            {
                var clients = clientobject as List<object>;
                if ((clients[0] as Dictionary<string, object>).TryGetValue("client_info", out var clientInfo))
                {
                    if ((clientInfo as Dictionary<string, object>).TryGetValue("mobilesdk_app_id", out var appId))
                    {
                        string version = Application.version;
                        int versionCode = PlayerSettings.Android.bundleVersionCode;
                        var symbolFile = $"{BuildFileName}-{version}-v{versionCode}.symbols.zip";
                        ExecuteCommand(outpath, uploadsymbolFile, $"{appId} {symbolFile}");
                    }
                    else
                    {
                        Debug.LogError("\"mobilesdk_app_id\" not exists in client.client_info of google-services.json");
                    }
                }
                else
                {
                    Debug.LogError("\"client_info\" not exists in client of google-services.json");
                }
            }
            else
            {
                Debug.LogError("\"client\" not exists in google-services.json");
            }
        }
    }

    static void ExecuteCommand(string workingDir, string command, string args = "")
    {
        Debug.Log($"Executing command: /c {command} {args}");

        var process = new Process();
        var info = new ProcessStartInfo("cmd.exe", $"/c {command} {args}");
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;
        info.RedirectStandardInput = true;
        info.UseShellExecute = false;
        info.WorkingDirectory = workingDir;
        process.StartInfo = info;
        process.Start();

        string output = string.Empty;
        using (StreamReader streamReader = process.StandardOutput)
            output = streamReader.ReadToEnd();
        if (!string.IsNullOrEmpty(output))
            Debug.Log("out:" + output);

        string error = string.Empty;
        using (StreamReader streamReader = process.StandardError)
            error = streamReader.ReadToEnd();
        if (!string.IsNullOrEmpty(error))
            Debug.LogError("err:" + error);


        process.WaitForExit();
        process.Close();
    }
}
#endif