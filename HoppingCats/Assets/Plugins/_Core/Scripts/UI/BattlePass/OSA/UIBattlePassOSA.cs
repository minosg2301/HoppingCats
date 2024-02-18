using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moonNest
{
    /// <summary>
    /// Battle Pass Container Scroller, use OSA
    /// </summary>
    public class UIBattlePassOSA : OSA<BaseParamsWithPrefab, BattlePassViewHolder>, IObserver
    {
        public UIBattlePassLevel battlePassLevel;
        public UIBattlePassFinalReward finalReward;

        void OnValidate()
        {
            gameObject.name = "UIBattlePassOSA";
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            ResetItems(ArenaAsset.Ins.levels.Count + 1);
        }

        protected override void Start()
        {
            base.Start();

            Parameters.ItemPrefab.gameObject.SetActive(false);
            finalReward.gameObject.SetActive(false);
            UserArena.Ins.Subscribe(this);
            FocusCurrent(0f);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UserArena.Ins.Unsubscribe(this);
        }

        private void FocusCurrent(float duration)
        {
            int levelCount = ArenaAsset.Ins.levels.Count;
            int currentLevel = UserArena.Ins.Level;
            int focusIndex = currentLevel > levelCount ? levelCount : currentLevel - 1;
            SmoothScrollTo(focusIndex, duration, 0.5f);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if(IsInitialized)
            {
                FocusCurrent(0f);
            }
        }

        protected override BattlePassViewHolder CreateViewsHolder(int itemIndex)
        {
            BattlePassViewHolder instance = new BattlePassViewHolder(this);
            instance.Init(Parameters.ItemPrefab, Content, itemIndex);
            return instance;
        }

        protected override void UpdateViewsHolder(BattlePassViewHolder newOrRecycled)
        {
            var arena = ArenaAsset.Ins;
            bool lastIndex = newOrRecycled.ItemIndex >= arena.levels.Count;
            BattlePassLevel battlePassLevel = newOrRecycled.ItemIndex >= arena.levels.Count ? null : arena.levels[newOrRecycled.ItemIndex];

            if(!lastIndex) newOrRecycled.SetData(battlePassLevel);
            else newOrRecycled.SetFinalReward(arena.finalLevel.reward);

            if(newOrRecycled.RequestUpdateSize) ScheduleComputeVisibilityTwinPass();
        }
    }

    public class BattlePassViewHolder : BaseItemViewsHolder
    {
        readonly UIBattlePassOSA battlePassOSA;

        public UIBattlePassFinalReward FinalReward { get; private set; }
        public UIBattlePassLevel UIBattlePassLevel { get; private set; }
        public bool RequestUpdateSize { get; internal set; }
        public Vector2 SizeDelta { get; internal set; }

        static readonly Dictionary<int, bool> animateds = new();

        public BattlePassViewHolder(UIBattlePassOSA battlePassOSA)
        {
            this.battlePassOSA = battlePassOSA;
        }

        public override void CollectViews()
        {
            base.CollectViews();

            if (!UIBattlePassLevel)
            {
                UIBattlePassLevel = Object.Instantiate(battlePassOSA.battlePassLevel, root);
            }
        }

        public void SetData(BattlePassLevel battlePassLevel)
        {
            UIBattlePassLevel.gameObject.SetActive(true);
            UIBattlePassLevel.SetData(battlePassLevel);

            if(FinalReward)
            {
                Object.Destroy(FinalReward.gameObject);
                FinalReward = null;
                RequestUpdateSize = true;
                SizeDelta = UIBattlePassLevel.RectTransform.sizeDelta;                
            }

            if(!animateds.ContainsKey(battlePassLevel.level))
            {
                animateds[battlePassLevel.level] = true;
                UIBattlePassLevel.PlayShowAnimation();
            }
        }

        public void SetFinalReward(RewardDetail rewardDetail)
        {
            UIBattlePassLevel.gameObject.SetActive(false);
            if(!FinalReward)
            {
                FinalReward = Object.Instantiate(battlePassOSA.finalReward, root);
                FinalReward.gameObject.SetActive(true);
                FinalReward.RectTransform.anchoredPosition = Vector2.zero;
                FinalReward.SetReward(rewardDetail);
                RequestUpdateSize = true;
                SizeDelta = FinalReward.RectTransform.sizeDelta;
            }
        }

        public override void MarkForRebuild()
        {
            if(RequestUpdateSize)
            {
                RequestUpdateSize = false;
                root.sizeDelta = SizeDelta;
            }

            base.MarkForRebuild();
        }
    }
}