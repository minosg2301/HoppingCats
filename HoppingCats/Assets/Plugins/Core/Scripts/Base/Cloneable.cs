using UnityEngine;

namespace moonNest
{
    public class Cloneable : System.ICloneable
    {
        public object Clone()
        {
            string json = JsonUtility.ToJson(this);
            Cloneable clone = (Cloneable)JsonUtility.FromJson(json, GetType());
            return clone;
        }
    }
}