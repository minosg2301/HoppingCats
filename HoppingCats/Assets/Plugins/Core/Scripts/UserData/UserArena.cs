using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    public class UserArena : RemotableUserData<FirestoreUserData>
    {
        public static UserArena Ins => LocalData.Get<UserArena>();

        #region serialize fields
        [SerializeField] private int season = 0;
        [SerializeField] private DateTime nextSeasonTime;
        [SerializeField] private SafeInt leagueReward = new SafeInt(-1);

        // battle pass
        [SerializeField] private int layerId = -1;
        [SerializeField] private bool paidPremium = false;
        [SerializeField] private SafeInt level = new SafeInt(-1);
        [SerializeField] private SafeInt exp = new SafeInt(0);
        [SerializeField] private List<int> claims = new List<int>();
        [SerializeField] private List<int> premiumClaims = new List<int>();
        #endregion

        private readonly Dictionary<int, RewardDetail> cachedRewards = new Dictionary<int, RewardDetail>();
        private readonly Dictionary<int, RewardDetail> cachedPremiumRewards = new Dictionary<int, RewardDetail>();

        #region private fields
        private int lastValue;
        #endregion

        #region properties
        /// <summary>
        /// Get Season Number
        /// </summary>
        public int Season => season;

        /// <summary>
        /// Wether have league reward can claim
        /// </summary>
        public bool CanClaimLeagueReward => leagueReward.Value != -1;

        /// <summary>
        /// Get league can claim reward
        /// </summary>
        public int LeagueReward => leagueReward.Value;

        /// <summary>
        /// Get last seconds util next season
        /// </summary>
        public double LastSeconds => nextSeasonTime.Subtract(DateTime.Now).TotalSeconds;

        /// <summary>
        /// Get requirement description
        /// </summary>
        public string RequireDescription => string.Format(ArenaAsset.Ins.requireDescription, Require);

        /// <summary>
        /// Get current require value
        /// </summary>
        public int Require => ArenaAsset.Ins.FindLevel(level.Value).requireValue;

        /// <summary>
        /// Get current level
        /// </summary>
        public int Level => level.Value;

        /// <summary>
        /// Get current exp
        /// </summary>
        public int EXP => exp.Value;

        /// <summary>
        /// Short-hand get Paid Battle Pass in User Property
        /// </summary>
        public bool PaidPremium => paidPremium;

        /// <summary>
        /// Check current level is max level
        /// </summary>
        public bool IsMaxLevel => level.Value >= ArenaAsset.Ins.finalLevel.level;

        /// <summary>
        /// Checking last reward can claim
        /// </summary>
        public bool CanClaimFinalReward => IsMaxLevel && exp.Value >= ArenaAsset.Ins.finalLevel.requireValue;

        /// <summary>
        /// Check have any reward can claim
        /// </summary>
        public bool HaveRewardCanClaim => claims.Count > 0 || premiumClaims.Count > 0;
        #endregion

        #region events
        public Action onNewSeason = delegate { };
        public Action onUpdated = delegate { };
        public Action onLeagueRewardClaimed = delegate { };
        #endregion

        #region override methods
        protected internal override void OnInit()
        {
            base.OnInit();
            nextSeasonTime = DateTime.Now;
        }

        protected internal override void OnLoad()
        {
            base.OnLoad();

            ArenaAsset arena = ArenaAsset.Ins;
            lastValue = UserData.Stat(arena.requireStatId);
            UserData.Ins.Subscribe(arena.requireStatId.ToString(), OnRequireUpdated);
            UserData.Ins.Subscribe(arena.premiumStatId.ToString(), OnPremiumUpdated);
        }
        #endregion

        #region public methods
        /// <summary>
        /// Update New Arena Season
        /// </summary>
        public void UpdateSeason()
        {
            if(nextSeasonTime <= DateTime.Now)
            {
                season++;
                nextSeasonTime = DateTime.Today.AddDays(ArenaAsset.Ins.seasonDuration);
                UpdateNewSeason();
            }
        }

        public RewardDetail GetReward(BattlePassLevel battlePassLevel, bool premium = false)
        {
            if(battlePassLevel == null) return null;

            int level = battlePassLevel.level;
            var rewards = premium ? cachedPremiumRewards : cachedRewards;
            if(rewards.TryGetValue(level, out var reward))
                return reward;

            reward = premium ? battlePassLevel.premiumReward : battlePassLevel.reward;
            var layer = ArenaAsset.Ins.GetLayerById(layerId);
            if(layer != null)
            {
                var levelLayer = ArenaAsset.Ins.GetLevelLayer(layer, level);
                if(levelLayer != null) reward = premium ? levelLayer.premiumReward : levelLayer.reward;
            }

            rewards[level] = reward;
            return reward;
        }

        /// <summary>
        /// Claim reward of battle pass level
        /// </summary>
        /// <param name="battlePassLevel"></param>
        /// <param name="premium"></param>
        public void ClaimReward(int level, bool premium = false)
        {
            List<int> list = premium ? premiumClaims : claims;
            if(!list.Contains(level)) return;

            list.Remove(level);
            BattlePassLevel battlePassLevel = ArenaAsset.Ins.FindLevel(level);
            RewardDetail rewardDetail = GetReward(battlePassLevel, premium);
            if(rewardDetail) RewardConsumer.ConsumeReward(rewardDetail);

            DirtyAndNotify();
            onUpdated();
        }

        /// <summary>
        /// Check reward id can claim
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool CanClaimReward(int level, bool premium = false)
            => (premium ? premiumClaims : claims).Contains(level);

        /// <summary>
        /// Claim final reward
        /// </summary>
        public void ClaimFinalReward()
        {
            if(!CanClaimFinalReward) return;

            RewardConsumer.ConsumeReward(ArenaAsset.Ins.finalLevel.reward);

            exp.Value = 0;
            level.Value += 1;

            DirtyAndNotify();
            onUpdated();
        }

        /// <summary>
        /// Claim league reward
        /// </summary>
        public void ClaimLeagueReward()
        {
            int value = leagueReward.Value;
            if(value != -1)
            {
                LeagueDetail league = ArenaAsset.Ins.FindLeague(value);
                if(league != null)
                {
                    leagueReward.Value = -1;
                    RewardConsumer.ConsumeReward(league.reward);
                    onLeagueRewardClaimed?.Invoke();
                }
                DirtyAndNotify();
            }
        }
        #endregion

        #region private methods
        void UpdateNewSeason()
        {
            LayerDetail layer = ArenaAsset.Ins.GetActiveLayer();
            layerId = layer ? layer.id : -1;
            level.Value = 1;
            exp.Value = 0;
            lastValue = 0;
            paidPremium = false;
            claims.Clear();
            premiumClaims.Clear();
            cachedRewards.Clear();
            cachedPremiumRewards.Clear();

            UserData.SetStat(ArenaAsset.Ins.premiumStatId, 0);
            UserData.SetStat(ArenaAsset.Ins.requireStatId, 0);

            DirtyAndNotify();
            onNewSeason?.Invoke();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="level"></param>
        void AddCanClaim(int level, bool premium = false)
        {
            List<int> list = premium ? premiumClaims : claims;
            if(!list.Contains(level)) list.Add(level);
        }

        void OnRequireUpdated(BaseUserData baseUserData)
        {
            int levelValue = level.Value;
            if(levelValue == -1) return;

            int expValue = exp.Value;
            var arena = ArenaAsset.Ins;
            int value = UserData.Stat(arena.requireStatId);
            int diff = value - lastValue;
            lastValue = value;

            if(diff > 0) expValue += diff;

            int require = Require;
            while(expValue >= require && levelValue < arena.MaxLevel)
            {
                BattlePassLevel battlePassLevel = arena.FindLevel(levelValue);
                AddCanClaim(battlePassLevel.level);
                if(paidPremium) AddCanClaim(battlePassLevel.level, true);

                levelValue++;
                expValue -= require;
                require = Require;
            }

            level.Value = levelValue;
            exp.Value = expValue;

            DirtyAndNotify();
            onUpdated();
        }

        void OnPremiumUpdated(BaseUserData baseUserData)
        {
            bool _premium = UserData.Stat(ArenaAsset.Ins.premiumStatId).AsInt == 1;
            if(paidPremium != _premium && _premium)
            {
                paidPremium = _premium;
                int levelValue = level.Value;
                for(int lvl = 1; lvl < levelValue; lvl++)
                    AddCanClaim(lvl, true);

                DirtyAndNotify();
                onUpdated();
            }
        }
        #endregion

        protected override void OnRemoteDataSync(FirestoreUserData remoteData) { }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData) { }
    }
}