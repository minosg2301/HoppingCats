using System;

namespace moonNest
{
    [Serializable]
    public class ActionDefinition : BaseDefinition
    {
        public const int LoginDay = 2004625552;
        public const int UseCurrency = 1980997796;
        public const int OpenChest = 234417996;
        public const int CompleteQuestInGroup = 245162442;

        public bool deletable = true;
        public ActionParamType[] paramTypes = new ActionParamType[3];
        public int[] enumTypes = new int[3];

        public ActionDefinition(string name) : base(name) { }

        internal static ActionData LoginDayAction() => new ActionData(LoginDay);
        internal static ActionData UseCurrencyAction(int currencyId) => new ActionData(UseCurrency, currencyId);
        internal static ActionData OpenChestAction(int chestId) => new ActionData(OpenChest, chestId);
        internal static ActionData CompleteQuestAction(int questGroupId) => new ActionData(CompleteQuestInGroup, questGroupId);
    }

    public enum ActionParamType { None, Currency, Item, Chest, Quest, IntValue, Enum, QuestGroup }
}