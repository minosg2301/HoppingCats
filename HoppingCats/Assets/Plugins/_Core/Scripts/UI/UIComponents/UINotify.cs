using UnityEngine;

namespace moonNest
{
    public class UINotify : MonoBehaviour
    {
        public NotifyType notify;
        public AchievementGroupId achievementGroupId;
        public QuestGroupId questGroupId;
        public ShopId shopId;
        public IAPGroupId iapGroupId;
        public QuestId questId;

        void Start()
        {
            switch (notify)
            {
                case NotifyType.Quest: UserQuest.Ins.Subscribe(new QuestObserver(this), questId.ToString()); break;
                case NotifyType.QuestGroup: UserQuest.Ins.Subscribe(new QuestGroupObserver(this), questGroupId.ToString()); break;
                case NotifyType.BattlePass: UserArena.Ins.Subscribe(new BattlePassObserver(this)); break;
                case NotifyType.Shop: UserShop.Ins.Subscribe(new ShopObserver(this), shopId.ToString()); break;
                case NotifyType.IAP: UserStore.Ins.Subscribe(new IAPGroupObserver(this), iapGroupId.ToString()); break;
            }
        }

        internal class AchievementGroupObserver : IObserver
        {
            private readonly UINotify notify;

            public AchievementGroupObserver(UINotify notify)
            {
                this.notify = notify;
            }

            public void OnNotify(IObservable data, string[] scopes)
            {
                var haveAchievementReward = UserAchievement.Ins.HaveAchievementReward(notify.achievementGroupId);
                notify.gameObject.SetActive(haveAchievementReward);
            }
        }

        internal class QuestGroupObserver : IObserver
        {
            private readonly UINotify notify;

            public QuestGroupObserver(UINotify notify)
            {
                this.notify = notify;
            }

            public void OnNotify(IObservable data, string[] scopes)
            {
                var haveQuestReward = UserQuest.Ins.HaveReward(notify.questGroupId);
                var haveKeyReward = UserQuest.Ins.HavePointReward(notify.questGroupId);
                notify.gameObject.SetActive(haveQuestReward || haveKeyReward);
            }
        }

        internal class QuestObserver : IObserver
        {
            private UINotify notify;

            public QuestObserver(UINotify notify)
            {
                this.notify = notify;
            }

            public void OnNotify(IObservable data, string[] scopes)
            {
                Quest quest = UserQuest.Ins.Find(notify.questId);
                notify.gameObject.SetActive(quest != null && quest.CanClaim && !quest.Claimed);
            }
        }

        internal class BattlePassObserver : IObserver
        {
            private readonly UINotify notify;

            public BattlePassObserver(UINotify notify)
            {
                this.notify = notify;
            }

            public void OnNotify(IObservable data, string[] scopes)
            {
                notify.gameObject.SetActive(UserArena.Ins.HaveRewardCanClaim);
            }
        }

        internal class IAPGroupObserver : IObserver
        {
            private readonly UINotify notify;

            public IAPGroupObserver(UINotify notify)
            {
                this.notify = notify;
            }

            public void OnNotify(IObservable data, string[] scopes)
            {
                notify.gameObject.SetActive(UserStore.Ins.HaveFreePackage(notify.iapGroupId));
            }
        }

        internal class ShopObserver : IObserver
        {
            private readonly UINotify notify;

            public ShopObserver(UINotify notify)
            {
                this.notify = notify;
            }

            public void OnNotify(IObservable data, string[] scopes)
            {
                Shop dailyOffer = UserShop.Ins.Find(notify.shopId);
                notify.gameObject.SetActive(dailyOffer.HaveFreeItem);
            }
        }
    }

    public enum NotifyType
    {
        Achievement, QuestGroup, BattlePass, IAP, Shop, Quest
    }
}