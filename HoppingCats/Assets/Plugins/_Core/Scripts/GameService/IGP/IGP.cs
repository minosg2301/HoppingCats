using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public static class IGP
    {
        public static IEnumerable<IGPGameInfo> GameSelectionInfos { get; private set; }

        public static bool HaveGameSelection => GameSelectionInfos != null && GameSelectionInfos.Count() > 1;

        public static bool Fetching { get; internal set; }

        public static event Action OnFetchDone = delegate { };

        static readonly Dictionary<string, Texture2D> imageCacheds = new();
        static readonly Dictionary<string, Texture2D> iconCacheds = new();

        public static void Init()
        {
            Fetching = true;
        }

        public static void OpenStore(IGPGameInfo gameInfo)
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(gameInfo.Identifier))
            {
                Application.OpenURL(string.Format(ApplicationExt.appStoreUrl, gameInfo.Identifier));
            }
#elif UNITY_IOS
            if (!string.IsNullOrEmpty(gameInfo.IosAppId))
            {
                Application.OpenURL(string.Format(ApplicationExt.appStoreUrl, gameInfo.IosAppId));
            }
#endif
        }

        static UniTask<T> Get<T>(string request)
        {
            return GameServiceAPI.Get<T>("/igp/" + request);
        }

        static async void Init_Internal()
        {
            await GetGameSelectionAsync();
            PrecacheResources();
            Fetching = false;
            OnFetchDone();
        }

        static async UniTask GetGameSelectionAsync()
        {
            try
            {
                var ret = await Get<IGPGameInfos>("game-selections");
                GameSelectionInfos = ret.data;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        static async void PrecacheResources()
        {
            if (GameSelectionInfos != null)
            {
                foreach (var gameInfo in GameSelectionInfos)
                {
                    if (IGPResourceManager.IsAvailableCached(gameInfo, "img"))
                        continue;

                    //Debug.Log($"PrecacheResources: {gameInfo.Id}");
                    await UniTask.WhenAll(
                        IGPResourceManager.LoadTextureAsync(gameInfo, "img"),
                        IGPResourceManager.LoadTextureAsync(gameInfo, "ic"));
                    //Debug.Log($"PrecacheResources: {gameInfo.Id} - Done");
                }
            }
        }

        static void OnFirebaseInitialized()
        {
            Init_Internal();
        }

        internal static async UniTask<Texture2D> GetGameImageAsync(IGPGameInfo gameInfo)
        {
            if (imageCacheds.TryGetValue(gameInfo.Id, out var texture))
            {
                return texture;
            }
            //Debug.Log($"GetGameImageAsync: {gameInfo.Id}");
            texture = await IGPResourceManager.LoadTextureAsync(gameInfo, "img");
            if (texture != null) imageCacheds[gameInfo.Id] = texture;
            return texture;
        }

        internal static async UniTask<Texture> GetGameIconAsync(IGPGameInfo gameInfo)
        {
            if (iconCacheds.TryGetValue(gameInfo.Id, out var texture))
            {
                return texture;
            }
            //Debug.Log($"GetGameIconAsync: {gameInfo.Id}");
            texture = await IGPResourceManager.LoadTextureAsync(gameInfo, "ic");
            if (texture != null) imageCacheds[gameInfo.Id] = texture;
            return texture;
        }
    }

    [Serializable]
    class TextureData
    {
        public string base64;
        public DateTime expiredTime;
        public int width;
        public int height;
        public TextureFormat format;
    }
}