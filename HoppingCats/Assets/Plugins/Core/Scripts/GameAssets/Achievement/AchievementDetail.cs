using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class AchievementDetail : BaseData
    {
        public int groupId;
        public int priority;
        public string description;
        public Sprite icon;
        public bool removeOnClaimed = true;
        public RewardDetail reward;
        public StatRequireDetail require = new StatRequireDetail();

        public AchievementDetail(string name, int groupId) : base(name)
        {
            this.groupId = groupId;
            reward = new RewardDetail("");
        }
    }
}