using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public struct DistinctStat
    {
        public int id;
        public int value;

        [NonSerialized] public string name;

        public DistinctStat(int id, int value)
        {
            this.id = id;
            this.value = value;
            name = "";
        }
    }
}