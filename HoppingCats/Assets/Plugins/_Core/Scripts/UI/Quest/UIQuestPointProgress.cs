using Doozy.Engine.Progress;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIQuestPointProgress : MonoBehaviour
    {
        public TextMeshProUGUI progressText;
        public Progressor progressor;
        public GameObject rewardContainer;
        readonly UIListContainer<PointReward, UIQuestPointReward> listContainer = new UIListContainer<PointReward, UIQuestPointReward>();

        public QuestGroup QuestGroup { get; private set; }
        public int MaxRequired { get; private set; }

        public void SetQuestGroup(QuestGroup questGroup)
        {
            if(questGroup.Detail.pointRewards.Count == 0) return;

            QuestGroup = questGroup;

            MaxRequired = questGroup.Detail.pointRewards.MaxBy(quest => quest.point).point;
            progressor.SetMin(0);
            progressor.SetMax(MaxRequired);
            Rect rect = rewardContainer.GetComponent<RectTransform>().rect;
            listContainer.SetList(rewardContainer.transform, questGroup.Detail.pointRewards, ui =>
            {
                float percent = (float)ui.Reward.point / MaxRequired;
                ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.x + percent * rect.width, 0);
                ui.onClaimed = OnRewardClicked;
            });
        }

        protected virtual void OnRewardClicked(UIQuestPointReward uiQuestKeyReward)
        {
            if(QuestGroup.CanClaim(uiQuestKeyReward.Reward.point))
            {
                DoClaimReward(uiQuestKeyReward);
            }
        }

        protected void DoClaimReward(UIQuestPointReward uiQuestKeyReward, int multiply = 1)
        {
            RewardConsumer.ConsumeReward(uiQuestKeyReward.Reward.reward, multiply);
            QuestGroup.DoClaim(uiQuestKeyReward.Reward.point);
            uiQuestKeyReward.CanClaim = false;
        }

        public void SetValue(int point)
        {
            if(progressText) progressText.text = point + "/" + MaxRequired;
            progressor.SetValue(point, !progressor.gameObject.activeInHierarchy);
            listContainer.UIList.ForEach(uiReward =>
            {
                uiReward.Unlocked = uiReward.Reward.point <= point;
                uiReward.CanClaim = QuestGroup.CanClaim(uiReward.Reward.point);
            });
        }
    }
}