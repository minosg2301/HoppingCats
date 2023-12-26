using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using moonNest.remotedata;

namespace moonNest
{
    public class UserQuest : RemotableUserDataGroup<FirestoreUserData, Quest, QuestDetail, QuestGroup, QuestGroupDetail>
    {
        public static UserQuest Ins => LocalData.Get<UserQuest>();

        #region override methods
        protected override bool AutoCreateNewGroup(QuestGroupDetail group) => group.activeOnLoad && group.refreshConfig.type != QuestRefeshType.OnTime;
        protected override bool CanCreateData(QuestDetail quest, QuestGroup group) => quest.activeOnLoad && !group.removedQuests.Contains(quest.id);

        protected override Quest CreateNew(QuestDetail detail)
        {
            var quest = new Quest(detail);
            var requireStatId = quest.Detail.statRequire.statId;
            if (requireStatId != -1)
            {
                UserData.Ins.Subscribe(requireStatId.ToString(), (data) => UpdateQuestByStat(quest, requireStatId));
            }
            return quest;
        }
        protected override QuestDetail FindDetail(int detailId) => QuestAsset.Ins.Find(detailId);
        protected override List<QuestDetail> FindDetailsByGroup(int groupId) => QuestAsset.Ins.FindByGroup(groupId);

        protected override bool ShouldUpdateGroup(QuestGroup group) => !group.removed;
        protected override QuestGroupDetail FindGroupDetail(QuestDetail detail) => QuestAsset.Ins.FindGroup(detail.groupId);
        protected override QuestGroupDetail FindGroupDetail(int groupId) => QuestAsset.Ins.FindGroup(groupId);
        protected override List<QuestGroupDetail> GroupDetails() => QuestAsset.Ins.Groups;
        protected override QuestGroup CreateNewGroup(QuestGroupDetail groupDetail) => new QuestGroup(groupDetail);

        protected internal override void OnLoad()
        {
            base.OnLoad();
        }
        #endregion

        #region static methods
        internal static void DoAction(ActionData action, int value = 1, List<Quest> listQuest = null)
        {
            List<Quest> updatedQuests = new List<Quest>();
            List<QuestGroup> updatedQuestGroups = new List<QuestGroup>();
            List<int> groupIds = new List<int>();

            UserQuest userQuest = Ins;
            List<Quest> unclaimedQuests = userQuest.FindByAction(action.id).FindAll(quest => !quest.Claimed);
            List<Quest> checkingQuests = listQuest ?? unclaimedQuests;
            foreach (Quest quest in checkingQuests)
            {
                bool satify = false;

                // Check Multi Action
                if (quest.IsUseMultiRequires)
                {
                    var requires = quest.ActionRequires;
                    satify = requires.Satify(action);
                }
                else
                {
                    ActionRequire require = quest.ActionRequire;
                    satify = require.Satify(action);
                }
                if (satify)
                {
                    quest.AddProgress(value);

                    // auto claim point if possible
                    if (quest.QuestGroup.Detail.pointAutoClaimed && quest.Detail.pointEnabled
                        && quest.CanClaim && !quest.Claimed)
                    {
                        quest.Claimed = true;
                        quest.QuestGroup.AddPoint(quest.Detail.point);
                        if (!updatedQuestGroups.Contains(quest.QuestGroup))
                            updatedQuestGroups.Add(quest.QuestGroup);
                    }
                    quest.Notify();
                    updatedQuests.Add(quest);
                    if (!groupIds.Contains(quest.GroupId))
                        groupIds.Add(quest.GroupId);
                }
                userQuest.dirty = true;
            }

            userQuest.Notify(groupIds.Map(groupId => groupId.ToString()).ToArray());

            if (updatedQuestGroups.Count > 0)
            {
                foreach (var group in updatedQuestGroups)
                {
                    userQuest.onGroupUpdated(group);
                }
            }

            if (updatedQuests.Count > 0)
                userQuest.onQuestUpdated(updatedQuests);
        }

