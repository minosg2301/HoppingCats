using System;
using System.Collections.Generic;
using System.Linq;

namespace moonNest
{
    internal class IAPTriggerData
    {
        internal int progress = 0;
        internal int eventCount = 0;

        public IAPTrigger Trigger { get; }
        public IAPTriggerData(IAPTrigger trigger)
        {
            Trigger = trigger;
        }

        public bool Activable => progress >= Trigger.frequences && eventCount < Trigger.sessionEvent;

        public override string ToString() => Trigger.name;
    }

    internal class IAPTriggerController : SingletonMono<IAPTriggerController>
    {
        readonly Dictionary<int, List<IAPTriggerData>> actionTriggerDatas = new Dictionary<int, List<IAPTriggerData>>();
        readonly Dictionary<int, List<IAPTriggerData>> activableTriggers = new Dictionary<int, List<IAPTriggerData>>();
        readonly List<IAPTriggerData> triggerDatas = new List<IAPTriggerData>();
        readonly List<IAPTriggerListener> listeners = new List<IAPTriggerListener>();

        internal void Init()
        {
            foreach (var triggerConfig in IAPPackageAsset.Ins.triggers)
            {
                var triggerData = new IAPTriggerData(triggerConfig);
                triggerDatas.Add(triggerData);
                foreach (int actionId in triggerConfig.actions)
                {
                    actionTriggerDatas
                        .GetOrCreate(actionId, () => new List<IAPTriggerData>())
                        .Add(triggerData);
                }
            }
        }

        /// <summary>
        /// Handle User Action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="value"></param>
        internal void OnUserAction(ActionData action, long value)
        {
            if (actionTriggerDatas.TryGetValue(action.id, out var triggers))
            {
                foreach (var triggerData in triggers)
                {
                    triggerData.progress++;
                    if (triggerData.Activable)
                        AddToActive(triggerData);
                }
            }
        }

        /// <summary>
        /// Handle a location which can perform triggers
        /// </summary>
        /// <param name="locationHandler"></param>
        internal void OnEnterLocation(LocationId location)
        {
            var actives = GetActives(location).ToList();

            foreach (var activeTrigger in actives)
            {
                // check user data conditions
                if (!SatifyUserTargeted(activeTrigger))
                    continue;

                var config = activeTrigger.Trigger;
                var packageIds = config.nonTargetedPackageIds;

                if (config.userTargeted && config.targetedContents.Count > 0)
                {
                    packageIds = FindPackageIdsByUserTargeted(config.targetedContents);
                }

                if (packageIds.Count > 0)
                {
                    IAPPackageId packageId = packageIds.Random();

                    foreach (var listener in listeners)
                        listener.OnIAPTrigger(packageId);

                    activeTrigger.progress = 0;
                    activeTrigger.eventCount++;
                    RemoveFromActive(activeTrigger);
                }
            }
        }

        List<IAPTriggerData> GetActives(int locationId)
        {
            return activableTriggers.GetOrCreate(locationId, () => new List<IAPTriggerData>());
        }

        void AddToActive(IAPTriggerData triggerData)
        {
            var list = activableTriggers.GetOrCreate(triggerData.Trigger.locationId, () => new List<IAPTriggerData>());
            if (!list.Contains(triggerData))
                list.Add(triggerData);
        }

        void RemoveFromActive(IAPTriggerData triggerData)
        {
            var list = activableTriggers.GetOrCreate(triggerData.Trigger.locationId, () => new List<IAPTriggerData>());
            list.Remove(triggerData);
        }

        List<IAPPackageId> FindPackageIdsByUserTargeted(List<IAPTriggerTargetedContent> targetedContents)
        {
            foreach (var targetedContent in targetedContents)
            {
                bool qualify = true;
                foreach (var userStat in targetedContent.userStats)
                {
                    qualify &= userStat.Compare(UserData.Stat(userStat.contentId).AsInt);
                    if (!qualify) break;
                }

                if (qualify)
                {
                    return targetedContent.contents;
                }
            }
            return new List<IAPPackageId>();
        }

        bool SatifyUserTargeted(IAPTriggerData activeTrigger)
        {
            var config = activeTrigger.Trigger;
            if (config.triggerType == TriggerType.AllConditionsMatched)
            {
                bool satify = true;
                foreach (var condition in config.conditions)
                {
                    satify &= IsSatify(condition);
                    if (!satify) return false;
                }
                return satify;
            }
            else
            {
                foreach (var condition in config.conditions)
                {
                    if (IsSatify(condition))
                        return true;
                }
                return config.conditions.Count == 0;
            }
        }

        bool IsSatify(IAPTriggerCondition condition)
        {
            switch (condition.scope)
            {
                case Scope.UserStat: return condition.Compare(UserData.Stat(condition.contentId).AsInt);
                case Scope.UserCurrency: return condition.Compare((int)UserCurrency.Get(condition.contentId).Value);
            }
            return false;
        }

        internal void RegisterListener(IAPTriggerListener listener)
        {
            if (!listeners.Contains(listener))
                listeners.Add(listener);
        }

        internal void UnregisterListener(IAPTriggerListener listener)
        {
            listeners.Remove(listener);
        }
    }
}