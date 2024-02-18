using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class SynchronizableTime
    {
        static readonly Dictionary<int, CachedTime> cachedTimes = new Dictionary<int, CachedTime>();
        public static async Task CacheTimes(string key)
        {
#if UNITY_EDITOR
            if (GlobalConfig.Ins.CheatNewDay)
            {
                return;
            }
#endif
        }

        static CachedTime GetOrCreateCachedTime(int id)
        {
            if (!cachedTimes.ContainsKey(id))
                cachedTimes[id] = new CachedTime(DateTime.Now);
            return cachedTimes[id];
        }

        [SerializeField] private int id;
        [SerializeField] private DateTime localTime;

        /// <summary>
        /// Get next refesh time in local
        /// </summary>
        /// <returns></returns>
        public DateTime LocalTime => localTime;

        /// <summary>
        /// Lấy khoảng cách của thời gian so với hiện tại
        /// </summary>
        public double DiffSeconds => localTime.Subtract(DateTime.Now).TotalSeconds;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public SynchronizableTime() { }

        public SynchronizableTime(int id)
        {
            this.id = id;
            localTime = DateTime.Now;
        }

        /// <summary>
        /// Update time by seconds
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        public async Task UpdateTimeBySecond(string key, int seconds)
        {
#if UNITY_EDITOR
            if (GlobalConfig.Ins.CheatNewDay)
            {
                localTime = DateTime.Now.AddSeconds(seconds);
            }
#endif
            localTime = DateTime.Now.AddSeconds(seconds);
        }

        /// <summary>
        /// Update time by days
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        public async Task UpdateTimeByDay(string key, int day)
        {
#if UNITY_EDITOR
            if (GlobalConfig.Ins.CheatNewDay)
            {
                localTime = DateTime.Today.AddDays(day);
            }
#endif
            localTime = DateTime.Today.AddDays(day);
        }

        /// <summary>
        /// Update time by week
        /// </summary>
        /// <param name="key"></param>
        /// <param name="week"></param>
        /// <returns></returns>
        public async Task UpdateTimeByWeek(string key, int week)
        {
#if UNITY_EDITOR
            if (GlobalConfig.Ins.CheatNewDay)
            {
                localTime = DateTime.Today.AddDays(week * 7);
            }
#endif
            localTime = DateTime.Today.AddDays(week * 7);
        }

        /// <summary>
        /// Update time by month
        /// </summary>
        /// <param name="key"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task UpdateTimeByMonth(string key, int month)
        {
#if UNITY_EDITOR
            if (GlobalConfig.Ins.CheatNewDay)
            {
                localTime = DateTime.Today.AddMonths(month);
            }
#endif
            localTime = DateTime.Today.AddMonths(month);
        }

        /// <summary>
        /// Update time with dynamic period config
        /// </summary>
        /// <param name="key"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task UpdateTime(string key, PeriodConfig period)
        {
            if (period == null) return;

            switch (period.type)
            {
                case PeriodType.Second: await UpdateTimeBySecond(key, period.duration); break;
                case PeriodType.Day: await UpdateTimeByDay(key, period.duration); break;
                case PeriodType.Week: await UpdateTimeByWeek(key, period.duration); break;
                case PeriodType.Month: await UpdateTimeByMonth(key, period.duration); break;
            }
        }

        /// <summary>
        /// Get time from server or local
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<DateTime> GetTime(string key)
        {
#if UNITY_EDITOR
            if (GlobalConfig.Ins.CheatNewDay)
            {
                if (!cachedTimes.TryGetValue(id, out var cached))
                {
                    localTime = DateTime.Today.AddDays(-1);
                    GetOrCreateCachedTime(id).SetTime(localTime);
                }
                return localTime;
            }
#endif
            return localTime;
        }

        /// <summary>
        /// Set local time<br/>
        /// Do nothing when GlobalConfig.syncDate is enabled
        /// </summary>
        /// <param name="dateTime"></param>
        internal void SetLocalTime(DateTime dateTime)
        {
            if (GlobalConfig.Ins.SyncDate) return;
            localTime = dateTime;
        }
    }

    /// <summary>
    /// Cached Time used for checking server time is valid to used
    /// If Server time is invalid (cached for long time), then get from server
    /// </summary>
    class CachedTime
    {
        DateTime lastCached;
        DateTime time;

        public CachedTime(DateTime dateTime)
        {
            SetTime(dateTime);
        }

        internal void SetTime(DateTime dateTime)
        {
            time = dateTime;
            lastCached = DateTime.Now;
        }

        /// <summary>
        /// Get time pass to value params
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        ///     TRUE: time is valid to use
        ///     FALSE: should cache new server time
        /// </returns>
        public bool TryGet(out DateTime value)
        {
            value = time;

            // if lastCached is just 5 seconds ago, consider time is valid ( time can not be hack ... maybe) ---> return TRUE
            // else consider time is invalid --> return FALSE
            return DateTime.Compare(lastCached.AddSeconds(5), DateTime.Now) > 0;
        }
    }
}