using System;

namespace moonNest
{
    public class CollectItemHandler : CollectHandler
    {
        public ItemDefinitionId itemType;

        void OnValidate()
        {
            if (type != CollectType.Item) type = CollectType.Item;
        }

        protected override void PlayAnim()
        {
            var requests = CollectingManager.Ins.FindItems(itemType, andRemove: true);
            if (requests != null)
            {
                DoPlay(requests);
            }
        }
    }
}