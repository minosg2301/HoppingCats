using Doozy.Engine.UI;
using System;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UISpinPointReward : BaseUIData<PointReward>
    {
        public TextMeshProUGUI pointText;
        public GameObject lockNode;
        public GameObject canClaimNode;
        public GameObject claimedNode;
        public UIButton button;

        public Action<UISpinPointReward> onClaimed;
        private bool unlocked;
        private bool canClaimed;

        public PointReward Reward { get; private set; }
        public bool Unlocked
        {
            set
            {
                unlocked = value;
                if(lockNode) lockNode.SetActive(!unlocked);
                if(canClaimNode) canClaimNode.SetActive(unlocked && canClaimed);
                if(claimedNode) claimedNode.SetActive(unlocked && !canClaimed);
                if(button) button.Interactable = unlocked && canClaimed;
            }
        }

        public bool CanClaim
        {
            set
            {
                canClaimed = value;
                if(canClaimNode) canClaimNode.SetActive(unlocked && canClaimed);
                if(claimedNode) claimedNode.SetActive(unlocked && !canClaimed);
                if(button) button.Interactable = unlocked && canClaimed;
            }
        }

        private void Start()
        {
            if(button) button.OnClick.OnTrigger.Event.AddListener(() => onClaimed.Invoke(this));
        }

        public override void SetData(PointReward reward)
        {
            Reward = reward;
            if(pointText) pointText.text = Reward.point.ToString();
        }
    }
}