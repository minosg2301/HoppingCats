using System;
using System.Collections.Generic;
using UnityEngine;

namespace moonNest
{
    public class GameDefinitionAsset : SingletonScriptObject<GameDefinitionAsset>
    {
        public List<FeatureConfig> features = new List<FeatureConfig>();
        public List<CurrencyDefinition> currencies = new List<CurrencyDefinition>();
        public List<CurrencyExchange> currencyExchanges = new List<CurrencyExchange>();
        public List<ActionDefinition> actions = new List<ActionDefinition>();
        public List<EnumDefinition> enums = new List<EnumDefinition>();
        public List<LocationDefinition> locations = new List<LocationDefinition>();
        public List<GameEvent> events = new List<GameEvent>();
        public List<string> categories = new List<string>();

        /// <summary>
        /// Invoke when asset loaded
        /// </summary>
        void Init()
        {
            if(features.Count == 0)
            {
                features = new List<FeatureConfig>() {
                FeatureConfig.OnlineReward,
                FeatureConfig.Achievement,
                FeatureConfig.Arena,
                FeatureConfig.Gatcha };
            }

            if(currencies.Count == 0)
            {
                currencies.Add(new CurrencyDefinition("Cash") { type = CurrencyType.Hard, numericType = NumericType.Number });
                currencies.Add(new CurrencyDefinition("Coin") { type = CurrencyType.Soft, numericType = NumericType.Number });
            }

            if(actions.Count == 0)
            {
                actions.Add(new ActionDefinition("Login Day") { id = ActionDefinition.LoginDay, deletable = false });

                ActionDefinition useCurrency = new ActionDefinition("Use Currency") { id = ActionDefinition.UseCurrency, deletable = false };
                useCurrency.paramTypes[0] = ActionParamType.Currency;
                actions.Add(useCurrency);

                ActionDefinition openChest = new ActionDefinition("Open Chest") { id = ActionDefinition.OpenChest, deletable = false };
                openChest.paramTypes[0] = ActionParamType.Chest;
                actions.Add(openChest);

                ActionDefinition completeQuestInGroup = new ActionDefinition("Complete Quest In Group") { id = ActionDefinition.CompleteQuestInGroup, deletable = false };
                completeQuestInGroup.paramTypes[0] = ActionParamType.QuestGroup;
                actions.Add(completeQuestInGroup);
            }
        }

        public double GetExchangeValue(int srcCurrency, int destCurrency, double valueGet)
        {
            var exchangeConfig = currencyExchanges.Find(exchange => exchange.srcCurrency == srcCurrency && exchange.destCurrency == destCurrency);
            if(exchangeConfig != null) return Mathf.CeilToInt(((float)(exchangeConfig.srcValue * valueGet)) / exchangeConfig.destValue);
            return valueGet;
        }

        public CurrencyDefinition FindCurrency(int id) => currencies.Find(_ => _.id == id);
        public CurrencyDefinition FindCurrencyByName(string name) => currencies.Find(_ => _.name == name);
        public List<CurrencyDefinition> FindCurrenciesByType(CurrencyType type) => currencies.FindAll(_ => _.type == type);
        public ActionDefinition FindAction(int id) => actions.Find(_ => _.id == id);
        public EnumDefinition FindEnum(int id) => enums.Find(_ => _.id == id);
        public FeatureConfig FindFeature(int featureId) => features.Find(_ => _.id == featureId);
        public FeatureConfig FindFeature(string featureName) => features.Find(_ => _.name == featureName);
        public GameEvent FindGameEvent(int id) => events.Find(_ => _.id == id);
    }
}