        public static void DoClaim(Quest quest, int rewardMultiply = 1)
        {
            if (quest.CanClaim && !quest.Claimed)
            {
                UserQuest userQuest = Ins;
                QuestGroup group = userQuest.FindGroup(quest.GroupId);
                QuestGroupDetail groupDetail = group.Detail;

                quest.Claimed = true;
                CoreHandler.DoAction(ActionDefinition.CompleteQuestAction(group.Id));

                List<Quest> newActivedQuests = new List<Quest>();
                if (quest.Detail.activeOnCompleteds.Count > 0)
                {
                    newActivedQuests = userQuest.ActiveQuestInGroup(group, quest.Detail.activeOnCompleteds);
                }

                if (quest.Detail.removeOnClaimed)
                {
                    group.removedQuests.Add(quest.DetailId);
                    userQuest.Remove(quest);
                    userQuest.Notify("Refresh" + group.Id.ToString());
                }

                userQuest.onQuestUpdated(new List<Quest>() { quest });

                if (groupDetail.pointEnabled)
                {
                    group.AddPoint(quest.Detail.point);
                }

                bool consumeQuestReward = !group.Detail.pointEnabled || !group.Detail.pointRewardOnly;
                if (consumeQuestReward && rewardMultiply > 0)
                    RewardConsumer.ConsumeReward(quest.Reward, rewardMultiply);

                bool groupCompleted = userQuest.UpdateGroupCompleted(group);
                if (groupCompleted && group.Detail.activeOnCompleteds.Count > 0)
                {
                    userQuest.ActiveNewGroups(group.Detail.activeOnCompleteds);
                }
                userQuest.dirty = true;

                // sync data nếu group chưa remove
                if (!group.removed)
                {
                    // cập nhật lại cả group nếu có quest mới actived
                    if (newActivedQuests.Count > 0)
                    {
                        userQuest.onGroupActiveNewQuests(group, newActivedQuests);
                    }

                    // chỉ cập nhật thông tin của group
                    if (groupDetail.pointEnabled || quest.Detail.removeOnClaimed)
                    {
                        userQuest.onGroupUpdated(group);
                    }

                    if (quest.Detail.removeOnClaimed)
                    {
                        userQuest.onQuestRemoved(quest);
                    }
                }
            }
        }

        private async void ActiveNewGroups(List<int> activeOnCompleteds)
        {
            if (activeOnCompleteds == null || activeOnCompleteds.Count == 0) return;

            List<Quest> loginQuests = new List<Quest>();

            // active quest groups
            foreach (int groupId in activeOnCompleteds)
            {
                var groupDetail = QuestAsset.Ins.FindGroup(groupId);
                if (!groupDetail)
                {
                    Debug.LogError(string.Format("Can not active quest group id {0} because group id not exists!", groupId));
                    continue;
                }

                var group = FindGroup(groupId);
                if (group != null)
                {
                    Debug.LogError(string.Format("Can not active quest group id {0} because group already active!", groupId));
                    continue;
                }

                // active a quest
                QuestGroup newGroup = CreateNewGroup(groupDetail);
                groups[groupId] = newGroup;
                UpdateNewGroup(newGroup);
                onGroupActived(newGroup);

                if (newGroup.IsRefreshOnTime)
                    await RefreshGroupByTime(newGroup);

                //Add login quest
                foreach (var quest in FindByGroup(groupId))
                {
                    if (quest.ActionRequire.action.id == ActionDefinition.LoginDay)
                        loginQuests.Add(quest);
                }
            }

            // Do Action version Login Quest
            if (loginQuests.Count >= 0)
            {
                DoAction(ActionDefinition.LoginDayAction(), 1, loginQuests);
            }
        }
        #endregion

        public Action<QuestGroup> onGroupActived = delegate { };
        public Action<QuestGroup> onGroupUpdated = delegate { };
        public Action<QuestGroup> onGroupRemoved = delegate { };
        public Action<QuestGroup> onGroupRefreshed = delegate { };
        public Action<QuestGroup, List<Quest>> onGroupActiveNewQuests = delegate { };
        public Action<Quest> onQuestRemoved = delegate { };
        public Action<List<Quest>> onQuestUpdated = delegate { };

