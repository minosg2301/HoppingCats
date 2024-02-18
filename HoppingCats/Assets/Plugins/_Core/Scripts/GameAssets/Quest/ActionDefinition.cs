using System;

namespace moonNest
{
    [Serializable]
    public class ActionDefinition : BaseDefinition
    {
        public const int kLoginDay = 2004625552;
        public const int kUseCurrency = 1980997796;
        public const int kGainCurrency = 1980997797;
        public const int kOpenChest = 234417996;
        public const int kCompleteQuestInGroup = 245162442;
        public const int kCompleteTutorial = 552188841;

        public bool deletable = true;
        public ActionParamType[] paramTypes = new ActionParamType[3];
        public int[] enumTypes = new int[3];

        public ActionDefinition(string name) : base(name) { }

        internal static ActionData LoginDayAction() => new(kLoginDay);
        internal static ActionData UseCurrencyAction(int currencyId) => new(kUseCurrency, currencyId);
        internal static ActionData GainCurrencyAction(int currencyId) => new(kGainCurrency, currencyId);
        internal static ActionData OpenChestAction(int chestId) => new(kOpenChest, chestId);
        internal static ActionData CompleteQuestAction(int questGroupId) => new(kCompleteQuestInGroup, questGroupId);
        internal static ActionData CompleteTutorial(int tutorialId) => new(kCompleteTutorial, tutorialId);
    }

    public enum ActionParamType { None, Currency, Item, Chest, Quest, IntValue, Enum, QuestGroup, Tutorial }
}