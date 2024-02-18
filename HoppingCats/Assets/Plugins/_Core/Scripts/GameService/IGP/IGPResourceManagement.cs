using BayatGames.SaveGameFree;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    internal static class IGPResourceManager
    {
        static readonly Dictionary<string, List<UniTaskCompletionSource<Texture2D>>> textureCachings = new();

        internal static bool IsAvailableCached(IGPGameInfo gameInfo, string type)
        {
            var key = $"igp_{type}_{gameInfo.Id}";
            if (SaveGame.Exists(key))
            {
                var textureData = SaveGame.Load<TextureData>(key);
                if (DateTime.Compare(textureData.expiredTime, DateTime.Now) > 0)
                {
                    return true;
                }

                SaveGame.Delete(key);
            }
            return false;
        }

        internal static UniTask<Texture2D> LoadTextureAsync(IGPGameInfo gameInfo, string type)
        {
            string key = $"{type}_{gameInfo.Id}";
            if (textureCachings.TryGetValue(key, out var list))
            {
                var tcs = new UniTaskCompletionSource<Texture2D>();
                list.Add(tcs);
                return tcs.Task;
            }

            return LoadTextureFromCached(key, gameInfo.ImageUrl);
        }

        static async UniTask<Texture2D> LoadTextureFromCached(string key, string imageUrl)
        {
            if (TryReadFromFile(key, out var texture))
            {
                return texture;
            }

            bool valid = Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!valid) return null;

            textureCachings[key] = new List<UniTaskCompletionSource<Texture2D>>();

            texture = await Texture2DExt.CreateFromURL(imageUrl);
            if (texture)
            {
                SaveToFile(key, texture);
            }

            foreach (var tcs in textureCachings[key])
            {
                tcs.TrySetResult(texture);
            }

            textureCachings.Remove(key);
            return texture;
        }

        static void SaveToFile(string key, Texture2D texture)
        {
            //Debug.Log($"IGP:SaveToFile {key}");

            var savedKey = $"igp_{key}";
            var data = texture.GetRawTextureData();
            var base64 = Convert.ToBase64String(data);
            var textureData = new TextureData()
            {
                base64 = base64,
                expiredTime = DateTime.Now.AddDays(7),
                width = texture.width,
                height = texture.height,
                format = texture.format
            };
            SaveGame.Save(savedKey, textureData);
        }

        static bool TryReadFromFile(string key, out Texture2D texture)
        {
            var savedKey = $"igp_{key}";
            if (SaveGame.Exists(savedKey))
            {
                var textureData = SaveGame.Load<TextureData>(savedKey);
                var data = Convert.FromBase64String(textureData.base64);
                texture = new Texture2D(textureData.width, textureData.height, textureData.format, false);
                texture.LoadRawTextureData(data);
                texture.Apply();
                return true;
            }
            texture = null;
            return false;
        }
    }
}