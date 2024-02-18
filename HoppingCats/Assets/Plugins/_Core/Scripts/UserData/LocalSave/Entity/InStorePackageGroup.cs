using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class InStorePackageGroup : GroupObject<IAPPackageGroup>
    {
        [SerializeField] internal int layerId = -1;
        [SerializeField] internal SynchronizableTime refreshTime;

        public bool Refreshable => Detail.refreshConfig.enabled;

        /// <summary>
        /// Get last seconds to next refresh time
        /// </summary>
        public double LastSeconds => refreshTime.LocalTime.Subtract(DateTime.Now).TotalSeconds;

        /// <summary>
        /// keep variable to prevent serialized field
        /// </summary>
        [NonSerialized] private IAPPackageGroupLayer groupLayer;

        internal InStorePackageGroup(IAPPackageGroup detail) : base(detail)
        {
            refreshTime = new SynchronizableTime(Id);
        }

        protected override IAPPackageGroup GetDetail() => IAPPackageAsset.Ins.FindGroup(Id);

        public override string ToString() => Detail ? Detail.name : "";

        /// <summary>
        /// update layer if any
        /// </summary>
        public void UpdateLayer()
        {
            // update layer if any
            LayerDetail layer = IAPPackageAsset.Ins.GetActiveLayer();
            layerId = layer ? layer.id : -1;
            groupLayer = IAPPackageAsset.Ins.GetIAPGroupLayer(layerId, Id);
        }

        #region internal methods
        internal List<RewardDetail> GetRewards(IAPPackage iapPackage)
        {
            var rewards = iapPackage.rewards;
            if(layerId != -1)
            {
                if(groupLayer == null)
                    groupLayer = IAPPackageAsset.Ins.GetIAPGroupLayer(layerId, Id);

                if(groupLayer != null)
                {
                    var packageLayer = groupLayer.packages.Find(q => q.iapPackageId == iapPackage.id);
                    if(packageLayer != null) rewards = packageLayer.rewards;
                }
            }
            return rewards;
        }
        #endregion
    }
}