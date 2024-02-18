using System;

namespace moonNest
{
    [Serializable]
    public class UserPropertyDefinition : BaseObjectDefinition
    {
        public const string kVip = "Vip";
        public const string kName = "Name";
        public const string kUserId = "UserId";
        public const string kLanguage = "Language";

        public bool Init()
        {
            if(stats.Count == 0)
            {
                stats.Add(new StatDefinition(kVip) { displayName = kVip, type = StatValueType.Int, initValue = 0, deletable = false, safe = true });

                attributes.Add(new AttributeDefinition(kUserId, AttributeType.String) { displayName = kUserId, deletable = false, sync = false });
                attributes.Add(new AttributeDefinition(kName, AttributeType.String) { displayName = kName, deletable = false });
                attributes.Add(new AttributeDefinition(kLanguage, AttributeType.String) { displayName = kLanguage, deletable = false, initValue = "English" });

                DoDeserialize();

                return true;
            }
            return false;
        }
    }
}