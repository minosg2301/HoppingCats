using UnityEngine;

namespace moonNest
{
    public abstract class DataObject<DetailConfig> where DetailConfig : BaseData
    {
        [SerializeField] protected int id;
        [SerializeField] protected int detailId;

        public int Id => id;
        public int DetailId => detailId;

        private DetailConfig _detail;
        public DetailConfig Detail { get { if(_detail == null) _detail = GetDetail(); return _detail; } }

        protected DataObject() { }

        protected DataObject(DetailConfig detail)
        {
            id = UnityEngine.Random.Range(0, int.MaxValue);
            detailId = detail.id;
        }

        protected virtual void Init(int id, int detailId)
        {
            this.id = id;
            this.detailId = detailId;
        }

        public override string ToString() => Detail.name;

        protected abstract DetailConfig GetDetail();

        public static implicit operator bool(DataObject<DetailConfig> exists) => exists != null;
    }
}