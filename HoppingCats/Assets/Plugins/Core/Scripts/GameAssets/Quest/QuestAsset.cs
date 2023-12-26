using System.Collections.Generic;

namespace moonNest
{
    public class QuestAsset : BaseGroupAsset<QuestAsset, QuestDetail, QuestGroupDetail>
    {
        public int layerGroupId = -1;
        public bool layerEnabled;

        public List<QuestDetail> Quests => Datas;

        protected override int GetGroupId(QuestDetail data) => data.groupId;

        #region layer methods
        public QuestGroupLayer GetQuestGroupLayer(int layerId, int groupId)
        {
            var layer = GetLayerById(layerId);
            if(!layer || !layerEnabled) return null;

            //var actualLayer = layer.arenaLinkedLayer == -1 ? layer : GetLayerById(layer.arenaLinkedLayer);
            //if(!actualLayer) return null;

            return layer.questGroups.Find(l => l.groupId == groupId);
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