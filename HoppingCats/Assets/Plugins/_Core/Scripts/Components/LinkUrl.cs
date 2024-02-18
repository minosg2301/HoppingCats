using System;
using UnityEngine;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace moonNest
{
    [Serializable]
    public class LinkUrl
    {
        [SerializeField] private string url;

        public LinkUrl(string url)
        {
            this.url = url;
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        internal static extern void OpenUrlJS(string url);
#endif

        public void Open()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            OpenUrlJS(url);
#else
            Application.OpenURL(url);
#endif
        }
    }
}