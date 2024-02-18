#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

public class GradleMod : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get => 999; }

    public void OnPostGenerateGradleAndroidProject(string path)
    {

        Debug.Log($"[BuildPreProcess] OnPostGenerateGradleAndroidProject. Path: {path}");
        //EditorUtility.RevealInFinder(path);
        string gradlePropertiesFile = path + "/../launcher/gradle.properties";
        var gradleContentsSb = new StringBuilder();
        if (File.Exists(gradlePropertiesFile))
        {
            string gradlePropertiesContent = File.ReadAllText(gradlePropertiesFile);
            gradleContentsSb.Append(gradlePropertiesContent);
            File.Delete(gradlePropertiesFile);
        }
        StreamWriter writer = File.CreateText(gradlePropertiesFile);
        gradleContentsSb.AppendLine("android.useAndroidX=true");
        gradleContentsSb.AppendLine("android.enableJetifier=true");
        //gradleContentsSb.AppendLine("android.enableR8=false");
        //gradleContentsSb.AppendLine("android.enableR8.libraries = false");

        writer.Write(gradleContentsSb.ToString());
        writer.Flush();
        writer.Close();
        Debug.LogFormat("[BuildPreProcess] OnPostGenerateGradleAndroidProject. Final gradle.properties content:\n{0}", gradleContentsSb);
    }
}
#endif