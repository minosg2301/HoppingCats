using System;

namespace moonNest
{
    [Serializable]
    public class GameEvent : BaseData
    {
        public GameEvent(string name) : base(name)
        {
        }
    }
}