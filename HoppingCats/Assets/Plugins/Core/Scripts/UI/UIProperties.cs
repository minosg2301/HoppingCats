using System;

namespace moonNest
{
    [Serializable]
    public struct CurrencyId
    {
        public int id;

        public CurrencyId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(CurrencyId value) => value.id;
        public static implicit operator CurrencyId(int id) => new CurrencyId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct ItemDefinitionId
    {
        public int id;

        public ItemDefinitionId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(ItemDefinitionId value) => value.id;
        public static implicit operator ItemDefinitionId(int id) => new ItemDefinitionId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct ItemId
    {
        public int definitionId;
        public int id;

        public ItemId(int itemId)
        {
            definitionId = -1;
            id = itemId;
        }

        public static implicit operator int(ItemId value) => value.id;
        public static implicit operator ItemId(int id) => new ItemId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct QuestGroupId
    {
        public int groupId;

        public QuestGroupId(int groupId)
        {
            this.groupId = groupId;
        }

        public static implicit operator int(QuestGroupId value) => value.groupId;
        public static implicit operator QuestGroupId(int id) => new QuestGroupId(id);

        public override string ToString() => groupId.ToString();
    }

    [Serializable]
    public struct QuestId
    {
        public int groupId;
        public int id;

        public QuestId(int questId)
        {
            groupId = -1;
            id = questId;
        }

        public static implicit operator int(QuestId value) => value.id;
        public static implicit operator QuestId(int id) => new QuestId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct ActionId
    {
        public int id;
        public ActionId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(ActionId value) => value.id;
        public static implicit operator ActionId(int id) => new ActionId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct AchievementGroupId
    {
        public int groupId;

        public AchievementGroupId(int groupId)
        {
            this.groupId = groupId;
        }

        public static implicit operator int(AchievementGroupId value) => value.groupId;
        public static implicit operator AchievementGroupId(int id) => new AchievementGroupId(id);

        public override string ToString() => groupId.ToString();
    }

    [Serializable]
    public struct AchievementId
    {
        public int groupId;
        public int id;

        public AchievementId(int achievementId)
        {
            groupId = -1;
            id = achievementId;
        }

        public static implicit operator int(AchievementId value) => value.id;
        public static implicit operator AchievementId(int id) => new AchievementId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct ShopId
    {
        public int id;

        public ShopId(int shopId)
        {
            id = shopId;
        }

        public static implicit operator int(ShopId value) => value.id;
        public static implicit operator ShopId(int id) => new ShopId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct ChestId
    {
        public int id;

        public ChestId(int chestId)
        {
            id = chestId;
        }

        public static implicit operator int(ChestId value) => value.id;
        public static implicit operator ChestId(int id) => new ChestId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public class IAPPackageId
    {
        public int groupId;
        public int id;

        public IAPPackageId(int id)
        {
            groupId = -1;
            this.id = id;
        }

        public static implicit operator int(IAPPackageId value) => value.id;
        public static implicit operator IAPPackageId(int id) => new IAPPackageId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct IAPGroupId
    {
        public int id;

        public IAPGroupId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(IAPGroupId value) => value.id;
        public static implicit operator IAPGroupId(int id) => new IAPGroupId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct LocationId
    {
        public int id;

        public LocationId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(LocationId value) => value.id;
        public static implicit operator LocationId(int id) => new LocationId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct NavigationId
    {
        public int id;

        public NavigationId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(NavigationId value) => value.id;
        public static implicit operator NavigationId(int id) => new NavigationId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct NavigationPathId
    {
        public int id;

        public NavigationPathId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(NavigationPathId value) => value.id;
        public static implicit operator NavigationPathId(int id) => new NavigationPathId(id);

        public override string ToString() => id.ToString();
    }



    [Serializable]
    public struct UserStatId
    {
        public int id;

        public UserStatId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(UserStatId userStatId) => userStatId.id;
        public static implicit operator UserStatId(int id) => new UserStatId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct UserAttributeId
    {
        public int id;

        public UserAttributeId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(UserAttributeId attribute) => attribute.id;
        public static implicit operator UserAttributeId(int id) => new UserAttributeId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct SpinId
    {
        public int id;

        public SpinId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(SpinId value) => value.id;
        public static implicit operator SpinId(int id) => new SpinId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct SpinItemId
    {
        public int spinId;
        public int id;

        public SpinItemId(int id)
        {
            spinId = -1;
            this.id = id;
        }

        public static implicit operator int(SpinItemId value) => value.id;
        public static implicit operator SpinItemId(int id) => new SpinItemId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct TutorialId
    {
        public int id;

        public TutorialId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(TutorialId value) => value.id;
        public static implicit operator TutorialId(int id) => new TutorialId(id);

        public override string ToString() => id.ToString();
    }

    [Serializable]
    public struct TutorialStepId
    {
        public int tutorialId;
        public int stepId;

        public TutorialStepId(int id)
        {
            tutorialId = -1;
            this.stepId = id;
        }

        public static implicit operator int(TutorialStepId value) => value.stepId;
        public static implicit operator TutorialStepId(int id) => new TutorialStepId(id);

        public override string ToString() => stepId.ToString();
    }

    [Serializable]
    public struct FeatureId
    {
        public int id;

        public FeatureId(int id)
        {
            this.id = id;
        }

        public static implicit operator int(FeatureId value) => value.id;
        public static implicit operator FeatureId(int id) => new FeatureId(id);

        public override string ToString() => id.ToString();
    }
}