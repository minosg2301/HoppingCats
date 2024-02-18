using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class FeatureConfig : BaseData
    {
        public string displayName;
        public Sprite icon;
        public GameObject uiPrefab;
        public bool locked = false;
        public int unlockConditionId = -1;
        public string description = "";
        public bool canEditName = true;

        public FeatureConfig(string name) : base(name)
        {
        }

        public static FeatureConfig OnlineReward = new FeatureConfig("OnlineReward") { canEditName = false };
        public static FeatureConfig Achievement = new FeatureConfig("Achievement") { canEditName = false };
        public static FeatureConfig Arena = new FeatureConfig("Arena") { canEditName = false };
        public static FeatureConfig Gatcha = new FeatureConfig("Gatcha") { canEditName = false };

        public static implicit operator bool(FeatureConfig exists) => exists != null;
    }
}