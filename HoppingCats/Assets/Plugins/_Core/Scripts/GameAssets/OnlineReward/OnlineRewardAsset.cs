using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class OnlineRewardAsset : SingletonScriptObject<OnlineRewardAsset>
    {
        public bool layerEnabled;
        public int layerGroupId = -1;

#if UNITY_EDITOR
        private LayerGroup layerGroup;
        public string LayerName
        {
            get
            {
                if(layerEnabled && layerGroupId != -1)
                {
                    if(!layerGroup || layerGroup.id != layerGroupId)
                        layerGroup = LayerAsset.Ins.FindGroup(layerGroupId);
                    return layerGroup.name;
                }
                return "No Layer";
            }
        }
#endif

        public List<OnlineRewardDetail> onlineRewards = new List<OnlineRewardDetail>();

        public OnlineRewardDetail Find(int onlineRewardId) => onlineRewards.Find(o => o.id == onlineRewardId);

        /// <summary>
        /// Find reward detail with layer id
        /// </summary>
        /// <param name="onlineRewardId"></param>
        /// <param name="rewardId"></param>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public RewardDetail FindReward(int onlineRewardId, int rewardId)
        {
            OnlineRewardDetail onlineReward = onlineRewards.Find(o => o.id == onlineRewardId);
            return onlineReward?.rewards.Find(r => r.id == rewardId);
        }

        public RewardDetail FindReward(LayerDetail layer, int onlineRewardId, int rewardId)
        {
            if(layer == null) throw new NullReferenceException("Layer is null");

            OnlineRewardLayer onlineRewardLayer = layer?.onlineRewards.Find(o => o.onlineRewardId == onlineRewardId);
            return onlineRewardLayer?.rewards.Find(r => r.id == rewardId);
        }

        /// <summary>
        /// Get active layer id if any
        /// </summary>
        /// <returns></returns>
        public LayerDetail GetActiveLayer()
        {
            return layerEnabled ? LayerHelper.GetActiveLayer(layerGroupId) : null;
        }

        public LayerDetail GetLayerById(int layerId)
        {
            if(!layerEnabled || layerGroupId == -1) return null;
            return LayerAsset.Ins.FindLayer(layerGroupId, layerId);
        }
    }
}