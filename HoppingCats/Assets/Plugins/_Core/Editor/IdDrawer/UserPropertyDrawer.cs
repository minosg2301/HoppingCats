using System.Collections.Generic;
using UnityEditor;
using moonNest;

[CustomPropertyDrawer(typeof(UserStatId))]
public class UserStatIdDrawer : BasePropertyDrawer
{
    public static List<StatDefinition> stats = new List<StatDefinition>();

    public override void DoDrawProperty()
    {
        UpdateKeys();
        rect = PrefixLabel(rect, "User Stat");
        DrawIntPopup(rect, "id", stats, "name", "id");
    }

    private static void UpdateKeys()
    {
        if(stats.Count != UserPropertyAsset.Ins.properties.stats.Count)
        {
            stats = UserPropertyAsset.Ins.properties.stats.FindAll(stat => stat.type == StatValueType.Int);
        }
    }
}

[CustomPropertyDrawer(typeof(UserAttributeId))]
public class UserAttributeIdDrawer : BasePropertyDrawer
{
    public override void DoDrawProperty()
    {
        rect = PrefixLabel(rect, "User Attribute");
        DrawIntPopup(rect, "id", UserPropertyAsset.Ins.properties.attributes, "name", "id");
    }
}