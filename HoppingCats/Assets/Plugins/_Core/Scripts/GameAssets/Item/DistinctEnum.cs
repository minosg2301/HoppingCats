using System;

namespace moonNest
{
    [Serializable]
    public struct DistinctEnum
    {
        public int id;
        public byte value;

        [NonSerialized] public string name;

        public DistinctEnum(int id, byte value)
        {
            this.id = id;
            this.value = value;
            name = "";
        }
    }
}