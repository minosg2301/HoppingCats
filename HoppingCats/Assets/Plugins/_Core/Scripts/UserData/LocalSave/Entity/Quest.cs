using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class Quest : DataObject<QuestDetail>
    {
        [SerializeField] internal Progress progress;
        [SerializeField] internal bool claimed = false;

        public bool CanClaim => Detail.useMultiActions ?
                                progress.value >= Detail.requires.count
                                : (Detail.statRequire.statId != -1 ? Detail.statRequire.Satify(progress.value) : progress.value >= Detail.require.count);
        public int GroupId => Detail.groupId;
        public Progress Progress => progress;

        public ActionRequire ActionRequire => Detail.require;
        //Multi Action + bool check
        public bool IsUseMultiRequires => Detail.useMultiActions;
        public ActionRequires ActionRequires => Detail.requires;

        public StatRequireDetail StatRequire => Detail.statRequire;
        public long RequireValue => Detail.useMultiActions ? Detail.requires.count : (Detail.statRequire.statId != -1 ? Detail.statRequire.value : Detail.require.count);

        public bool Claimed { get { return claimed; } set { claimed = value; Notify(); } }

        private QuestGroup _group;
        public QuestGroup QuestGroup { get { if (_group == null) _group = UserQuest.Ins.FindGroup(Detail.groupId); return _group; } }

        private RewardDetail _reward;
        public RewardDetail Reward { get { if (!_reward) _reward = QuestGroup?.GetReward(this); return _reward; } }

        public Quest(QuestDetail detail) : base(detail)
        {
            progress = new Progress();
        }

        protected override QuestDetail GetDetail() => QuestAsset.Ins.Find(DetailId);

        public bool ContainsAction(int actionId)
        {
            if (Detail.useMultiActions)
            {
                foreach (var action in Detail.requires.actions)
                {
                    if (action.id == actionId) return true;
                }
                return false;
            }
            return Detail.require.action.id == actionId;
        }
        public void Subscibe(IObserver observer) => UserQuest.Ins.Subscribe(observer, DetailId.ToString());
        public void Unsubcribe(IObserver observer) => UserQuest.Ins?.Unsubscribe(observer);
        public void Notify() => UserQuest.Ins.Notify(DetailId.ToString());

        public void AddProgress(long value)
        {
            progress.lastValue = progress.value;
            progress.value += value;
        }

        /// <summary>
        /// Chỉ cập nhật progress nếu value > current progress
        /// </summary>
        /// <param name="value"></param>
        public void UpdateProgress(long value)
        {
            if (value > progress.value)
            {
                progress.lastValue = progress.value;
                progress.value = value;
            }
        }
    }

    [Serializable]
    public struct Progress
    {
        public long value;
        public long lastValue;
    }
}