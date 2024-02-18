using Doozy.Engine.Progress;
using UnityEngine;

namespace moonNest
{
    public class UISpinPointProgress : MonoBehaviour
    {
        public Progressor progressor;
        public GameObject rewardContainer;
        readonly UIListContainer<PointReward, UISpinPointReward> listContainer = new UIListContainer<PointReward, UISpinPointReward>();

        public Spin Spin { get; private set; }
        public int MaxRequired { get; private set; }

        public void SetSpin(Spin spin)
        {
            if(spin.Detail.pointRewards.Count == 0) return;

            Spin = spin;

            MaxRequired = Spin.Detail.pointRewards.MaxBy(quest => quest.point).point;
            progressor.SetMin(0);
            progressor.SetMax(MaxRequired);
            Rect rect = rewardContainer.GetComponent<RectTransform>().rect;
            listContainer.SetList(rewardContainer.transform, Spin.Detail.pointRewards, ui =>
            {
                float percent = (float)ui.Reward.point / MaxRequired;
                ui.GetComponent<RectTransform>().anchoredPosition = new Vector2(rect.x + percent * rect.width, 0);
                ui.onClaimed = OnRewardClicked;
            });
        }

        protected virtual void OnRewardClicked(UISpinPointReward uiQuestKeyReward)
        {
            if(Spin.CanClaim(uiQuestKeyReward.Reward.point))
            {
                DoClaimReward(uiQuestKeyReward);
            }
        }

        protected void DoClaimReward(UISpinPointReward uiQuestKeyReward, int multiply = 1)
        {
            RewardConsumer.ConsumeReward(uiQuestKeyReward.Reward.reward, multiply);
            Spin.DoClaim(uiQuestKeyReward.Reward.point);

            uiQuestKeyReward.CanClaim = false;
        }

        public void SetValue(int point)
        {
            progressor.SetValue(point);

            listContainer.UIList.ForEach(uiReward =>
            {
                uiReward.Unlocked = uiReward.Reward.point <= point;
                uiReward.CanClaim = Spin.CanClaim(uiReward.Reward.point);
            });
        }
    }
}