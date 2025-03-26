using UnityEngine;

public class ItemGenerator
{
    public static ItemConfig GenerateRandomItem()
    {
        return ItemConfigManager.Ins.randomItemConfigs.Random();
    }
}
