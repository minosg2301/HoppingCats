using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using moonNest.remotedata;

namespace moonNest
{
    [Preserve]
    public class UserCurrency : RemotableUserData<FirestoreUserData>
    {
        public static UserCurrency Ins => LocalData.Get<UserCurrency>();

        [SerializeField] private Dictionary<int, Currency> idMaps = new Dictionary<int, Currency>();

        private readonly Dictionary<string, Currency> stringMaps = new Dictionary<string, Currency>();

        public event Action<Currency, UnitApplyType> OnCurrencyUsed = delegate { };
        public event Action<Currency, UnitApplyType> OnCurrencyAdded = delegate { };

        public List<Currency> Currencies => idMaps.Values.ToList();
        public List<Currency> GrowableCurrencies => idMaps.Values.FindAll(currency => currency.GrowOverTime);

        protected internal override void OnLoad()
        {
            GameDefinitionAsset.Ins.currencies.ForEach(_ =>
            {
                if (!idMaps.ContainsKey(_.id))
                {
                    idMaps.Add(_.id, new Currency(_));
                    dirty = true;
                }
            });
            Currencies.ForEach(currency => stringMaps[currency.Name] = currency);

            if (GlobalConfig.Ins.StoreRemoteData)
            {
                OnCurrencyUsed += OnCurrencyUpdated;
                OnCurrencyAdded += OnCurrencyUpdated;
            }
        }

        public void UpdateLogin()
        {
            Currencies.ForEach(currency => { if (currency.GrowOverTime) currency.UpdateLogin(); });
        }

        public static Currency Get(string name) => Ins.stringMaps.TryGetValue(name, out Currency currency) ? currency : null;
        public static Currency Get(int id) => Ins.idMaps.TryGetValue(id, out Currency currency) ? currency : null;

        private void OnCurrencyUpdated(Currency currency, UnitApplyType applyType)
        {
            RemoteData.AddRequest("Currencies", currency.Id.ToString(), currency.Value);
        }

        protected override void OnRemoteDataSync(FirestoreUserData remoteData)
        {
            if (RemoteData.Currencies == null)
            {
                OnRemoteDataCreated(remoteData);
                return;
            }

            RemoteData.Currencies.ForEach(pair =>
            {
                if (idMaps.TryGetValue(pair.Key, out Currency currency) && currency.Definition.sync)
                    currency.SetValue(pair.Value);
            });
        }

        protected override void OnRemoteDataCreated(FirestoreUserData remoteData)
        {
            RemoteData.Currencies = idMaps.Values.ToMap(v => v.Id, v => v.Value);
            RemoteData.AddRequests("Currencies", RemoteData.Currencies);
        }

        internal void NotifyCurrencyUsed(Currency currency, UnitApplyType applyType)
        {
            OnCurrencyUsed(currency, applyType);
        }

        internal void NotifCurrencyAdded(Currency currency, UnitApplyType applyType)
        {
            OnCurrencyAdded(currency, applyType);
        }
    }
}