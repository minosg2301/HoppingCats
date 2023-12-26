using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    public class UserOnlineReward : RemotableUserData<FirestoreUserData>
    {
        public static UserOnlineReward Ins => LocalData.Get<UserOnlineReward>();

        /// <summary>
        /// Key to get time from server
        /// Modified can cause bugs about time
        /// </summary>
        private const int kOnlineRewardTime = 99999999;

        #region Serialize Field
        [SerializeField] private bool firstTimeReceive = true;
        [SerializeField] private float onlineDuration;
        [SerializeField] private int layerId = -1;
        [SerializeField] private SynchronizableTime refreshTime = new SynchronizableTime(kOnlineRewardTime);
        [SerializeField] private Dictionary<int, OnlineReward> onlineRewards = new Dictionary<int, OnlineReward>();
        #endregion

        public bool FirstTimeReceive => firstTimeReceive;
        public float OnlineDuration => onlineDuration;
        public double LastSeconds => refreshTime.LocalTime.Subtract(DateTime.Now).TotalSeconds;

        private List<OnlineReward> _rewards = new List<OnlineReward>();
        public List<OnlineReward> Rewards
        {
            get
            {
                if (_rewards == null || _rewards.Count == 0)
                {
                    _rewards = onlineRewards.Values.ToList();
                    _rewards.SortAsc(o => o.Detail.minutes);
                }
                return _rewards;
            }
        }

        public OnlineReward CurrentReward => Rewards.Find(or => !or.Claimed);

        private DateTime onlineTime;

        public Action onRefreshed = delegate { };
        public Action<OnlineReward> onRewardClaimed = delegate { };

        #region override methods
        protected internal override void OnLoad()
        {
            base.OnLoad();

            onlineTime = DateTime.Now;

            LayerDetail layer = OnlineRewardAsset.Ins.GetLayerById(layerId);
            if (layer)
            {
                onlineRewards.Values.ForEach(onlineReward =>
                    onlineReward.Reward = OnlineRewardAsset.Ins.FindReward(layer, onlineReward.Id, onlineReward.RewardId));
            }
            else
            {
                onlineRewards.Values.ForEach(onlineReward =>
                    onlineReward.Reward = OnlineRewardAsset.Ins.FindReward(onlineReward.Id, onlineReward.RewardId));
            }
        }

        protected internal override void OnPause()
        {
            base.OnPause();

            onlineDuration += (float)DateTime.Now.Subtract(onlineTime).TotalSeconds;
            onlineTime = DateTime.Now;
        }

        protected internal override void OnQuit()
        {
            base.OnQuit();

            onlineDuration += (float)DateTime.Now.Subtract(onlineTime).TotalSeconds;
        }

        #endregion

        #region private methods
        void UpdateNewDay()
        {
            onlineTime = DateTime.Now;
            onlineDuration = 0;
            firstTimeReceive = true;
            _rewards.Clear();
            onlineRewards.Clear();
            layerId = -1;

            LayerDetail layer = OnlineRewardAsset.Ins.GetActiveLayer();
            if (layer)
            {
                layerId = layer.id;
                foreach (OnlineRewardDetail onlineRewardDetail in OnlineRewardAsset.Ins.onlineRewards)
                {
                    OnlineRewardLayer onlineRewardLayer = layer.onlineRewards.Find(o => o.onlineRewardId == onlineRewardDetail.id);
                    RewardDetail rewardDetail = onlineRewardLayer.rewards.Random();
                    onlineRewards[onlineRewardDetail.id] = new OnlineReward(onlineRewardDetail, rewardDetail.id) { Reward = rewardDetail };
                }
            }
            else
            {
                foreach (OnlineRewardDetail onlineRewardDetail in OnlineRewardAsset.Ins.onlineRewards)
                {
                    RewardDetail rewardDetail = onlineRewardDetail.rewards.Random();
                    onlineRewards[onlineRewardDetail.id] = new OnlineReward(onlineRewardDetail, rewardDetail.id) { Reward = rewardDetail };
                }
            }
        }
        #endregion

        #region public methods
        public void UpdateLogin()
        {
            // update local time from cached
            refreshTime.GetTime(UserData.UserId).ConfigureAwait(false);
        }

        public async void UpdateNewDayLogin()
        {
            DateTime nextRefreshTime = await refreshTime.GetTime(UserData.UserId);
            if (nextRefreshTime <= DateTime.Now)
            {
                UpdateNewDay();
                await refreshTime.UpdateTimeByDay(UserData.UserId, 1);
                DirtyAndNotify();
                onRefreshed?.Invoke();
            }
        }

        public void UpdateOnlineDuration()
        {
            onlineDuration += (float)DateTime.Now.Subtract(onlineTime).TotalSeconds;
            onlineTime = DateTime.Now;
            dirty = true;
        }

        public void DisableFirstTime()
        {
            firstTimeReceive = false;
            dirty = true;
        }

        public void ClaimReward()
        {
            UpdateOnlineDuration();

            if (CurrentReward == null || CurrentReward.Detail.Seconds > onlineDuration) return;

            var currentReward = CurrentReward;
            onlineTime = DateTime.Now;
            onlineDuration = 0;
            currentReward.Claimed = true;

            DirtyAndNotify();
            onRewardClaimed(currentReward);

            RewardConsumer.ConsumeReward(currentReward.Reward);
        }

        public void Logout()
        {
            onlineDuration += (float)DateTime.Now.Subtract(onlineTime).TotalSeconds;
        }
        #endregion

        #region remote methods

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {

        }
        #endregion
    }
}