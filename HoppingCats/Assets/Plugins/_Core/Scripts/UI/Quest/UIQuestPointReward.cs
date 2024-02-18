using Doozy.Engine.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace moonNest
{
    public class UIQuestPointReward : BaseUIData<PointReward>
    {
        public TextMeshProUGUI pointText;
        public GameObject lockNode;
        public GameObject canClaimNode;
        public GameObject claimedNode;
        public UIButton button;
        public GameObject notifyObj;

        public Action<UIQuestPointReward> onClaimed;
        private bool unlocked;
        private bool canClaimed;

        public Action OnCheckClaimRewardHandle;
        public Action OnClaimRewardHandle;

        public PointReward Reward { get; private set; }

        private DOTweenAnimation btnAnim;
        public DOTweenAnimation BtnAnim { get { if (!btnAnim) btnAnim = button.GetComponent<DOTweenAnimation>(); return btnAnim; } }

        public bool Unlocked
        {
            set
            {
                unlocked = value;
                if (lockNode) lockNode.SetActive(!unlocked);
                if (canClaimNode) canClaimNode.SetActive(unlocked && canClaimed);
                if (claimedNode) claimedNode.SetActive(unlocked && !canClaimed);
                if (button) button.Interactable = unlocked && canClaimed;
                if (BtnAnim != null)
                {
                    if (unlocked && canClaimed) BtnAnim.DOPlay();
                    else BtnAnim.DORewind();
                }
            }
        }

        public bool CanClaim
        {
            set
            {
                canClaimed = value;
                if (canClaimNode) canClaimNode.SetActive(unlocked && canClaimed);
                if (claimedNode) claimedNode.SetActive(unlocked && !canClaimed);
                //if(button) button.Interactable = unlocked && canClaimed;
                if (button)
                {
                    button.Interactable = unlocked && canClaimed;
                    button.transform.GetChild(0).GetComponent<Image>().DOFade(unlocked && canClaimed ? 1.0f : 0.5f, 0.1f);
                    button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().DOFade(unlocked && canClaimed ? 1.0f : 0.5f, 0.1f);
                    if (BtnAnim != null)
                    {
                        if (unlocked && canClaimed) BtnAnim.DOPlay();
                        else BtnAnim.DORewind();
                    }
                }
            }
        }

        private void Awake()
        {
            if (button) button.OnClick.OnTrigger.Event.AddListener(() =>
            {
                OnClick();
            });
        }

        private void OnClick()
        {
            OnCheckClaimRewardHandle?.Invoke();
        }

        public void ClaimReward()
        {
            onClaimed.Invoke(this);
            OnClaimRewardHandle?.Invoke();
            notifyObj.SetActive(false);
        }

        public override void SetData(PointReward reward)
        {
            Reward = reward;
            if (pointText) pointText.text = Reward.point.ToString();
        }
    }
}