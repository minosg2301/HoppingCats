using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class BaseGatchaItemDetail : BaseData
    {
        public Sprite icon;
        public int weight = 1000;
        public float probability = 1;
        public RewardDetail reward;
        public bool isBigReward;

        public BaseGatchaItemDetail(string name) : base(name)
        {
            reward = new RewardDetail("");
        }
    }
}