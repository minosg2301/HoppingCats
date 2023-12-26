using System.Collections.Generic;

namespace moonNest
{
    public class ChestAsset : SingletonScriptObject<ChestAsset>
    {
        public bool layerEnabled;
        public int layerGroupId = -1;

        public List<ChestDetail> chests = new List<ChestDetail>();

        public ChestDetail Find(int id) => chests.Find(chest => chest.id == id);

        public ChestContent GetActualChestContent(ChestDetail chestDetail)
        {
            LayerDetail layer = GetActiveLayer();
            if(!layer) return chestDetail.content;
            ChestLayer chestLayer = GetChestLayer(layer, chestDetail);
            return chestLayer != null ? chestLayer.content : chestDetail.content;
        }

        #region layer methods
        public ChestLayer GetChestLayer(LayerDetail layer, ChestDetail chestDetail)
        {
            return layer && chestDetail.layerEnabled
                ? layer.chests.Find(chest => chest.chestId == chestDetail.id)
                : null;
        }

        public LayerDetail GetActiveLayer()
        {
            return layerEnabled && layerGroupId != -1 ? LayerHelper.GetActiveLayer(layerGroupId) : null;
        }
        #endregion
    }
}