        #region public methods
        /// <summary>
        /// Called every time user login in game
        /// </summary>
        public void UpdateLogin()
        {
            List<Quest> loginQuests = new List<Quest>();
            var refreshGroups = GroupDetails().FindAll(g => g.refreshConfig.type == QuestRefeshType.OnTime);
            foreach (QuestGroupDetail groupDetail in refreshGroups)
            {
                // only update local time from cached for exists quest group
                var groupId = groupDetail.id;
                var group = FindGroup(groupId);
                if (group != null)
                {
                    group.refreshTime.GetTime(UserData.UserId).ConfigureAwait(false);
                }
                else if (groupDetail.activeOnLoad)
                {
                    // active new group
                    group = CreateNewGroup(groupDetail);
                    groups[groupId] = group;
                    UpdateNewGroup(group);
                    onGroupActived(group);
                    UpdateRefreshTimeAndNotify(group);

                    //Add login quest
                    foreach (var quest in FindByGroup(groupId))
                    {
                        if (quest.ActionRequire.action.id == ActionDefinition.LoginDay)
                            loginQuests.Add(quest);
                    }
                }
            }

            // Do Action version Login Quest
            if (loginQuests.Count >= 0)
            {
                DoAction(ActionDefinition.LoginDayAction(), 1, loginQuests);
            }
        }

        /// <summary>
        /// Called every time user login in game on new day
        /// </summary>
        public async void UpdateNewDayLogin(Action onCompleted)
        {
            var refreshGroups = GroupDetails().FindAll(g => g.refreshConfig.type == QuestRefeshType.OnTime);
            foreach (QuestGroupDetail groupDetail in refreshGroups)
            {
                if (groupDetail.activeOnLoad || FindGroup(groupDetail.id) != null)
                    await RefreshGroupByTime(GetOrCreateGroup(groupDetail));
            }
            onCompleted();
        }

        /// <summary>
        /// Check group have any quest have reward can claim
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HaveReward(int groupId)
        {
            return FindByGroup(groupId).Find(quest => quest.CanClaim && !quest.Claimed) != null;
        }

        /// <summary>
        /// Check group have point reward can claim
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public bool HavePointReward(int groupId)
        {
            QuestGroup questGroup = FindGroup(groupId);

            if (questGroup == null || !questGroup.Detail.pointEnabled) return false;

            var keyReward = questGroup.Detail.pointRewards.Find(reward => reward.point <= questGroup.Point && !questGroup.ClaimedKeys.Contains(reward.point));
            return keyReward != null;
        }

        /// <summary>
        /// Find quests have require action
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        public List<Quest> FindByAction(int actionId) => datas.Values.FindAll(quest => quest.ContainsAction(actionId));
        #endregion

        #region private methods

        void UpdateQuestByStat(Quest quest, int statId)
        {
            int statValue = UserData.Stat(statId).AsInt;
            quest.UpdateProgress(statValue);
            quest.Notify();
            if (quest.StatRequire.Satify(statValue))
            {
                Notify(quest.GroupId.ToString());
            }
        }

        /// <summary>
        /// Refresh quests in a group with fresh type 'OnTime'
        /// </summary>
        /// <param name="group"></param>
        async Task RefreshGroupByTime(QuestGroup group)
        {
            // skip if group is marked as removed
            if (group.removed) return;

            DateTime nextRefreshTime = await group.refreshTime.GetTime(UserData.UserId);
            if (nextRefreshTime <= DateTime.Now)
            {
                UpdateNewGroup(group);
                UpdateRefreshTimeAndNotify(group);
            }
            else
            {
                Notify(group.Id.ToString());
            }
        }

        /// <summary>
        /// Update new refresh time and notify for group
        /// </summary>
        /// <param name="group"></param>
        async void UpdateRefreshTimeAndNotify(QuestGroup group)
        {
            // notify and callback that group is refreshed
            DirtyAndNotifyRefreshGroup(group);

            // update new refresh time
            await group.refreshTime.UpdateTime(UserData.UserId, group.Detail.refreshConfig.period);

            // due to time can be get from server, we notify again for next refresh time
            DirtyAndNotify(group.Id.ToString());
        }

