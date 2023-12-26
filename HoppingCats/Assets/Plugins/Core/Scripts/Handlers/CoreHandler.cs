using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    [RequireComponent(typeof(TickInterval))]
    public class CoreHandler : SingletonMono<CoreHandler>
    {
        public static float ScaleFactor { get; internal set; } = 1f;

        #region mono methods

        public delegate void OnGameAction(ActionData action);
        public static event OnGameAction OnGameActionEvent = delegate { };

        public delegate void OnLogin(bool newDay);
        public static event OnLogin OnLoginEvent = delegate { };

        public void Init()
        {
            // add new callback
            UserCurrency.Ins.OnCurrencyUsed += OnCurrencyUsed;
            UserData.Ins.onLogin += UpdateLogin;

            // listen layerable user's stat updated
            LayerAsset.Ins.Groups
                .ForEach(group => UserData.Ins.Subscribe(group.statId.ToString(), (baseUserData) => OnLayerStatUpdated(group.statId)));

            // listen unlock condition
            UnlockContentAsset.Ins.StatIds
                .ForEach(statId => UserData.Ins.Subscribe(statId.ToString(), (baseUserData) => UpdateUnlockCondition(statId)));

            // listen user progress
            StatProgressAsset.Ins.groups
                .ForEach(group =>
                {
                    if (group.type == StatProgressType.Active && group.linkedStatId != -1)
                    {
                        UserData.Ins.Subscribe(group.statId.ToString(), (baseUserData) => UpdateActiveProgress(group));
                        if (group.premiumStatId != -1)
                            UserData.Ins.Subscribe(group.premiumStatId.ToString(), (baseUserData) => UpdatePremiumActiveProgress(group));
                    }
                    else if (group.type == StatProgressType.Passive)
                    {
                        UserData.Ins.Subscribe(group.progressId.ToString(), (baseUserData) => UpdatePassiveProgress(group));
                        if (group.premiumStatId != -1)
                            UserData.Ins.Subscribe(group.premiumStatId.ToString(), (baseUserData) => UpdatePremiumPassiveProgress(group));
                    }
                });

            // Quest Progress Drawer
            QuestProgressDrawer.Init();

            // unit collecting manager
            CollectingManager.Ins.Init();
        }

        protected override void Awake()
        {
            base.Awake();

            // Init Trigger Controller
            IAPTriggerController.Ins.Init();

            // start Tick Interval
            TickInterval tickInterval = GetComponent<TickInterval>();
            tickInterval.interval = 0.5f;
            tickInterval.onTick = OnUpdate;
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                nextDayLoginDT?.Kill();
                nextDayLoginDT = null;
            }
            else
            {
                if (UserData.SecondsForNextDay <= 0)
                {
                    DoLogin().ConfigureAwait(false);
                }
                else
                {
                    PrepareForNextDayLogin();
                }
            }
        }

        #endregion

        #region core update
        private List<Shop> refreshableShops = new List<Shop>();
        private List<Currency> growableCurrencies = new List<Currency>();

        /// <summary>
        /// Tick Interval callback
        /// </summary>
        void OnUpdate()
        {
            if (refreshableShops.Count > 0)
                refreshableShops.ForEach(shop => { if (shop.CanRefresh) shop.DoRefreshAsync(); });

            if (growableCurrencies.Count > 0)
                growableCurrencies.ForEach(currency => currency.UpdateGrow());

            if (GlobalConfig.Ins.StoreRemoteData)
                RemoteDataManager.Update();
        }
        #endregion

        #region callback events
        private void OnCurrencyUsed(Currency currency, UnitApplyType applyType)
        {
            DoAction(ActionDefinition.UseCurrencyAction(currency.Id), (int)Math.Abs(currency.Modifier));
        }
        #endregion

        #region update login

        Tween nextDayLoginDT;
        const float kGapLoginTime = 0.5f;

        internal async Task DoLogin()
        {
            await SynchronizableTime.CacheTimes(UserData.UserId);
            await UserData.Ins.UpdateLogin();
            PrepareForNextDayLogin();

            if (GlobalConfig.Ins.VerboseLog)
                Debug.LogFormat("Login {0} - {1}", UserData.ProfileId, UserData.UserId);
        }

        void PrepareForNextDayLogin()
        {
            float delay = UserData.SecondsForNextDay + kGapLoginTime;
            nextDayLoginDT?.Kill();
            nextDayLoginDT = DOVirtual.DelayedCall(delay, () =>
            {
                nextDayLoginDT = null;
                DoLogin().ConfigureAwait(false);
            });
        }

        /// <summary>
        /// called when user login in game
        /// </summary>
        static void UpdateLogin(bool newDay)
        {
            UserCurrency.Ins.UpdateLogin();
            UserShop.Ins.UpdateLogin();

            if (newDay)
            {
                UserArena.Ins.UpdateSeason();
                UserOnlineReward.Ins.UpdateNewDayLogin();
                UserStore.Ins.UpdateNewDayLogin();
                UserQuest.Ins.UpdateNewDayLogin(() => DoAction(ActionDefinition.LoginDayAction()));
                UserGatcha.Ins.UpdateNewDayLogin();
            }
            else
            {
                UserOnlineReward.Ins.UpdateLogin();
                UserStore.Ins.UpdateLogin();
                UserQuest.Ins.UpdateLogin();
            }

            Ins.growableCurrencies = UserCurrency.Ins.GrowableCurrencies;
            Ins.refreshableShops = UserShop.Ins.Shops.FindAll(g => g.RefreshEnabled);

            OnLoginEvent(newDay);
        }
        #endregion

        #region update layer
        /// <summary>
        /// Handle updated user's stat used for layer
        /// </summary>
        /// <param name="statName"></param>
        private void OnLayerStatUpdated(int statId)
        {
            LayerHelper.UpdateActiveLayer(statId);
        }
        #endregion

        #region update user progress
        /// <summary>
        /// Hanlde updated user'stat used for progress
        /// </summary>
        /// <param name="progressId"></param>
        private void UpdateActiveProgress(StatProgressGroupDetail group)
        {
            int progressValue = UserData.Stat(group.statId);
            ProgressDetail progressDetail = group.FindByRequireValue(progressValue);
            if (progressDetail) UserProgress.Ins.UnlockProgress(group.id, progressDetail);
        }

        private void UpdatePremiumActiveProgress(StatProgressGroupDetail group)
        {
            if (UserData.Stat(group.premiumStatId) == 1 && UserProgress.Ins.UnlockPremium(group.id))
            {
                int progressValue = UserData.Stat(group.statId);
                var progressDetails = group.progresses.FindAll(progress => progress.requireValue <= progressValue);
                UserProgress.Ins.UnlockProgresses(group.id, progressDetails, true);
            }
        }

        /// <summary>
        /// Hanlde updated user'stat used for progress
        /// </summary>
        /// <param name="progressId"></param>
        private void UpdatePassiveProgress(StatProgressGroupDetail group)
        {
            int statId = group.statId;
            int statvalue = UserData.Stat(statId);
            int progressId = group.progressId;
            int progressValue = UserData.Stat(progressId);
            if (group.accumulatable)
            {
                ProgressDetail progressDetail = group.FindByRequireValue(progressValue);
                if (progressDetail)
                {
                    // update user stat by progress detail
                    if (progressDetail.statValue > statvalue)
                    {
                        UserData.SetStat(group.statId, progressDetail.statValue);
                    }
                    else if (progressDetail.statValue < statvalue)
                    {
                        ProgressDetail progressByStat = group.FindByStatValue(statvalue);
                        if (progressByStat.downgradable)
                            UserData.SetStat(group.statId, progressDetail.statValue);
                        else
                            UserData.SetStat(group.progressId, progressByStat.requireValue);
                    }

                    // unlock progress if possible
                    UserProgress.Ins.UnlockProgress(group.id, progressDetail);
                }
            }
            else
            {
                int nextStat = statvalue + 1;
                ProgressDetail progressDetail = group.FindByStatValue(nextStat);
                if (progressDetail && progressValue > progressDetail.requireValue)
                {
                    int diff = progressValue - progressDetail.requireValue;
                    UserData.SetStat(group.statId, nextStat);
                    UserData.SetStat(group.progressId, diff);
                }
            }

            if (group.LinkedGroup)
            {
                UpdateActiveProgress(group.LinkedGroup);
            }
        }

        private void UpdatePremiumPassiveProgress(StatProgressGroupDetail group)
        {
            if (UserData.Stat(group.premiumStatId) == 1 && UserProgress.Ins.UnlockPremium(group.id))
            {
                int statValue = UserData.Stat(group.statId);
                var progressDetails = group.progresses.FindAll(progress => progress.statValue <= statValue);
                UserProgress.Ins.UnlockProgresses(group.id, progressDetails, true);

                if (group.LinkedGroup && UserProgress.Ins.UnlockPremium(group.LinkedGroup.id))
                {
                    int progressValue = UserData.Stat(group.progressId);
                    progressDetails = group.LinkedGroup.progresses.FindAll(progress => progress.requireValue <= progressValue);
                    UserProgress.Ins.UnlockProgresses(group.LinkedGroup.id, progressDetails, true);
                }
            }
        }
        #endregion

        #region update condition
        /// <summary>
        /// Handle updated user's stats for unlock condition
        /// </summary>
        /// <param name="statName"></param>
        private void UpdateUnlockCondition(int statId)
        {
            List<UnlockConditionDetail> unlockConditions = UnlockContentAsset.Ins.FindConditions(statId);
            if (unlockConditions == null || unlockConditions.Count == 0) return;
            int value = UserData.Stat(statId);

            // update feature unlocked
            foreach (Feature feature in UserData.Ins.Features)
            {
                if (feature.Locked)
                {
                    UnlockConditionDetail unlockCondition = unlockConditions.Find(u => u.id == feature.Config.unlockConditionId);
                    if (!unlockCondition || unlockCondition.requireValue <= value)
                    {
                        feature.Unlock();
                    }
                }
            }

            // update content unlocked
            List<UnlockConditionDetail> filteredConditions = unlockConditions.FindAll(u => u.requireValue <= value);
            if (filteredConditions != null)
            {
                List<Item> items = new List<Item>();
                foreach (UnlockConditionDetail unlockCondition in filteredConditions)
                {
                    List<Item> newUnlockedItems = UserInventory.Ins.UnlockItems(unlockCondition);
                    if (newUnlockedItems.Count > 0)
                        items.AddRange(newUnlockedItems);
                }

                if (items.Count > 0)
                {
                    UserInventory.Ins.onItemsUnlocked(items);
                }
            }
        }
        #endregion

        #region static methods
        public static void DoAction(ActionData action, int value = 1)
        {
            UserQuest.DoAction(action, value);
            IAPTriggerController.Ins.OnUserAction(action, value);
            OnGameActionEvent(action);
        }

        internal static void EnterLocation(LocationId location)
        {
            IAPTriggerController.Ins.OnEnterLocation(location);
        }

        internal static void ExitLocation(LocationId location)
        {
            // todo: handle when a location exited
        }
        #endregion
    }
}