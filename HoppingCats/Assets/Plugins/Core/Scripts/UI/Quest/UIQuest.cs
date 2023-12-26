using UnityEngine;
using TMPro;
using System;
using I2.Loc;
using Doozy.Engine.UI;
using Doozy.Engine.Progress;

namespace moonNest
{
    public class UIQuest : BaseUIData<Quest>, IObserver
    {
        public QuestId questId = -1;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI pointRewardText;
        public Progressor progressor;
        public UIRewardDetail rewardsContainer;
        public UIButton claimButton;
        public UIButton navigationButton;
        public bool hideClaimButtonWhenInProgress;
        public bool hideClaimButtonWhenClaimed;
        public GameObject claimedNode;
        public GameObject inProgressNode;
        public UIProgress progress;

        public bool AutoConsumeReward { get; set; } = true;
        public Quest Quest { get; private set; }

        private QuestGroup _questGroup;
        public QuestGroup QuestGroup { get { if (_questGroup == null) _questGroup = UserQuest.Ins.FindGroup(Quest.GroupId); return _questGroup; } }

        private Localize _nameLoc;
        public Localize NameLoc { get { if (nameText && !_nameLoc) _nameLoc = nameText.GetComponent<Localize>(); return _nameLoc; } }

        private Localize _descLoc;
        public Localize DescLoc { get { if (descriptionText && !_descLoc) _descLoc = descriptionText.GetComponent<Localize>(); return _descLoc; } }

        public Action<UIQuest> onQuestClaimed;

        protected virtual void Start()
        {
            if (claimButton) claimButton.OnClick.OnTrigger.Event.AddListener(DoClaim);
            if (navigationButton)
            {
                if (Quest.Detail.isNavigation)
                    navigationButton.OnClick.OnTrigger.Event.AddListener(DoNavigation);
                else navigationButton.DisableButton();
            }
        }

        protected virtual void OnEnable()
        {
            if (questId != -1) Quest = UserQuest.Ins.Find(questId);
            Quest?.Subscibe(this);
        }

        protected virtual void OnDisable()
        {
            Quest?.Unsubcribe(this);
        }

        public override void SetData(Quest quest)
        {
            if (Quest) Quest.Unsubcribe(this);
            Quest = quest;
            Quest.Subscibe(this);
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            UpdateUI();
        }

        protected virtual void UpdateUI()
        {
            if (NameLoc) NameLoc.Term = Quest.Detail.displayName;
            else if (nameText) nameText.text = Quest.Detail.displayName;

            if (DescLoc) DescLoc.Term = Quest.Detail.description;
            else if (descriptionText) descriptionText.text = Quest.Detail.description;

            int progressValue = Quest.progress.value;
            int require = Quest.IsUseMultiRequires ? Quest.ActionRequires.count : (Quest.StatRequire.statId != -1 ? Quest.StatRequire.value : Quest.ActionRequire.count);
            if (progressor)
            {
                progressor.SetMin(0);
                progressor.SetMax(require);
                progressor.SetValue(progressValue);
            }

            if (progress)
            {
                progress.SetProgress(progressValue, require);
            }

            if (pointRewardText && Quest.Detail.pointEnabled) pointRewardText.text = Quest.Detail.point.ToString();
            if (rewardsContainer) rewardsContainer.SetData(Quest.Reward);

            if (claimButton)
            {
                claimButton.Interactable = Quest.CanClaim && !Quest.Claimed;
                claimButton.gameObject.SetActive(!hideClaimButtonWhenInProgress || Quest.CanClaim);
                if (Quest.Claimed && hideClaimButtonWhenClaimed)
                    claimButton.gameObject.SetActive(false);
            }

            if (claimedNode) claimedNode.SetActive(Quest.Claimed);
            if (inProgressNode) inProgressNode.SetActive(!Quest.CanClaim);

            if (navigationButton && Quest.Detail.isNavigation)
            {
                if (Quest.CanClaim) navigationButton.DisableButton();
                else navigationButton.EnableButton();
            }
        }

        public virtual void DoClaim()
        {
            if (Quest.CanClaim && !Quest.Claimed)
            {
                onQuestClaimed?.Invoke(this);
                if (AutoConsumeReward) UserQuest.DoClaim(Quest);
            }
        }

        public virtual void DoNavigation()
        {
            if (Quest.Detail.isNavigation)
                NavigationHandler.Ins.DoNavigate(Quest.Detail.navigationId); 
        }
    }
}