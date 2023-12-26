using UnityEngine;

namespace moonNest
{
    public abstract class BaseUIData<T> : MonoBehaviour
    {
        public abstract void SetData(T t);
    }
}