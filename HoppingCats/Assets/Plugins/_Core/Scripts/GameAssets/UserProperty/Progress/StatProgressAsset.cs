using System;
using System.Collections.Generic;

namespace moonNest
{
    public class StatProgressAsset : SingletonScriptObject<StatProgressAsset>
    {
        public List<StatProgressGroupDetail> groups = new List<StatProgressGroupDetail>();

        public Action onGroupChanged;

        public List<int> StatIds => groups.Map(group => group.statId);

        /// <summary>
        /// Find group by id
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public StatProgressGroupDetail FindGroup(int groupId) => groups.Find(group => group.id == groupId);

        /// <summary>
        /// Create Progress Group base on stat definition
        /// </summary>
        /// <param name="stat"></param>
        public StatProgressGroupDetail CreateGroup(StatDefinition stat)
        {
            if(!stat) return null;

            StatProgressGroupDetail group = FindGroupByStat(stat.id);
            if(group) return group;

            group = new StatProgressGroupDetail(stat);
            groups.Add(group);
            onGroupChanged?.Invoke();
            return group;
        }

        /// <summary>
        /// Remove Progress Group base on stat definition
        /// </summary>
        /// <param name="stat"></param>
        public void DeleteGroup(StatDefinition stat)
        {
            groups.Remove(group => group.statId == stat.id);
            onGroupChanged?.Invoke();
        }

        /// <summary>
        /// Remove all segments by stat
        /// </summary>
        /// <param name="stat"></param>
        public void UpdateProgressType(StatDefinition stat)
        {
            StatProgressGroupDetail group = FindGroupByStat(stat.id);
            if(group && group.type != stat.progressType) group.SetType(stat.progressType);
        }

        /// <summary>
        /// Find Progress group by stat id
        /// </summary>
        /// <param name="statId"></param>
        /// <returns></returns>
        public StatProgressGroupDetail FindGroupByStat(int statId) => groups.Find(group => group.statId == statId);

        /// <summary>
        /// Find Progress group by progress id
        /// </summary>
        /// <param name="progressId"></param>
        /// <returns></returns>
        public StatProgressGroupDetail FindGroupByProgress(int progressId) => groups.Find(g => g.progressId == progressId);

        /// <summary>
        /// Short cut to get Range
        /// </summary>
        /// <param name="statId"></param>
        /// <param name="statValue"></param>
        /// <returns></returns>
        public Range GetRange(int statId, int statValue)
        {
            StatProgressGroupDetail group = FindGroupByStat(statId);
            if(group == null) throw new ArgumentException("Group by stat id does not exists!");

            return group.GetRange(statValue);
        }

        /// <summary>
        /// Shortcut to find progress detail by value
        /// </summary>
        /// <param name="statId"></param>
        /// <param name="statValue"></param>
        /// <returns></returns>
        public ProgressDetail Find(int statId, int statValue)
        {
            StatProgressGroupDetail group = FindGroupByStat(statId);
            if(group == null) throw new ArgumentException("Group by stat id does not exists!");

            return group.FindByStatValue(statValue);
        }

        /// <summary>
        /// Shortcut to find progress detail by require value
        /// </summary>
        /// <param name="statId"></param>
        /// <param name="statValue"></param>
        /// <returns></returns>
        public ProgressDetail FindByRequire(int statId, int requireValue)
        {
            StatProgressGroupDetail group = FindGroupByStat(statId);
            if(group == null) throw new ArgumentException("Group by stat id does not exists!");

            return group.FindByRequireValue(requireValue);
        }
    }
}