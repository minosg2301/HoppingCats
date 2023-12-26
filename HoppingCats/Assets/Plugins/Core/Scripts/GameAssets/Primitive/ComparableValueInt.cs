using System;

namespace moonNest
{
    [Serializable]
    public class ComparableValueInt
    {
        public int contentId = -1;
        public CompareFunc compareType = CompareFunc.Equal;
        public int value = 0;

        internal bool Compare(int statvalue)
        {
            switch(compareType)
            {
                case CompareFunc.LessThan: return statvalue < value;
                case CompareFunc.LessOrEqual: return statvalue <= value;
                case CompareFunc.Equal: return statvalue == value;
                case CompareFunc.GreaterOrEqual: return statvalue >= value;
                case CompareFunc.Greater: return statvalue > value;
                case CompareFunc.NotEqual: return statvalue != value;
                default: return true;
            }
        }
    }
}