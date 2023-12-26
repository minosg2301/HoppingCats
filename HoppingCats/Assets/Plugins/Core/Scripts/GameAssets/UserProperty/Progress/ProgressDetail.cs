using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class ProgressDetail : BaseData
    {
        public string displayName;
        public Sprite icon;

        /// <summary>
        /// Group id which progress belong to
        /// </summary>
        public int groupId;

        /// <summary>
        /// Available if group type is passive
        /// Updated progress id meet require
        /// </summary>
        public int statValue;

        /// <summary>
        /// Stat value can be set lower value
        /// </summary>
        public bool downgradable = true;

        /// <summary>
        /// Require value to received reward
        /// And update stat value if group type is passive
        /// </summary>
        public int requireValue;

        /// <summary>
        /// Custom ui prefab for this progress
        /// </summary>
        public UIStatProgress customPrefab;

        /// <summary>
        /// Map id with unlock condition id
        /// </summary>
        public int unlockConditionId = -1;

        /// <summary>
        /// List unlock content ids whiches is unlocked by unlockConditionId
        /// </summary>
        public List<int> unlockContentIds = new List<int>();

        /// <summary>
        /// Rewards can be consumed when progress id meet require
        /// </summary>
        public RewardDetail reward;

        /// <summary>
        /// Premium rewards can be consumed when require value meets and group enable premium reward
        /// </summary>
        public RewardDetail premiumReward;

        /// <summary>
        /// Cached unlock contents
        /// </summary>
        [NonSerialized] private List<UnlockContentDetail> _unlockContents;
        public List<UnlockContentDetail> UnlockContents
        {
            get
            {
                if(_unlockContents == null)
                    _unlockContents = unlockContentIds.Map(id => UnlockContentAsset.Ins.FindContent(id));

                return _unlockContents;
            }

            set
            {
                _unlockContents = value;
            }
        }

        /// <summary>
        /// Get progress group of this
        /// </summary>
        [NonSerialized] private StatProgressGroupDetail _group;
        public StatProgressGroupDetail Group { get { if(!_group) _group = StatProgressAsset.Ins.FindGroup(groupId); return _group; } }

        /// <summary>
        /// Get next progress detail from this
        /// </summary>
        [NonSerialized] private ProgressDetail _next;
        [NonSerialized] private bool _isLast = false;
        public ProgressDetail Next { get { if(!_next && !_isLast) { _next = Group.progresses.Next(this); _isLast = !_next; } return _next; } }
        public float StepToNext => Next ? Next.requireValue - requireValue : 0;

        /// <summary>
        /// Cached unlock condition
        /// </summary>
        [NonSerialized] private UnlockConditionDetail _unlockCondition = null;
        public UnlockConditionDetail UnlockCondition
        {
            get
            {
                if(!_unlockCondition && unlockConditionId != -1)
                    _unlockCondition = UnlockContentAsset.Ins.FindCondition(unlockConditionId);

                return _unlockCondition;
            }

            set
            {
                _unlockCondition = value;
                unlockConditionId = _unlockCondition ? _unlockCondition.id : -1;
            }
        }

        public string UnlockConditionName => UnlockCondition ? UnlockCondition.name : "No Condition";

        public ProgressDetail(string name) : base(name)
        {
            reward = new RewardDetail("");
            premiumReward = new RewardDetail("");
        }

        public override string ToString() => "(" + statValue + "," + requireValue + ")";
    }
}