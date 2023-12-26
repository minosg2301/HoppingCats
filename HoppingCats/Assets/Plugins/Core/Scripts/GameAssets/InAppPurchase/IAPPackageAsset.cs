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