using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public abstract class BaseGroupAsset<T, Data, Group> : SingletonScriptObject<T>, ISerializationCallbackReceiver
    where T : BaseGroupAsset<T, Data, Group>
    where Data : BaseData
    where Group : BaseData
    {
        [SerializeField] private List<Data> datas = new List<Data>();
        [SerializeField] private List<Group> groups = new List<Group>();

        protected Dictionary<int, Data> dataMap = new Dictionary<int, Data>();
        protected readonly Dictionary<int, List<Data>> groupMap = new Dictionary<int, List<Data>>();

        public List<Group> Groups => groups.ToList();
        public List<Data> Datas => datas.ToList();

        /// <summary>
        /// Invoke when asset loaded
        /// </summary>
        protected virtual void Init() { OnAfterDeserialize(); }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            // init data map
            dataMap = datas.ToMap(d => d.id);

            UpdateGroupMap();
        }

        void UpdateGroupMap()
        {
            // init group map
            groupMap.Clear();
            List<Data> _datas = datas.ToList();
            foreach(Group group in groups)
            {
                var list = GetOrCreate(group.id);
                list.AddRange(_datas.FindAll(data => GetGroupId(data) == group.id));
                _datas.RemoveAll(list);
            }
        }

        protected abstract int GetGroupId(Data data);

        private List<Data> GetOrCreate(int groupId) => groupMap.GetOrCreate(groupId, (id) => new List<Data>());

        public void Add(Data data)
        {
            datas.Add(data);
            dataMap[data.id] = data;
            GetOrCreate(GetGroupId(data)).Add(data);
        }

        public void Remove(Data data)
        {
            datas.Remove(data);
            dataMap.Remove(data.id);
            GetOrCreate(GetGroupId(data)).Remove(data);
        }

        public Data Find(int id) => dataMap.TryGetValue(id, out var data) ? data : null;
        public List<Data> FindByGroup(int groupId) => groupMap.GetOrCreate(groupId, (id) => new List<Data>()).ToList();

        // Group
        public Group FindGroup(int groupId) => groups.Find(_ => _.id == groupId);
        public void AddGroup(Group group) => groups.Add(group);
        public void RemoveGroup(int groupId)
        {
            var group = groups.Find(g => g.id == groupId);
            var _datasByGroup = FindByGroup(groupId);

            _datasByGroup.ForEach(d => dataMap.Remove(d.id));
            datas.RemoveAll(_datasByGroup);
            groups.Remove(group);
            groupMap.Remove(groupId);
        }

#if UNITY_EDITOR
        public void Editor_DoSwapGroup(Group group1, Group group2)
        {
            int index1 = groups.IndexOf(group1);
            int index2 = groups.IndexOf(group2);
            groups.Remove(group1);
            groups.Remove(group2);
            if (index1 >= groups.Count) groups.Add(group2); else groups.Insert(index1, group2);
            if (index2 >= groups.Count) groups.Add(group1); else groups.Insert(index2, group1);
        }

        public void Editor_DoSwap(Data data1, Data data2)
        {
            int index1 = datas.IndexOf(data1);
            int index2 = datas.IndexOf(data2);
            if(index1 > -1 &&  index2 > -1)
            {
                datas[index1] = data2;
                datas[index2] = data1;
            }

            UpdateGroupMap();
        }

        public void UpdateList(int groupId, List<Data> newList)
        {
            groupMap[groupId] = newList;
            datas.Clear();
            foreach(var list in groupMap.Values)
            {
                datas.AddRange(list);
            }
        }
#endif
    }

}