using System.Collections.Generic;

namespace moonNest
{
    /// <summary>
    /// Helper Class that keep track current Layer of all Layer group in runtime
    /// </summary>
    public class LayerHelper
    {
        /// <summary>
        /// Map keeps active layers
        /// </summary>
        public static Dictionary<int, LayerDetail> activeLayers = new Dictionary<int, LayerDetail>();

        /// <summary>
        /// Get active layer by user's stat
        /// </summary>
        /// <param name="statName"></param>
        /// <returns></returns>
        public static LayerDetail GetActiveLayer(int statId)
        {
            return activeLayers.TryGetValue(statId, out var layerDetail) ? layerDetail : null;
        }

        /// <summary>
        /// Update active layer by user's stat
        /// </summary>
        /// <param name="statName"></param>
        public static void UpdateActiveLayer(int statId)
        {
            int value = UserData.Stat(statId).AsInt;
            var layerGroup = LayerAsset.Ins.FindGroup(statId);
            var layerDetail = LayerAsset.Ins.FindLayerByRequireValue(layerGroup.id, value);
            activeLayers[statId] = layerDetail;
        }

        /// <summary>
        /// Get reward list by layer id
        /// </summary>
        /// <param name="layerRewards"></param>
        /// <param name="layerId"></param>
        /// <returns></returns>
        public static List<RewardDetail> GetRewards(List<LayerRewardList> layerRewards, int layerId)
        {
            var layerReward = layerRewards.Find(layer => layer.layerId == layerId);
            return layerReward?.rewards;
        }
    }
}