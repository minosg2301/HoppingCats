using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using moonNest.remotedata;

namespace moonNest
{
    [Preserve]
    internal class UserSocialData : RemotableUserData<FirestoreUserData>
    {
        public static UserSocialData Ins => LocalData.Get<UserSocialData>();

        public event Action OnInviteLinkUpdated = delegate { };
        public event Action OnInviteCountUpdated = delegate { };

        [SerializeField] private string inviteLink = "";
        internal static string InviteLink
        {
            get { return Ins.inviteLink; }
            set { Ins.inviteLink = value; Ins.dirty = true; Ins.OnInviteLinkUpdated(); }
        }

        [SerializeField] private int inviteCount = 0;
        internal static int InviteCount
        {
            get { return Ins.inviteCount; }
            set
            {
                if (value != Ins.inviteCount)
                {
                    var ins = Ins;
                    ins.inviteCount = value;
                    ins.OnInviteCountUpdated();
                    ins.DirtyAndNotify();
                }
            }
        }

        [SerializeField] private List<int> InviteRewardIds = new List<int>();

        protected internal override void OnLoad()
        {
            base.OnLoad();

            InviteRewardIds ??= new List<int>();

            if (GlobalConfig.Ins.StoreRemoteData)
            {
                OnInviteLinkUpdated += HandleInviteLinkUpdated;
                OnInviteCountUpdated += HandleInviteCountUpdated;
            }
        }

        internal bool CanClaimInviteReward(int inviteRequire)
        {
            return !InviteRewardIds.Contains(inviteRequire);
        }

        internal void AddClaimedInviteRewards(int inviteRequire)
        {
            if (InviteRewardIds.Contains(inviteRequire))
                return;

            InviteRewardIds.Add(inviteRequire);
            DirtyAndNotify();

            if (RemoteData == null) return;
            RemoteData.AddRequest("InviteRewardIds", InviteRewardIds);
        }

        void HandleInviteLinkUpdated()
        {
            if (RemoteData == null) return;
            RemoteData.InviteLink = inviteLink;
            RemoteData.AddRequest("InviteLink", inviteLink);
        }

        void HandleInviteCountUpdated()
        {
            if (RemoteData == null) return;
            RemoteData.InviteCount = inviteCount;
            RemoteData.AddRequest("InviteCount", inviteCount);
        }

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            inviteLink = RemoteData.InviteLink;
            inviteCount = RemoteData.InviteCount;
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {
            RemoteData.InviteLink = inviteLink;
            remoteData.AddRequest("InviteLink", RemoteData.InviteLink);
            remoteData.AddRequest("InviteCount", RemoteData.InviteCount);
        }
    }
}