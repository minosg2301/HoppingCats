using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Networking;

namespace moonNest
{
    public static class Texture2DExt
    {
        public static Texture2D ToTexture(this RenderTexture rt) => Apply(rt, new Texture2D(rt.width, rt.height));

        public static Texture2D ToTexture(this RenderTexture rt, GraphicsFormat format, TextureCreationFlags flags)
            => Apply(rt, new Texture2D(rt.width, rt.height, format, flags));

        private static Texture2D Apply(RenderTexture rt, Texture2D tex)
        {
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();
            RenderTexture.active = currentActiveRT;
            return tex;
        }

        public static void WriteToFile(this Texture2D texture, string filename)
        {
            if (filename.IndexOf("/") != -1)
            {
                string directory = Application.persistentDataPath + filename.Substring(0, filename.LastIndexOf("/"));
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(Application.persistentDataPath + filename, texture.EncodeToPNG());
        }

        public static void SaveToAsset(this Texture2D texture, string filename)
        {
            if (filename.IndexOf("/") != -1)
            {
                string directory = Application.dataPath + filename.Substring(0, filename.LastIndexOf("/"));
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            }
            File.WriteAllBytes(Application.dataPath + filename, texture.EncodeToPNG());
        }

        public static Texture2D CreateFromFile(string filename)
        {
            byte[] bytes = File.ReadAllBytes(Application.persistentDataPath + filename);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            return texture;
        }

        public static async Task<Texture2D> CreateFromURL(string url)
        {
            try
            {
                UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
                await webRequest.SendWebRequest();
                if (webRequest.result == UnityWebRequest.Result.ProtocolError
                || webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    return null;
                }
                else
                {
                    return DownloadHandlerTexture.GetContent(webRequest);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}