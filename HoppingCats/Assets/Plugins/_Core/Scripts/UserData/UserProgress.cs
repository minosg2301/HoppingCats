using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using moonNest.remotedata;

namespace moonNest
{
    [Preserve]
    public class UserProgress : RemotableUserData<FirestoreUserData>
    {
        public static UserProgress Ins => LocalData.Get<UserProgress>();

        #region serialize fields
        /// <summary>
        /// Keep track of StatProgress by detail id
        /// </summary>
        [SerializeField] protected Dictionary<int, StatProgressGroup> groups = new Dictionary<int, StatProgressGroup>();
        #endregion

        #region events
        public Action<StatProgressGroup> onPremiumUnlocked = delegate { };
        public Action<StatProgressGroup, bool> onClaimed = delegate { };
        public Action<StatProgressGroup> onUnlocked = delegate { };
        public Action<StatProgressGroup> onReset = delegate { };
        #endregion

        #region override methods
        /// <summary>
        /// Called when save data is load
        /// </summary>
        protected internal override void OnLoad()
        {
            base.OnLoad();

            // remove group which detail does not exists in asset
            groups.Values.ToList()
                .FindAll(group => StatProgressAsset.Ins.FindGroup(group.Id) == null)
                .ForEach(group => groups.Remove(group.Id));

            foreach (StatProgressGroupDetail groupDetail in StatProgressAsset.Ins.groups)
            {
                if (!groups.ContainsKey(groupDetail.id))
                    groups[groupDetail.id] = new StatProgressGroup(groupDetail);
            }

            if (GlobalConfig.Ins.StoreRemoteData)
            {

            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Find group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public StatProgressGroup FindGroup(int groupId)
            => groups.TryGetValue(groupId, out var group) ? group : null;

        public StatProgressGroup ResetGroup(int groupId)
        {
            if (groups.TryGetValue(groupId, out var group))
            {
                group.Reset();
                DirtyAndNotify(groupId);
                onReset(group);
                return group;
            }
            return null;
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Unlock a progress in group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="progressDetail"></param>
        /// <param name="premium"></param>
        internal void UnlockProgress(int groupId, ProgressDetail progressDetail)
        {
            if (!groups.TryGetValue(groupId, out var group) || !progressDetail) return;
            if (group.UpdateRewardCanClaim(progressDetail))
            {
                DirtyAndNotify(groupId);
                onUnlocked(group);
            }
        }

        /// <summary>
        /// Unlock progresses in group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="progressDetails"></param>
        /// <param name="premium"></param>
        internal void UnlockProgresses(int groupId, List<ProgressDetail> progressDetails, bool premium)
        {
            if (!groups.TryGetValue(groupId, out var group)) return;
            if (premium && !group.PaidPremium) return;
            bool ret = false;
            progressDetails.ForEach(progress => { ret |= group.UpdateRewardCanClaim(progress, premium); });
            if (ret)
            {
                DirtyAndNotify(groupId);
                onUnlocked(group);
            }
        }

        /// <summary>
        /// Claim reward if any
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="progressDetail"></param>
        /// <param name="premium">Reward is premium or not</param>
        internal void ClaimReward(int groupId, ProgressDetail progressDetail, bool premium = false)
        {
            if (!groups.TryGetValue(groupId, out var group) || !progressDetail) return;
            if (group.ClaimReward(progressDetail, premium))
            {
                DirtyAndNotify(groupId);
                onClaimed(group, premium);
            }
        }

        /// <summary>
        /// Unlock premium for group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        internal bool UnlockPremium(int groupId)
        {
            if (!groups.TryGetValue(groupId, out var group)) return false;
            if (group.UnlockPremium()) { DirtyAndNotify(groupId); onPremiumUnlocked(group); return true; }
            return false;
        }

        /// <summary>
        /// Shorthand to dirty and notify group
        /// </summary>
        /// <param name="groupId"></param>
        void DirtyAndNotify(int groupId) { dirty = true; Notify(groupId.ToString()); }

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            throw new NotImplementedException();
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}