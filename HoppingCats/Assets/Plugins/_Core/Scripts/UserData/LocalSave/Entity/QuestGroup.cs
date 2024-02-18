using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class QuestGroup : GroupObject<QuestGroupDetail>
    {
        #region serialize field
        [SerializeField] internal int layerId = -1;
        [SerializeField] internal SynchronizableTime refreshTime;
        [SerializeField] internal int point = 0;
        [SerializeField] internal bool completed = false;
        [SerializeField] internal bool removed = false; // mark as removed to prevent creating new 
        [SerializeField] internal List<int> pointClaimeds = new List<int>();
        [SerializeField] internal List<int> removedQuests = new List<int>();
        #endregion

        #region properties
        public List<int> ClaimedKeys => pointClaimeds.ToList();
        public int Point => point;
        public bool Completed => completed;
        public bool Removed => removed;
        public bool RemoveOnCompleted => Detail.removeOnCompleted;

        public bool IsRefreshOnCompleted => Detail.refreshConfig.type == QuestRefeshType.OnCompleted;
        public bool IsRefreshOnTime => Detail.refreshConfig.type == QuestRefeshType.OnTime;

        /// <summary>
        /// Get last seconds to next refresh time
        /// </summary>
        public double LastSeconds => refreshTime.LocalTime.Subtract(DateTime.Now).TotalSeconds;

        /// <summary>
        /// keep variable to prevent serialized field
        /// </summary>
        [NonSerialized] private QuestGroupLayer groupLayer;
        #endregion

        public PointUpdateEvent onPointUpdated;

        public QuestGroup(QuestGroupDetail groupDetail) : base(groupDetail)
        {
            completed = false;
            refreshTime = new SynchronizableTime(Id);
            point = 0;
            pointClaimeds.Clear();
        }

        protected override QuestGroupDetail GetDetail() => QuestAsset.Ins.FindGroup(Id);

        #region public methods
        public override string ToString() => Detail.name;

        public void Notify() => UserQuest.Ins.Notify(Id.ToString());

        public bool CanClaim(int point) => !pointClaimeds.Contains(point);

        public void DoClaim(int point)
        {
            if(!pointClaimeds.Contains(point))
            {
                pointClaimeds.Add(point);
                UserQuest.Ins.dirty = true;
                UserQuest.Ins.Notify(Id.ToString());
                UserQuest.Ins.onGroupUpdated(this);
            }
        }
        #endregion

        #region internal methods
        internal RewardDetail GetReward(Quest quest)
        {
            var reward = quest.Detail.reward;
            if(layerId != -1)
            {
                if(groupLayer == null)
                    groupLayer = QuestAsset.Ins.GetQuestGroupLayer(layerId, Id);

                if(groupLayer != null)
                {
                    var questLayer = groupLayer.quests.Find(q => q.questId == quest.DetailId);
                    if(questLayer != null) reward = questLayer.reward;
                }
            }
            return reward;
        }

        internal void AddPoint(int value)
        {
            int lastPoint = point;
            point = Math.Max(0, point + value);
            onPointUpdated?.Invoke(lastPoint, point);
        }

        internal void Reset()
        {
            layerId = -1;
            groupLayer = null;
            completed = false;
            point = 0;
            pointClaimeds.Clear();
        }
        #endregion
    }
}