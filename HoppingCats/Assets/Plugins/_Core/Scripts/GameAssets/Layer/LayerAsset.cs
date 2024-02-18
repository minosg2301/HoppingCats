using System;
using System.Collections.Generic;

namespace moonNest
{
    public class LayerAsset : BaseGroupAsset<LayerAsset, LayerDetail, LayerGroup>
    {
        public List<LayerDetail> Layers => Datas;

        #region static methods
        #endregion


#if UNITY_EDITOR
        public Action<LayerGroup> onOverrideUpdated;
#endif

        #region group/layer methods
        protected override int GetGroupId(LayerDetail data) => data.groupId;

        public void CreateGroup(StatDefinition stat)
        {
            if(!stat)
                throw new NullReferenceException("Stat is null");

            AddGroup(new LayerGroup(stat));
        }

        public void DeleteGroup(StatDefinition stat)
        {
            if(!stat)
                throw new NullReferenceException("Stat is null");

            RemoveGroup(stat.id);
        }

        public LayerDetail FindLayer(int groupId, int layerId)
        {
            return FindByGroup(groupId).Find(layer => layer.id == layerId);
        }

        public LayerDetail FindLayerByRequireValue(int groupId, int requireValue)
        {
            return FindByGroup(groupId).FindLast(layer => layer.requireValue <= requireValue);
        }
        #endregion
    }
}