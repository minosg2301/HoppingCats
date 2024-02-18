using System;
using System.Collections.Generic;
using System.Linq;

namespace moonNest
{
    public class UnlockContentAsset : BaseGroupAsset<UnlockContentAsset, UnlockConditionDetail, UnlockConditionGroup>
    {
        public List<UnlockContentDetail> unlockContents = new List<UnlockContentDetail>();

        private Dictionary<int, UnlockContentDetail> unlockContentMap = new Dictionary<int, UnlockContentDetail>();

        public List<int> StatIds => groupMap.Keys.ToList();

        protected override int GetGroupId(UnlockConditionDetail unlockConditionDetail) => unlockConditionDetail.groupId;

        protected override void Init()
        {
            base.Init();

            unlockContentMap = unlockContents.ToMap(unlockContent => unlockContent.id);
        }

        /// <summary>
        /// Find unlock condition by id
        /// </summary>
        /// <param name="unlockConditionId"></param>
        /// <returns></returns>
        public UnlockConditionDetail FindCondition(int unlockConditionId) => Find(unlockConditionId);

        /// <summary>
        /// Find all unlock conditions by stat id
        /// </summary>
        /// <param name="statId"></param>
        /// <returns></returns>
        public List<UnlockConditionDetail> FindConditions(int statId) => FindByGroup(statId);

        /// <summary>
        /// Find unlock content by id
        /// </summary>
        /// <param name="unlockContentId"></param>
        /// <returns></returns>
        public UnlockContentDetail FindContent(int unlockContentId) => unlockContentMap.TryGetValue(unlockContentId, out var unlockContent) ? unlockContent : null;

        /// <summary>
        /// Add unlock content
        /// </summary>
        /// <param name="unlockContent"></param>
        public void AddContent(UnlockContentDetail unlockContent)
        {
            unlockContents.Add(unlockContent);
            unlockContentMap.Add(unlockContent.id, unlockContent);
        }

        /// <summary>
        /// Remove unlock content by id
        /// </summary>
        /// <param name="unlockContentId"></param>
        public void RemoveContent(int unlockContentId)
        {
            unlockContents.Remove(FindContent(unlockContentId));
            unlockContentMap.Remove(unlockContentId);
        }

        /// <summary>
        /// Helper function to update unlock content in progress which linked with unlock condition
        /// </summary>
        /// <param name="unlockContent"></param>
        public void UpdateLinkedProgress(UnlockContentDetail unlockContent)
        {
            if(unlockContent.conditionId == -1)
            {
                StatProgressAsset.Ins.groups.ForEach(group =>
                {
                    List<ProgressDetail> progresses = group.progresses.FindAll(progress => progress.unlockContentIds.Contains(unlockContent.id));
                    progresses.ForEach(progress =>
                    {
                        progress.unlockContentIds.Remove(unlockContent.id);
                        progress.UnlockContents = null;
                    });
                });
            }
            else
            {
                UnlockConditionDetail unlockCondition = FindCondition(unlockContent.conditionId);
                foreach(StatProgressGroupDetail group in StatProgressAsset.Ins.groups)
                {
                    if(unlockCondition.statId != group.statId) continue;

                    StatProgressGroupDetail actualGroup = group.LinkedGroup ? group.LinkedGroup : group;
                    List<ProgressDetail> progresses = actualGroup.progresses.FindAll(progress => progress.unlockContentIds.Contains(unlockContent.id));
                    progresses.ForEach(progress =>
                    {
                        progress.unlockContentIds.Remove(unlockContent.id);
                        progress.UnlockContents = null;
                    });

                    ProgressDetail linkedProgress;
                    if(group.LinkedGroup)
                    {
                        linkedProgress = group.progresses.Find(progress => progress.statValue == unlockCondition.requireValue);
                        linkedProgress = group.LinkedGroup.progresses.Find(progress => progress.requireValue == linkedProgress.requireValue);
                    }
                    else
                    {
                        linkedProgress = group.progresses.Find(progress => progress.requireValue == unlockCondition.requireValue);
                    }

                    if(linkedProgress)
                    {
                        linkedProgress.unlockContentIds.Add(unlockContent.id);
                        linkedProgress.UnlockContents = null;
                    }
                }
            }
        }
    }
}