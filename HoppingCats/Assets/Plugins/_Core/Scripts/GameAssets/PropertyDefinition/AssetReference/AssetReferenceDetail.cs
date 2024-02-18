using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace moonNest
{
    [Serializable]
    public class AssetReferenceDetail
    {
        public int id;
        public string name;
        public AssetType type;
        public AssetReference assetReference;

        [NonSerialized] Object _origin = null;
        [NonSerialized] bool loading;
        [NonSerialized] List<TaskCompletionSource<bool>> taskCompletionSources;

        public T AssetAs<T>() where T : Object => _origin as T;
        public Object Origin => _origin;

        public AssetReferenceDetail()
        {
            taskCompletionSources = new List<TaskCompletionSource<bool>>();
        }

        public async Task LoadAssetAsync()
        {
            if (_origin) return;

            if (!loading)
            {
                loading = true;
                _origin = await assetReference.LoadAssetAsync<Object>().Task;
                loading = false;
                foreach (var tcs in taskCompletionSources) tcs.SetResult(true);
                taskCompletionSources.Clear();
                if (_origin is GameObject)
                    PoolSystem.SetObject(assetReference.RuntimeKey.ToString(), _origin as GameObject);
            }
            else
            {
                var tcs = new TaskCompletionSource<bool>();
                taskCompletionSources.Add(tcs);
                await tcs.Task;
            }
        }

        public Task<T> GetOrLoadAsync<T>()
        {
            return assetReference.GetOrLoadAsync<T>();
        }

#if UNITY_EDITOR
        [NonSerialized] public AssetReferenceDetailDrawer detailDrawer;

        public void Draw(float maxWidth = -1)
        {
            if (detailDrawer == null) detailDrawer = new AssetReferenceDetailDrawer(assetReference);

            detailDrawer.Draw(maxWidth);
        }

        public void DrawSubAsset(bool autoSize, float maxWidth = -1, float maxHeight = -1)
        {
            if (detailDrawer == null) detailDrawer = new AssetReferenceDetailDrawer(assetReference);

            detailDrawer.DrawSubSprite(autoSize, maxWidth, maxHeight);
        }
#endif

        public AssetReferenceDetail(AssetReferenceDefinition definition)
        {
            id = definition.id;
            name = definition.name;
            type = definition.type;
            assetReference = new AssetReference();
        }
    }
}