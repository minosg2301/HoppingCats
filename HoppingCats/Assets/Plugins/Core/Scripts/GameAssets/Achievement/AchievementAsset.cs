using System.Collections.Generic;

namespace moonNest
{
    public class AchievementAsset : BaseGroupAsset<AchievementAsset, AchievementDetail, AchievementGroupDetail>
    {
        public List<AchievementDetail> Achievements => Datas;

        protected override int GetGroupId(AchievementDetail data) => data.groupId;
    }
}