using System.Collections.Generic;

namespace moonNest
{
    public class IAPPackageAsset : BaseGroupAsset<IAPPackageAsset, IAPPackage, IAPPackageGroup>
    {
        public bool layerEnabled;
        public int layerGroupId = -1;

        public List<IAPTrigger> triggers = new List<IAPTrigger>();

        public List<IAPPackage> Packages => Datas;

        protected override int GetGroupId(IAPPackage data) => data.groupId;

        public IAPPackage Find(string productId) => Datas.Find(data => data.productId == productId || data.promotionProductId == productId);

        #region layer methods
        public IAPPackageGroupLayer GetIAPGroupLayer(int layerId, int groupId)
        {
            var layer = GetLayerById(layerId);
            if(!layer || !layerEnabled) return null;

            var group = layer.iapPackageGroups.Find(l => l.groupId == groupId);
            if(group.iapLinkedLayer != -1)
            {
                var linkedLayer = GetLayerById(group.iapLinkedLayer);
                group = linkedLayer.iapPackageGroups.Find(l => l.groupId == groupId);
            }
            return group;
        }

        public LayerDetail GetActiveLayer()
        {
            return layerEnabled ? LayerHelper.GetActiveLayer(layerGroupId) : null;
        }

        public LayerDetail GetLayerById(int layerId)
        {
            if(!layerEnabled || layerGroupId == -1) return null;
            return LayerAsset.Ins.FindLayer(layerGroupId, layerId);
        }
        #endregion
    }
}