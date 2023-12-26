namespace moonNest
{
    public class AchievementGroup : GroupObject<AchievementGroupDetail>
    {
        protected override AchievementGroupDetail GetDetail() => AchievementAsset.Ins.FindGroup(Id);

        public AchievementGroup(AchievementGroupDetail detail) : base(detail) { }

    }
}