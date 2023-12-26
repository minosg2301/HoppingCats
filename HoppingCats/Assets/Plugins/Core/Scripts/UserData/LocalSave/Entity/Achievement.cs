using System;
using UnityEngine;

namespace moonNest
{
    public class Achievement : DataObject<AchievementDetail>
    {
        private static bool Satify(StatRequireDetail require) => UserData.Stat(require.statId) >= require.value;

        [SerializeField] private bool active;
        [SerializeField] private bool hide;
        [SerializeField] private bool claimed;

        public bool CanClaim => Satify(Require);
        public bool Hide => hide;
        public int GroupId => Detail.groupId;
        public int Progress => UserData.Stat(Require.statId);
        public StatRequireDetail Require => Detail.require;

        public bool Active { get { return active; } internal set { active = value; Notify(); } }
        public bool Claimed { get { return claimed; } set { claimed = value; Notify(); } }

        public Achievement(AchievementDetail detail) : base(detail)
        {
            active = true;
            claimed = false;
        }

        protected override AchievementDetail GetDetail() => AchievementAsset.Ins.Find(DetailId);

        public void Subscibe(IObserver observer) => UserQuest.Ins.Subscribe(observer, DetailId.ToString());
        public void Unsubcribe(IObserver observer) => UserQuest.Ins?.Unsubscribe(observer);
        public void Notify() => UserQuest.Ins.Notify(DetailId.ToString());
    }
}