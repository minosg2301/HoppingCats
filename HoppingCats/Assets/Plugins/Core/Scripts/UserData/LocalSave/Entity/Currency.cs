using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class Currency
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private SafeDouble safeValue;
        [SerializeField] private double lastValue = 0;
        [SerializeField] private SynchronizableTime growTime;

        private bool updatingGrowValue = false;
        private int modifier;
        private double currentValue;

        public Currency(CurrencyDefinition definition)
        {
            name = definition.name;
            id = definition.id;
            lastValue = definition.initialValue;
            safeValue = new SafeDouble(lastValue);
            growTime = new SynchronizableTime(id);
        }

        private CurrencyDefinition _definition;
        public CurrencyDefinition Definition { get { if (!_definition) _definition = GameDefinitionAsset.Ins.FindCurrency(id); return _definition; } }

        public virtual double Value => safeValue.Value;
        public virtual double LastValue => lastValue;
        public virtual int Modifier => modifier;
        public double LastSeconds => growTime.LocalTime.Subtract(DateTime.Now).TotalSeconds;

        public int Id => id;
        public Sprite Icon => Definition.icon;
        public string Name => Definition.name;
        public double MaxValue => Definition.maxValue;
        public int GrowMaxValue => Definition.growMaxValue;
        public bool GrowOverTime => Definition.growOverTime;
        public bool NewValueChanged { get; set; }

        /// <summary>
        /// ---------- USE WITH CARE ----------<br/>
        /// Override currency value
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetValue(double value)
        {
            double newValue = Math.Max(0, Math.Min(value, Definition.maxValue));
            lastValue = newValue;
            safeValue.Value = newValue;
            currentValue = newValue;
            modifier = 0;
            DirtyAndNotify();
        }

        /// <summary>
        /// Modified currency by value
        /// </summary>
        /// <param name="value">
        ///     value < 0 : used to consume currency
        ///     value > 0 : used to add more currency
        ///     value = 0 : do nothing
        /// </param>
        /// <param name="forceSave"></param>
        /// <param name="applyType"></param>
        public virtual void AddValue(double value, bool forceSave = true, UnitApplyType applyType = UnitApplyType.Single)
        {
            if (value == 0) return;

            lastValue = safeValue.Value;
            double newValue = Math.Max(0, Math.Min(lastValue + value, Definition.maxValue));
            safeValue.Value = newValue;
            currentValue = newValue;
            NewValueChanged = true;
            modifier = (int)(newValue - lastValue);

            UpdateGrowTime();
            DirtyAndNotify();

            if (lastValue > newValue)
                UserCurrency.Ins.NotifyCurrencyUsed(this, applyType);
            else if (lastValue < newValue)
                UserCurrency.Ins.NotifCurrencyAdded(this, applyType);

            if (forceSave) UserCurrency.Ins.Save();
        }

        // get value by string
        public virtual string ValueString() => safeValue.Value.ToString();

        // get name
        public override string ToString() => name;

        // short hand for Subscribe/Notify
        public void Subscribe(IObserver observer, bool first = false) => UserCurrency.Ins.Subscribe(observer, first, name);
        public void Unsubscribe(IObserver observer) => UserCurrency.Ins?.Unsubscribe(observer);
        public void Subscribe(Action<BaseUserData> handler) => UserCurrency.Ins.Subscribe(name, handler);
        public void Unsubscribe(Action<BaseUserData> handler) => UserCurrency.Ins.Unsubscribe(name, handler);
        public void DirtyAndNotify() => UserCurrency.Ins.DirtyAndNotify(name);

        #region update grow
        async void UpdateGrowTime()
        {
            if (Definition.growOverTime)
            {
                if (lastValue >= GrowMaxValue && currentValue < GrowMaxValue)
                {
                    updatingGrowValue = true;
                    await growTime.UpdateTimeBySecond(UserData.UserId, Definition.growPeriod);
                    updatingGrowValue = false;
                    DirtyAndNotify();
                }
            }
        }

        internal void UpdateLogin()
        {
            if (growTime == null)
                growTime = new SynchronizableTime(id);

            growTime.GetTime(UserData.UserId).ConfigureAwait(false);
        }

        internal async void UpdateGrow()
        {
            if (updatingGrowValue)
                return;

            if (currentValue >= GrowMaxValue || DateTime.Compare(growTime.LocalTime, DateTime.Now) > 0)
                return;

            var periodInSeconds = Definition.growPeriod;
            updatingGrowValue = true;
            currentValue = Value;
            if (currentValue < GrowMaxValue)
            {
                var actualTime = await growTime.GetTime(UserData.UserId);
                var seconds = DateTime.Now.Subtract(actualTime).TotalSeconds;
                if (seconds > periodInSeconds)
                {
                    double secondOdds = seconds % periodInSeconds;
                    double addValue = Math.Floor(seconds / periodInSeconds);
                    SetValue(Math.Min(currentValue + addValue, GrowMaxValue));

                    if (currentValue < GrowMaxValue)
                        await growTime.UpdateTimeBySecond(UserData.UserId, (int)(secondOdds == 0 ? periodInSeconds : secondOdds));
                }
                else if (seconds > 0)
                {
                    SetValue(Math.Min(currentValue + 1, GrowMaxValue));

                    if (currentValue < GrowMaxValue)
                        await growTime.UpdateTimeBySecond(UserData.UserId, periodInSeconds);
                }
            }

            updatingGrowValue = false;
            DirtyAndNotify();
        }
        #endregion
    }
}