using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class StatProgressGroupDetail : BaseData
    {
        /// <summary>
        /// Type of progressf
        /// </summary>
        public StatProgressType type;

        /// <summary>
        /// default added value when new progress/segment created
        /// </summary>
        public int stepValue = 1;

        /// <summary>
        /// User stat id, updated by progress id
        /// id == statId
        /// </summary>
        public int statId = -1;

        /// <summary>
        /// User stat id which is used as max value ever reached
        /// </summary>
        public int bufferedStatId = -1;

        /// <summary>
        /// User stat id, used for update stat
        /// </summary>
        public int progressId = -1;

        /// <summary>
        /// User stat id which is used as max progress value ever reached
        /// </summary>
        public int bufferedProgressId = -1;

        /// <summary>
        /// True: reset progressId's value
        /// False: keep progressId's value
        /// </summary>
        public bool accumulatable = false;

        /// <summary>
        /// Enable premium reward for segments
        /// </summary>
        public bool premiumReward = true;

        /// <summary>
        /// Stat marked as premium paid for this progress
        /// </summary>
        public int premiumStatId = -1;

        /// <summary>
        /// Link this progress with progress id
        /// </summary>
        public bool linkedProgress = false;

        /// <summary>
        /// This progress is linked with other group which has same statId
        /// </summary>
        public int linkedStatId = -1;

        /// <summary>
        /// List of progress detail
        /// </summary>
        public List<ProgressDetail> progresses = new List<ProgressDetail>();

        [NonSerialized] private StatProgressGroupDetail _linkedGroup;
        public StatProgressGroupDetail LinkedGroup
        {
            get { if(!_linkedGroup && linkedProgress) _linkedGroup = StatProgressAsset.Ins.FindGroupByStat(progressId); return _linkedGroup; }
        }

        /// <summary>
        /// Constructor by stat definition
        /// </summary>
        /// <param name="statDefinition"></param>
        public StatProgressGroupDetail(StatDefinition statDefinition) : base(statDefinition.name)
        {
            id = statId = statDefinition.id;
            SetType(statDefinition.progressType);
        }

        /// <summary>
        /// Set type of group
        /// </summary>
        /// <param name="type"></param>
        public void SetType(StatProgressType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Find progress detail by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ProgressDetail Find(int id)
        {
            return progresses.Find(progress => progress.id == id);
        }

        /// <summary>
        /// Find progress detail by stat value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual ProgressDetail FindByStatValue(int statValue)
        {
            return progresses.Find(progress => progress.statValue == statValue);
        }

        /// <summary>
        /// Find progress detail by require value
        /// </summary>
        /// <param name="requireValue"></param>
        /// <returns>
        /// Null if group is non-accumulated and not active
        /// </returns>
        public virtual ProgressDetail FindByRequireValue(int requireValue)
        {
            return progresses.FindLast(p => requireValue >= p.requireValue);
        }

        /// <summary>
        /// Get Range by value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Range GetRange(int statValue)
        {
            if(type == StatProgressType.Active)
            {
                ProgressDetail current = progresses.Find(p => p.statValue == statValue);
                ProgressDetail next = progresses.Find(p => current.requireValue < p.requireValue);
                return new Range(0, next ? next.requireValue : current.requireValue);
            }
            else
            {
                ProgressDetail current = progresses.Find(p => p.statValue == statValue);
                ProgressDetail next = progresses.Find(p => current.requireValue < p.requireValue);
                if(accumulatable) return new Range(current.requireValue, next ? next.requireValue : current.requireValue);
                else return new Range(0, next ? next.requireValue : current.requireValue);
            }
        }

        /// <summary>
        /// Find list of linked progress by stat value
        /// </summary>
        /// <param name="statValue"></param>
        /// <returns></returns>
        public List<ProgressDetail> FindLinkedProgresses(int statValue)
        {
            if(type == StatProgressType.Passive)
            {
                ProgressDetail current = progresses.Find(p => p.statValue == statValue);
                ProgressDetail next = progresses.Next(current);
                int min = current.requireValue;
                int max = next ? next.requireValue : -1;
                if(max == -1)
                    return LinkedGroup.progresses.FindAll(progress => progress.requireValue >= min);
                else
                    return LinkedGroup.progresses.FindAll(progress => progress.requireValue >= min && progress.requireValue < max);
            }

            return null;
        }
    }
}