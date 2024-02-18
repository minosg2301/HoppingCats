using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    public class UIBaseRewardNode : MonoBehaviour
    {
        [SerializeField] private UIRewardDetail reward;
        [SerializeField] private GameObject claimNode;
        [SerializeField] private GameObject lockedNode;
        [SerializeField] private GameObject claimedNode;
        [SerializeField] private UIButton claimButton;

        public event Action onClaimClicked = delegate { };

        void Start()
        {
            if (claimButton) claimButton.OnClick.OnTrigger.Event.AddListener(OnClaimClicked);
        }

        protected internal virtual void OnClaimClicked()
        {
            onClaimClicked();
        }

        public void Reset()
        {
            if (!reward) reward = GetComponentInChildren<UIRewardDetail>();
            if (!claimButton) claimButton = GetComponentInChildren<UIButton>();
        }

        public bool Unlocked
        {
            set
            {
                if (lockedNode) lockedNode.SetActive(!value);
            }
        }

        public bool CanClaim
        {
            set
            {
                if (claimNode) claimNode.SetActive(value);
                if (claimButton) claimButton.Interactable = value;
            }
        }

        public bool Claimed
        {
            set
            {
                if (claimedNode) claimedNode.SetActive(value);
            }
        }

        public void SetReward(RewardDetail rewardDetail)
        {
            reward.SetData(rewardDetail);
        }
    }
}