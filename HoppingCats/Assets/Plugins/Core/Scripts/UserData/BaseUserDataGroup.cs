using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    /// <summary>
    /// Abstract class handles a data/group structure
    /// </summary>
    /// <typeparam name="Data"></typeparam>
    /// <typeparam name="DataDetail"></typeparam>
    /// <typeparam name="Group"></typeparam>
    /// <typeparam name="GroupDetail"></typeparam>
    public abstract class BaseUserDataGroup<Data, DataDetail, Group, GroupDetail> : BaseUserData
    where Data : DataObject<DataDetail>
    where DataDetail : BaseData
    where Group : GroupObject<GroupDetail>
    where GroupDetail : BaseData
    {
        /// <summary>
        /// Keep track of data by detail id
        /// </summary>
        [SerializeField] protected Dictionary<int, Data> datas = new Dictionary<int, Data>();

        /// <summary>
        /// Keep track of group by group id
        /// </summary>
        [SerializeField] protected Dictionary<int, Group> groups = new Dictionary<int, Group>();

        /// <summary>
        /// Keep track of list data by group id
        /// </summary>
        private readonly Dictionary<int, List<Data>> groupDataMap = new Dictionary<int, List<Data>>();

        protected virtual bool AutoCreateNewGroup(GroupDetail group) { return true; }
        protected virtual bool CanCreateData(DataDetail detail, Group group) { return true; }

        protected abstract Data CreateNew(DataDetail detail);
        protected abstract DataDetail FindDetail(int detailId);
        protected abstract List<DataDetail> FindDetailsByGroup(int groupId);

        protected virtual bool ShouldUpdateGroup(Group group) { return true; }
        protected abstract Group CreateNewGroup(GroupDetail groupDetail);
        protected abstract GroupDetail FindGroupDetail(int groupId);
        protected abstract GroupDetail FindGroupDetail(DataDetail detail);
        protected abstract List<GroupDetail> GroupDetails();

        /// <summary>
        /// Remove data not exists in asset
        /// Remove group if group not exist in asset
        /// Create group if possible
        /// Update new detail from asset
        /// </summary>
        protected internal override void OnLoad()
        {
            // remove data which detail does not exists in asset
            datas.Values
                .FindAll(data => FindDetail(data.DetailId) == null || FindGroupDetail(data.Detail) == null)
                .ForEach(data => datas.Remove(data.DetailId));

            // remove group which detail doest not exists in asset
            groups.Keys.ToList()
                .FindAll(groupId => FindGroupDetail(groupId) == null)
                .ForEach(groupId => groups.Remove(groupId));

            // init runtime data's map
            datas.Values.ForEach(data =>
            {
                GroupDetail group = FindGroupDetail(data.Detail);
                List<Data> inGroupDatas = groupDataMap.GetOrCreate(group.id, (id) => new List<Data>());
                inGroupDatas.Add(data);
            });

            // update groups
            List<GroupDetail> groupDetails = GroupDetails();
            groupDetails.ForEach(groupDetail =>
            {
                if (AutoCreateNewGroup(groupDetail))
                {
                    Group group = GetOrCreateGroup(groupDetail);
                    if (ShouldUpdateGroup(group))
                    {
                        FindDetailsByGroup(groupDetail.id)
                        .Where(detail => !Contains(detail.id))
                        .ForEach(detail =>
                        {
                            if (CanCreateData(detail, group))
                            {
                                Add(CreateNew(detail));
                            }
                        });
                    }
                }
            });
        }

        protected Group GetOrCreateGroup(GroupDetail groupDetail) => groups.GetOrCreate(groupDetail.id, (id) => CreateNewGroup(groupDetail));

        /// <summary>
        /// Add new data
        /// </summary>
        /// <param name="data"></param>
        protected void Add(Data data)
        {
            datas[data.DetailId] = data;
            FindByGroup(FindGroupDetail(data.Detail)).Add(data);
        }

        /// <summary>
        /// Remove data
        /// </summary>
        /// <param name="data"></param>
        protected void Remove(Data data)
        {
            datas.Remove(data.DetailId);
            FindByGroup(FindGroupDetail(data.Detail)).Remove(data);
        }

        /// <summary>
        /// Check detailId is exist
        /// </summary>
        /// <param name="detailId"></param>
        /// <returns></returns>
        public bool Contains(int detailId) => datas.ContainsKey(detailId);

        /// <summary>
        /// Find data by detailId
        /// </summary>
        /// <param name="detailId"></param>
        /// <returns></returns>
        public Data Find(int detailId) => datas.TryGetValue(detailId, out Data ret) ? ret : null;

        /// <summary>
        /// Find list of data by group detail
        /// </summary>
        /// <param name="groupDetail"></param>
        /// <returns></returns>
        private List<Data> FindByGroup(GroupDetail groupDetail)
        {
            if (groupDetail == null || !groups.ContainsKey(groupDetail.id))
                throw new NullReferenceException("GroupDetail is null");

            return groupDataMap.GetOrCreate(groupDetail.id, g => new List<Data>());
        }
        /// <summary>
        /// Find list of data by group id
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <param name="predicate">Condition of finding if any</param>
        /// <returns>List of datas</returns>
        public List<Data> FindByGroup(int groupId, Predicate<Data> predicate = null)
        {
            if (!groups.ContainsKey(groupId)) throw new NullReferenceException("Group id does not exists");

            if (predicate == null)
                return groupDataMap.GetOrCreate(groupId, g => new List<Data>());
            else
                return groupDataMap.GetOrCreate(groupId, g => new List<Data>()).FindAll(predicate);
        }

        /// <summary>
        /// Get all groups
        /// </summary>
        /// <returns></returns>
        public List<Group> FindAllGroups() => groups.Values.ToList();

        /// <summary>
        /// Find group by group id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public Group FindGroup(int groupId) => groups.ContainsKey(groupId) ? groups[groupId] : null;

        /// <summary>
        /// Remove group by group id
        /// </summary>
        /// <param name="groupId"></param>
        public void RemoveGroup(int groupId)
        {
            if (groups.ContainsKey(groupId))
            {
                RemoveByGroup(groupId);
                groups.Remove(groupId);
                groupDataMap.Remove(groupId);
            }
        }

        /// <summary>
        /// Remove all datas which have same group id
        /// </summary>
        /// <param name="groupId"></param>
        public void RemoveByGroup(int groupId)
        {
            List<Data> list = groupDataMap.GetOrCreate(groupId, g => new List<Data>());
            foreach (var data in list) datas.Remove(data.DetailId);
            list.Clear();
        }
    }
}