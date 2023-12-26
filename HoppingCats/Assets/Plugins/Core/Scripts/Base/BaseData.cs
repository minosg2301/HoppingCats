using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace moonNest
{
    public abstract class BaseData : ICloneable
    {
        public int id;
        public string name = "";

        protected BaseData() { }

        public BaseData(string name)
        {
            id = Random.Range(0, int.MaxValue);
            this.name = name;
        }

        public object Clone()
        {
            string json = JsonUtility.ToJson(this);
            BaseData clone = (BaseData)JsonUtility.FromJson(json, GetType());
            clone.id = Random.Range(0, int.MaxValue);
            return clone;
        }

        public override string ToString() => name;

        public static implicit operator bool(BaseData exists) => exists != null;
    }
}