using UnityEngine;

namespace moonNest
{
    public abstract class GroupObject<GroupDetail> where GroupDetail : BaseData
    {
        [SerializeField] private int id;

        public int Id => id;

        private GroupDetail _detail;
        public GroupDetail Detail { get { if(_detail == null) _detail = GetDetail(); return _detail; } }

        protected GroupObject(GroupDetail detail)
        {
            id = detail.id;
        }

        protected abstract GroupDetail GetDetail();
    }
}