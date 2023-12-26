using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class AchievementGroupDetail : BaseData
    {
        public int priority;
        public string description;
        public Sprite icon;
        public bool removeOnClaimed = true;
        public RewardDetail reward;

        public AchievementGroupDetail(string name) : base(name) { }
    }
}