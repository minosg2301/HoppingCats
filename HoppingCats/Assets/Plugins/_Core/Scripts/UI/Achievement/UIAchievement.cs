using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIAchievement : MonoBehaviour, IObserver
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI claimedText;
        public Image icon;
        public UIProgress progress;
        public UIRewardDetail rewardContainer;
        public UIButton claimButton;

        public Achievement Achievement { get; private set; }

        protected virtual void Awake()
        {
            if(claimButton)
            {
                //claimButton.onClick.AddListener(DoClaim);
            }
        }

        protected virtual void OnEnable() => Achievement?.Subscibe(this);

        protected virtual void OnDisable() => Achievement?.Unsubcribe(this);

        public void SetData(Achievement achievement)
        {
            Achievement = achievement;
            achievement.Subscibe(this);
        }

        public virtual void OnNotify(IObservable data, string[] scopes)
        {
            if(titleText) titleText.text = Achievement.Detail.name;
            if(descriptionText) descriptionText.text = Achievement.Detail.description;
            if(icon) icon.sprite = Achievement.Detail.icon;
            if(progress) progress.SetProgress(Achievement.Progress, Achievement.Require.value);
            if(rewardContainer) rewardContainer.SetData(Achievement.Detail.reward);
            if(Achievement.Claimed && claimedText) claimedText.text = "Claimed";
            if(claimButton) claimButton.Interactable = Achievement.CanClaim && !Achievement.Claimed;
        }

        //protected virtual void DoClaim()
        //{
        //    if (Achievement.CanClaim && !Achievement.Claimed)
        //    {
        //        UserAchievement.Ins.Claim(Achievement);
        //    }
        //}
    }
}