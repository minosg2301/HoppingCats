using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace moonNest
{
    public class BaseScriptableObject : ScriptableObject
    {
#if UNITY_EDITOR
        public static T CreateAsset<T>(string title, string defaultName) where T : ScriptableObject
        {
            T asset = CreateInstance<T>();
            string path = EditorUtility.SaveFilePanelInProject(title, defaultName, "asset", "Select Path", "Assets/_Game/Resources/");
            if(path.Length > 0)
            {
                AssetDatabase.CreateAsset(asset, path);
                Selection.activeObject = asset;
            }
            return asset;
        }
#endif
    }
}