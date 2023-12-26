using UnityEngine.AddressableAssets;
using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class FlexibleAsset<T> where T : Object
    {
        public bool useReference;
        public T directAsset;
        public AssetReference assetReference = new AssetReference();

        [NonSerialized] T _origin;
        [NonSerialized] bool loading;

        void UpdateOrigin()
        {
            if (_origin) return;
            if (!useReference) _origin = directAsset;
        }

        public async Task LoadAssetAsync()
        {
            if (_origin) return;

            if (!loading)
            {
                loading = true;
                _origin = await assetReference.LoadAssetAsync<T>().Task;
                loading = false;
                if (_origin is GameObject)
                    PoolSystem.SetObject(assetReference.RuntimeKey.ToString(), _origin as GameObject);
            }
            else
            {
                await Task.Factory.StartNew(() => { while (loading) { } });
            }
        }

        public static implicit operator T(FlexibleAsset<T> flexAsset)
        {
            flexAsset.UpdateOrigin();
            return flexAsset._origin;
        }

#if UNITY_EDITOR
        [NonSerialized] public AssetReferenceDetailDrawer detailDrawer;

        public void DrawAssetReference(float maxWidth = -1)
        {
            if (detailDrawer == null) detailDrawer = new AssetReferenceDetailDrawer(assetReference);

            detailDrawer.Draw(maxWidth);
        }
#endif
    }
}