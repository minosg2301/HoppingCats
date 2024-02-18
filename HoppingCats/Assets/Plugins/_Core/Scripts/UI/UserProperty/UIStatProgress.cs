using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using I2.Loc;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIStatProgress : EnhancedScrollerCellView
    {
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI requireValueText;
        public UIUnlockContents unlockContents;
        public UIProgressReward reward;
        public UIProgressReward premiumReward;
        public DOTweenAnimation[] showAnimations;

        private RectTransform _rect;
        public RectTransform Rect { get { if (!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

        private Localize _nameLoc;
        public Localize NameLoc { get { if (!_nameLoc && nameText) _nameLoc = nameText.GetComponent<Localize>(); return _nameLoc; } }

        private StatProgressGroup group;
        public ProgressDetail Detail { get; private set; }

        public int RequireValue => Detail.requireValue;

        void Start()
        {
            if (reward) reward.onClaimClicked += DoClaimReward;
            if (premiumReward) premiumReward.onClaimClicked += DoClaimPremiumReward;
        }

        void OnDestroy()
        {
            if (reward) reward.onClaimClicked -= DoClaimReward;
            if (premiumReward) premiumReward.onClaimClicked -= DoClaimPremiumReward;
        }

        public void SetData(StatProgressGroup progressGroup, ProgressDetail progressDetail)
        {
            Detail = progressDetail;
            group = progressGroup;
            UpdateUI();
        }

        public virtual void UpdateUI()
        {
            if (group.Detail.type == StatProgressType.Passive) UpdateUIPassive();
            else UpdateUIActive();
        }

        private void UpdateUIActive()
        {
            var groupDetail = group.Detail;
            var requireValue = groupDetail.bufferedStatId == -1
                ? UserData.Stat(groupDetail.statId)
                : UserData.Stat(groupDetail.bufferedStatId);
            var passed = requireValue >= Detail.requireValue;
            UpdateUIUncheck(group.PaidPremium, passed);
        }

        private void UpdateUIPassive()
        {
            var groupDetail = group.Detail;
            int statValue = groupDetail.bufferedStatId == -1
                ? UserData.Stat(groupDetail.statId)
                : UserData.Stat(groupDetail.bufferedStatId);
            bool passed = statValue >= Detail.statValue;
            UpdateUIUncheck(group.PaidPremium, passed);
        }

        private void UpdateUIUncheck(bool paid, bool passed)
        {
            if (NameLoc) NameLoc.Term = Detail.displayName;
            else if (nameText) nameText.text = Detail.displayName;
            if (requireValueText) requireValueText.text = RequireValue.ToString();
            SetUnlockContents(Detail.UnlockContents);
            if (reward)
            {
                bool canClaim = group.CanClaimReward(Detail);
                reward.SetReward(Detail.reward);
                reward.Unlocked = passed;
                reward.CanClaim = passed && canClaim;
                reward.Claimed = passed && !canClaim;
            }
            if (premiumReward)
            {
                bool unlocked = paid && passed;
                bool canClaim = group.CanClaimReward(Detail, true);
                premiumReward.SetReward(Detail.premiumReward);
                premiumReward.Unlocked = unlocked;
                premiumReward.CanClaim = passed && canClaim;
                premiumReward.Claimed = unlocked && !canClaim;
            }
        }

        protected virtual void SetUnlockContents(List<UnlockContentDetail> unlockContentDetails)
        {
            if (unlockContents) unlockContents.SetData(unlockContentDetails);
        }

        private void DoClaimReward()
        {
            UserProgress.Ins.ClaimReward(group.Id, Detail);
            UpdateUI();
        }

        private void DoClaimPremiumReward()
        {
            UserProgress.Ins.ClaimReward(group.Id, Detail, true);
            UpdateUI();
        }

        protected internal virtual void PlayShowAnimation()
        {
            showAnimations.ForEach(anim =>
            {
                if (anim.tween == null)
                    anim.CreateTween();
                anim.DORestart();
            });
        }
    }
}