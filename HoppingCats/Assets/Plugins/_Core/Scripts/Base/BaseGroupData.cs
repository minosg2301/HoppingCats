using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public abstract class BaseGroupData<Data, Group> where Data : BaseData where Group : BaseData
    {
        [SerializeField] private List<Data> datas = new List<Data>();
        [SerializeField] private List<Group> groups = new List<Group>();

        private readonly Dictionary<int, List<Data>> groupMap = new Dictionary<int, List<Data>>();

        public List<Group> Groups => groups.ToList();
        public List<Data> Datas => datas.ToList();

        public void Init()
        {
            groupMap.Clear();

            List<Data> _datas = datas.ToList();
            foreach(var group in groups)
            {
                List<Data> list = GetOrCreate(group.id);
                list.AddRange(_datas.FindAll(data => GetGroupId(data) == group.id));
                _datas.RemoveAll(list);
            }
        }

        protected abstract int GetGroupId(Data data);

        private List<Data> GetOrCreate(int groupId) => groupMap.GetOrCreate(groupId, (id) => new List<Data>());

        public void Add(Data data)
        {
            datas.Add(data);
            GetOrCreate(GetGroupId(data)).Add(data);
        }

        public void Remove(Data data)
        {
            datas.Remove(data);
            GetOrCreate(GetGroupId(data)).Remove(data);
        }

        public Data Find(int id) => datas.Find(_ => _.id == id);
        public List<Data> FindByGroup(int groupId) => groupMap.GetOrCreate(groupId, (id) => new List<Data>()).ToList();

        // Group
        public Group FindGroup(int groupId) => groups.Find(_ => _.id == groupId);
        public void AddGroup(Group group) => groups.Add(group);
        public void RemoveGroup(int groupId)
        {
            var group = groups.Find(g => g.id == groupId);
            var _groupedPackages = FindByGroup(groupId);

            datas.RemoveAll(_groupedPackages);
            groups.Remove(group);
            groupMap.Remove(groupId);
        }

        public void DoSwap(Data data1, Data data2)
        {
            int index1 = datas.IndexOf(data1);
            int index2 = datas.IndexOf(data2);
            datas.Remove(data1);
            datas.Remove(data2);
            datas.Insert(index1, data2);
            datas.Insert(index2, data1);

            Init();
        }

#if UNITY_EDITOR
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