        /// <summary>
        /// Update if all quests in group have completed
        /// </summary>
        /// <param name="group"></param>
        bool UpdateGroupCompleted(QuestGroup group)
        {
            var quests = FindByGroup(group.Id);
            group.completed = quests.Find(quest => !quest.Claimed) == null;
            if (group.completed)
            {
                onGroupUpdated(group);
                Notify(group.Id.ToString());
                if (group.IsRefreshOnCompleted)
                {
                    UpdateNewGroup(group);
                    DirtyAndNotifyRefreshGroup(group);
                }
                else if (group.RemoveOnCompleted)
                {
                    group.removed = true;
                    group.pointClaimeds.Clear();
                    RemoveByGroup(group.Id);
                    onGroupRemoved(group);
                }

                dirty = true;
            }
            Notify(group.Id.ToString());
            return group.completed;
        }

        /// <summary>
        /// Update group data when new group created or refreshed
        /// </summary>
        /// <param name="group"></param>
        void UpdateNewGroup(QuestGroup group)
        {
            LayerDetail layer = QuestAsset.Ins.GetActiveLayer();
            int layerId = layer ? layer.id : -1;
            group.Reset();
            group.layerId = layerId;
            RemoveByGroup(group.Id);
            CreateQuestsInGroup(group);
        }

        /// <summary>
        /// When a group created or refreshed, create available for that group
        /// </summary>
        /// <param name="group"></param>
        void CreateQuestsInGroup(QuestGroup group)
        {
            List<QuestDetail> questDetails = QuestAsset.Ins.FindByGroup(group.Detail.id).ToList();
            QuestRefreshConfig refreshConfig = group.Detail.refreshConfig;
            if (refreshConfig.Enabled)
            {
                if (group.Detail.type == QuestGroupType.List)
                {
                    if (refreshConfig.maxQuest > 0)
                    {
                        List<QuestDetail> showAlwayQuests = ListExt.RemoveAll(questDetails, quest => quest.showAlways);
                        List<Quest> quests = showAlwayQuests.Map(questDetail => CreateNew(questDetail));
                        while (quests.Count < refreshConfig.maxQuest && questDetails.Count > 0) quests.Add(CreateNew(questDetails.PopRandom()));
                        quests.ForEach(quest => Add(quest));
                    }
                    else
                    {
                        questDetails.ForEach(questDetail =>
                        {
                            if (CanCreateData(questDetail, group)) Add(CreateNew(questDetail));
                        });
                    }
                }
                else
                {
                    Dictionary<int, List<QuestDetail>> questMap = questDetails.ToListMap(questDetail => questDetail.slot);
                    List<Quest> quests = new List<Quest>();
                    foreach (var pair in questMap)
                    {
                        QuestDetail questDetail = pair.Value.Random();
                        quests.Add(CreateNew(questDetail));
                    }
                    quests.ForEach(quest => Add(quest));
                }
            }
            else
            {
                questDetails.ForEach(questDetail =>
                {
                    if (CanCreateData(questDetail, group)) Add(CreateNew(questDetail));
                });
            }
        }

        List<Quest> ActiveQuestInGroup(QuestGroup group, List<int> questIds)
        {
            var quests = new List<Quest>();
            foreach (int questId in questIds)
            {
                if (questId == -1) continue;
                var quest = CreateNew(FindDetail(questId));
                quests.Add(quest);
                Add(quest);
            }
            if (quests.Count > 0)
            {
                Notify("Refresh" + group.Id.ToString());
            }
            return quests;
        }

        void DirtyAndNotifyRefreshGroup(QuestGroup group)
        {
            string groupId = group.Id.ToString();
            DirtyAndNotify(groupId, "Refresh" + groupId);
            onGroupRefreshed(group);
        }
        #endregion

        #region remote override methods
        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {
        }
        #endregion
    }
}