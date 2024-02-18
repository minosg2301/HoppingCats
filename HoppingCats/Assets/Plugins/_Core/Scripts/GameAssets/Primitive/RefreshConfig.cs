using System;

namespace moonNest
{
    [Serializable]
    public class RefreshConfig
    {
        public bool enabled = false;
        public LimitConfig limit = new LimitConfig();
        public PeriodConfig period = new PeriodConfig();
    }

    public enum LimitType { Unlimit, ByAmount/*, BySlot */}
    [Serializable]
    public class LimitConfig
    {
        public LimitType type = LimitType.Unlimit;
        public int value = -1;
    }

    public enum PeriodType { Second, Day, Week, Month }
    [Serializable]
    public class PeriodConfig
    {
        public PeriodType type = PeriodType.Day;
        public int duration = 1;
    }
}