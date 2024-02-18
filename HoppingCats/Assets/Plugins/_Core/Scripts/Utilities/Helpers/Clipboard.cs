using System.Runtime.InteropServices;
using UnityEngine;

namespace moonNest
{
    public class Clipboard
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        internal static extern void CopyJS(string content);

        [DllImport("__Internal")]
        internal static extern string PasteJS();
#endif

        public static void Copy(string content)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            CopyJS(content);
#else
            GUIUtility.systemCopyBuffer = content;
#endif
        }

        public static string Paste()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return PasteJS();
#else
            return GUIUtility.systemCopyBuffer;
#endif
        }
    }
}