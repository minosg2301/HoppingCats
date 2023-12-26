using System;
using System.Collections.Generic;
using System.Linq;

namespace moonNest
{
    public class UserAchievement : BaseUserDataGroup<Achievement, AchievementDetail, AchievementGroup, AchievementGroupDetail>
    {
        public static UserAchievement Ins => LocalData.Get<UserAchievement>();

        #region override method
        protected override Achievement CreateNew(AchievementDetail detail) => new Achievement(detail);
        protected override AchievementDetail FindDetail(int detailId) => AchievementAsset.Ins.Find(detailId);
        protected override List<AchievementDetail> FindDetailsByGroup(int groupId) => AchievementAsset.Ins.FindByGroup(groupId);

        protected override List<AchievementGroupDetail> GroupDetails() => AchievementAsset.Ins.Groups;
        protected override AchievementGroupDetail FindGroupDetail(AchievementDetail detail) => AchievementAsset.Ins.FindGroup(detail.groupId);
        protected override AchievementGroupDetail FindGroupDetail(int groupId) => AchievementAsset.Ins.FindGroup(groupId);
        protected override AchievementGroup CreateNewGroup(AchievementGroupDetail groupDetail) => new AchievementGroup(groupDetail);
        #endregion

        public Action<Achievement> onAchievementAdded;
        public Action<Achievement> onAchievementRemoved;
        public Action<Achievement, ConsumedRewards> onAchievementClaimed;

        public bool HaveAchievementReward(int achievementGroupId)
        {
            var achievements = FindByGroup(achievementGroupId);
            return achievements.Find(achievement => achievement.CanClaim && !achievement.Claimed) != null;
        }

        public void Claim(Achievement achievement)
        {
            if(achievement != null && achievement.CanClaim)
            {
                achievement.Claimed = true;

                ConsumedRewards rewardResults = RewardConsumer.ConsumeReward(achievement.Detail.reward);
                onAchievementClaimed?.Invoke(achievement, rewardResults);

                if(achievement.Detail.removeOnClaimed && achievement.Claimed)
                {
                    Remove(achievement);
                    onAchievementRemoved?.Invoke(achievement);
                }

                achievement.Notify();
                Notify(achievement.GroupId.ToString());
                dirty = true;
            }
        }
    }
}