namespace moonNest
{
    public class CollectChestKeyHandler : CollectHandler
    {
        void OnValidate()
        {
            if (type != CollectType.ChestKey) type = CollectType.ChestKey;
        }
    }
}