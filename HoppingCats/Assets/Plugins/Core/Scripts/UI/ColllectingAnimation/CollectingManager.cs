using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace moonNest
{
    public class CollectingManager : SingletonMono<CollectingManager>, IObservable
    {
        #region fields
        private readonly Dictionary<CollectType, List<CollectRequest>> requests = new Dictionary<CollectType, List<CollectRequest>>();
        private readonly List<bool> allowFlags = new List<bool>();
        private readonly List<int> ignoreCurrencies = new List<int>();
        #endregion

        #region properties
        public bool AllowPopupCollect => allowFlags.Count != 0 && allowFlags.Last();
        public bool AllowCollect { get; set; }
        #endregion

        #region methods

        public void Init()
        {
            UserCurrency.Ins.OnCurrencyAdded += OnCurrencyAdded;
            UserInventory.Ins.onItemAdded += OnItemAdded;
            UserInventory.Ins.onChestKeyAdded += OnChestKeyAdded;
        }

        void OnItemAdded(Item item, int amount, UnitApplyType applyType)
        {
            AddRequest(new CollectItem(item.Definition.id, amount, item.Detail.icon, item.Detail.UIPrefab));
        }

        void OnCurrencyAdded(Currency currency, UnitApplyType applyType)
        {
            if (currency.Modifier <= 0 || ignoreCurrencies.Contains(currency.Id))
                return;

            AddRequest(new CollectCurrency(currency, currency.Modifier));
        }

        void OnChestKeyAdded(ChestKey chestKey, int amount, UnitApplyType applyType)
        {
            AddRequest(new CollectRequest(CollectType.ChestKey, amount, chestKey.ChestDetail.keyIcon));
        }

        /// <summary>
        /// Find all requests of collect type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal List<CollectRequest> Find(CollectType type, bool andRemove = false)
        {
            if (requests.TryGetValue(type, out var list))
            {
                if (andRemove) requests.Remove(type);
                return list;
            }
            return null;
        }

        /// <summary>
        /// Find collect request of currency
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        internal CollectCurrency FindCurrency(CurrencyId currencyId, bool andRemove = false)
        {
            if (requests.TryGetValue(CollectType.Currency, out var list))
            {
                var collectCurrency = list.Find(r => (r as CollectCurrency).currencyId == currencyId) as CollectCurrency;
                if (andRemove && collectCurrency != null) list.Remove(collectCurrency);
                return collectCurrency;
            }
            return null;
        }

        /// <summary>
        /// Remove collect request of currency
        /// </summary>
        /// <param name="currencyId"></param>
        internal void RemoveCurrency(CurrencyId currencyId)
        {
            if (requests.TryGetValue(CollectType.Currency, out var list))
                list.Remove(request => (request as CollectCurrency).currencyId == currencyId);
        }

        internal List<CollectItem> FindItems(ItemDefinitionId definitionId, bool andRemove = false)
        {
            if (requests.TryGetValue(CollectType.Item, out var list))
            {
                var collectItems = list.FindAll(r => (r as CollectItem).definitionId == definitionId).Map(r => r as CollectItem);
                if (andRemove && collectItems.Count > 0) collectItems.ForEach(c => list.Remove(c));
                return collectItems;
            }
            return null;
        }

        internal void RemoveItems(ItemDefinitionId definitionId)
        {
            if (requests.TryGetValue(CollectType.Item, out var list))
                list.RemoveAll(request => (request as CollectItem).definitionId == definitionId);
        }

        /// <summary>
        /// Find collect request of currency
        /// </summary>
        /// <param name="currencyId"></param>
        /// <returns></returns>
        internal CollectStat FindStat(int statId, bool andRemove = false)
        {
            if (requests.TryGetValue(CollectType.Stat, out var list))
            {
                var collectStat = list.Find(r => (r as CollectStat).statId == statId) as CollectStat;
                if (andRemove && collectStat != null) list.Remove(collectStat);
                return collectStat;
            }
            return null;
        }

        /// <summary>
        /// Remove collect request of stat
        /// </summary>
        /// <param name="statId"></param>
        internal void RemoveStat(int statId)
        {
            if (requests.TryGetValue(CollectType.Stat, out var list))
                list.RemoveAll(request => (request as CollectStat).statId == statId);
        }

        /// <summary>
        /// Add collect request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="notify">should notify or not</param>
        internal void AddRequest(CollectRequest request, bool notify = true)
        {
            requests.GetOrCreate(request.type, (t) => new List<CollectRequest>()).Add(request);
            if (notify) Notify(request.type);
        }

        /// <summary>
        /// Add collect request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="notify">should notify or not</param>
        internal void AddRequest(CollectCurrency request, bool notify = true)
        {
            var list = requests.GetOrCreate(request.type, (t) => new List<CollectRequest>());
            var currencyRequest = list.Find(r => (r as CollectCurrency).currencyId == request.currencyId);
            if (currencyRequest != null) currencyRequest.amount += request.amount;
            else list.Add(request);
            if (notify) Notify(request.type);
        }

        /// <summary>
        /// Helper function that add a collect request by consumed reward
        /// </summary>
        /// <param name="reward"></param>
        internal void AddRequest(ConsumedReward reward)
        {
            if (reward.Type == RewardType.Chest)
            {
                requests
                .GetOrCreate(CollectType.ChestKey, (t) => new List<CollectRequest>())
                .Add(new CollectRequest(CollectType.ChestKey, reward.Amount, reward.Icon));
            }

        }
        #endregion

        #region public methods
        /// <summary>
        /// Push an allowing flag
        /// </summary>
        /// <param name="allow"></param>
        public static void PushAllowPopup() => Ins.allowFlags.Add(true);

        /// <summary>
        /// Pop an allowing flag
        /// </summary>
        public static void PopAllowPopup() => Ins.allowFlags.Pop();

        /// <summary>
        /// Add currency will be ignored
        /// </summary>
        public void AddIgnoredCurrency(int currencyId)
        {
            if (!ignoreCurrencies.Contains(currencyId))
                ignoreCurrencies.Add(currencyId);
        }

        /// <summary>
        /// Remove currency from ignored list
        /// </summary>
        public void RemoveIgnoredCurrency(int currencyId)
        {
            ignoreCurrencies.Remove(currencyId);
        }

        #endregion

        #region observer
        private ObserverProvider<CollectingManager> provider = new ObserverProvider<CollectingManager>();

        public void NotifyAll() => Enum.GetNames(typeof(CollectType)).ForEach(name => provider.Notify(this, name));

        internal void Notify(CollectType type) => provider.Notify(this, type.ToString());

        internal void Subcribe(IObserver observer, CollectType type)
        {
            provider.Subscribe(observer, type.ToString());
            observer.OnNotify(this, type.ToString());
        }

        internal void Unsubcribe(IObserver observer) => provider.Unsubscribe(observer);
        #endregion
    }

    public enum CollectType { Currency, Item, ChestKey, Stat }

    public class CollectRequest
    {
        public CollectType type;
        public int amount;
        public Sprite icon;
        public GameObject prefab;

        public CollectRequest(CollectType type, int amount, Sprite icon)
        {
            this.type = type;
            this.amount = amount;
            this.icon = icon;
        }

        public CollectRequest(CollectType type, int amount, Sprite icon, GameObject prefab)
        {
            this.type = type;
            this.amount = amount;
            this.icon = icon;
            this.prefab = prefab;
        }
    }

    public class CollectCurrency : CollectRequest
    {
        public CurrencyId currencyId;

        public CollectCurrency(Currency currency, int amount)
            : base(CollectType.Currency, amount, currency.Definition.icon)
        {
            currencyId = currency.Id;
        }
    }

    public class CollectItem : CollectRequest
    {
        public ItemDefinitionId definitionId;

        public CollectItem(ItemDefinitionId itemDefinition, int amount, Sprite icon, GameObject prefab)
            : base(CollectType.Item, amount, icon, prefab)
        {
            this.definitionId = itemDefinition;
        }
    }

    public class CollectStat : CollectRequest
    {
        public int statId;

        public CollectStat(int statId, int amount)
            : base(CollectType.Stat, amount, UserPropertyAsset.Ins.FindStat(statId).displayIcon)
        {
            this.statId = statId;
        }
    }
}