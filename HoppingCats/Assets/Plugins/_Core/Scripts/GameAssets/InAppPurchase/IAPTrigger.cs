using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class IAPTrigger : BaseData
    {
        public int locationId = -1;
        public int frequences = 1;
        public int sessionEvent = 1;
        public TriggerType triggerType;
        public bool userTargeted = false;
        public List<IAPPackageId> nonTargetedPackageIds = new List<IAPPackageId>();
        public List<IAPTriggerTargetedContent> targetedContents = new List<IAPTriggerTargetedContent>();
        public List<IAPTriggerCondition> conditions = new List<IAPTriggerCondition>();
        public List<int> actions = new List<int>();

        public IAPTrigger(string name) : base(name) { }
    }

    [Serializable]
    public class IAPTriggerTargetedContent : Cloneable
    {
        public string name = "";        
        public List<ComparableValueInt> userStats = new List<ComparableValueInt>();
        public List<IAPPackageId> contents = new List<IAPPackageId>();

        public IAPTriggerTargetedContent(string name)
        {
            this.name = name;
        }
    }

    [Serializable]
    public class IAPTriggerCondition : ComparableValueInt
    {
        public Scope scope;
    }

    public enum TriggerType { AllConditionsMatched, AnyConditionMatched }
    public enum Scope { UserStat, UserCurrency }
}