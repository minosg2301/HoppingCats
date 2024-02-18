using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using moonNest.remotedata;

namespace moonNest
{
    [Preserve]
    public class UserStore : RemotableUserDataGroup<FirestoreUserData, InStorePackage, IAPPackage, InStorePackageGroup, IAPPackageGroup>
    {
        public static UserStore Ins => LocalData.Get<UserStore>();

        [SerializeField] private int totalBuy = 0;

        public int TotalBuy => totalBuy;

        #region  override methods
        protected override bool AutoCreateNewGroup(IAPPackageGroup group) => !group.refreshConfig.enabled;
        protected override InStorePackage CreateNew(IAPPackage package) => new InStorePackage(package);
        protected override IAPPackage FindDetail(int detailId) => IAPPackageAsset.Ins.Find(detailId);
        protected override List<IAPPackage> FindDetailsByGroup(int groupId) => IAPPackageAsset.Ins.FindByGroup(groupId);

        protected override IAPPackageGroup FindGroupDetail(IAPPackage package) => IAPPackageAsset.Ins.FindGroup(package.groupId);
        protected override IAPPackageGroup FindGroupDetail(int groupId) => IAPPackageAsset.Ins.FindGroup(groupId);
        protected override List<IAPPackageGroup> GroupDetails() => IAPPackageAsset.Ins.Groups;
        protected override InStorePackageGroup CreateNewGroup(IAPPackageGroup group) => new InStorePackageGroup(group);

        protected internal override void OnLoad()
        {
            base.OnLoad();

            if (GlobalConfig.Ins.StoreRemoteData)
            {

            }
        }
        #endregion

        public Action<InStorePackageGroup> onGroupUpdated = delegate { };
        public Action<InStorePackageGroup> onGroupRefreshed = delegate { };
        public Action<InStorePackage> onPackageUpdated = delegate { };
        public Action<InStorePackage> onPackageActive = delegate { };

        #region public methods
        /// <summary>
        /// Called every time user login in game
        /// </summary>
        public void UpdateLogin()
        {
            // update refreshable groups
            var refreshGroups = GroupDetails().FindAll(d => d.refreshConfig.enabled);
            foreach (var groupDetail in refreshGroups)
            {
                var group = FindGroup(groupDetail.id);
                if (group != null)
                {
                    group.refreshTime.GetTime(UserData.UserId).ConfigureAwait(false);

                    // if refreshable group is unlimit amount, update new iap packages if available
                    if (groupDetail.refreshConfig.limit.type == LimitType.Unlimit)
                    {
                        // find new iap packages
                        var iapPackages = FindDetailsByGroup(group.Id).FindAll(p => Find(p.id) == null);

                        // create new instore package
                        iapPackages.ForEach(p =>
                        {
                            var instorePackage = CreateNew(p);
                            Add(instorePackage);
                            onPackageUpdated(instorePackage);
                        });
                    }
                }
                else
                {
                    // refresh new group if it does not exists 
                    RefreshGroupAsync(GetOrCreateGroup(groupDetail));
                }
            }
        }

        /// <summary>
        /// Called every time user login in game on new day
        /// </summary>
        public void UpdateNewDayLogin()
        {
            var refreshGroups = GroupDetails().FindAll(d => d.refreshConfig.enabled);
            foreach (var groupDetail in refreshGroups)
                RefreshGroupAsync(GetOrCreateGroup(groupDetail));
        }

        /// <summary>
        /// Find iap package available in group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<InStorePackage> FindActiveInGroup(int groupId) => FindByGroup(groupId, p => p.active);

        public InStorePackage FindAndActive(int packageId)
        {
            InStorePackage instorePackage = Find(packageId);
            if (instorePackage && instorePackage.Activate())
            {
                DirtyAndNotify(packageId.ToString());
                onPackageUpdated(instorePackage);
            }
            return instorePackage;
        }

        internal void UpdateBuyPackage(InStorePackage package)
        {
            if (!package) return;

            totalBuy++;
            package.UpdateBuy();
            Notify(package.DetailId.ToString(), package.GroupId.ToString());
            onPackageUpdated(package);
            Save();
        }

        /// <summary>
        /// Check this group have a free offer
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HaveFreePackage(int groupId)
        {
            return FindActiveInGroup(groupId).Find(p => p.Available && p.Detail.free);
        }
        #endregion

        #region private methods
        async void RefreshGroupAsync(InStorePackageGroup group)
        {
            DateTime nextRefreshTime = await group.refreshTime.GetTime(UserData.UserId);
            if (nextRefreshTime <= DateTime.Now)
            {
                RefreshGroup(group);
                UpdateRefreshTimeAndNotify(group);
            }
            else
            {
                Notify(group.Id.ToString());
            }
        }

        void RefreshGroup(InStorePackageGroup group)
        {
            group.UpdateLayer();

            // remove all package in group
            RemoveByGroup(group.Id);

            var iapPackages = FindDetailsByGroup(group.Id);
            var refreshConfig = group.Detail.refreshConfig;

            // update new packages in assets to group by limit config
            switch (refreshConfig.limit.type)
            {
                case LimitType.ByAmount:
                    {
                        int maxAmount = refreshConfig.limit.value;
                        var showAlways = ListExt.RemoveAll(iapPackages, p => p.showAlways);
                        var instores = showAlways.Map(p => CreateNew(p));
                        while (instores.Count < maxAmount && iapPackages.Count > 0)
                            instores.Add(CreateNew(iapPackages.PopRandom()));
                        instores.ForEach(package => Add(package));
                    }
                    break;
                default:
                    {
                        iapPackages.ForEach(p => Add(CreateNew(p)));
                    }
                    break;
            }
        }

        /// <summary>
        /// Update new refresh time and notify for group
        /// </summary>
        /// <param name="group"></param>
        async void UpdateRefreshTimeAndNotify(InStorePackageGroup group)
        {
            // notify and callback that group is refreshed
            DirtyAndNotifyRefreshGroup(group);

            // update new refresh time
            await group.refreshTime.UpdateTime(UserData.UserId, group.Detail.refreshConfig.period);

            // due to time can be get from server, we notify again for next refresh time
            DirtyAndNotify(group.Id.ToString());
        }

        void DirtyAndNotifyRefreshGroup(InStorePackageGroup group)
        {
            string groupId = group.Id.ToString();
            DirtyAndNotify(groupId, "Refresh" + groupId);
            onGroupRefreshed(group);
        }

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