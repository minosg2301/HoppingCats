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
        }

        #region internal methods
        internal List<RewardDetail> GetRewards(IAPPackage iapPackage)
        {
            var rewards = iapPackage.rewards;
            return rewards;
        }
        #endregion
    }
}