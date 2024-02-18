using System;
using System.Collections.Generic;

namespace moonNest
{
    [Serializable]
    public class ActionRequire
    {
        public ActionDetail action;
        public int count = 1;

        public bool Satify(ActionData actionData)
        {
            return action.actionParams[0].Satify(actionData.arg1)
                && action.actionParams[1].Satify(actionData.arg2)
                && action.actionParams[2].Satify(actionData.arg3);
        }
    }

    [Serializable]
    public class ActionRequires
    {
        public List<ActionDetail> actions = new List<ActionDetail>();
        public int count = 1;

        public bool Satify(ActionData actionData)
        {
            foreach (var action in actions)
            {
                if (action.actionParams[0].Satify(actionData.arg1)
                && action.actionParams[1].Satify(actionData.arg2)
                && action.actionParams[2].Satify(actionData.arg3))
                    return true;
            }
            return false;
        }
    }

    [Serializable]
    public class StatRequireDetail
    {
        public int statId = -1;
        public long value = 1;

#if UNITY_EDITOR
        [NonSerialized] private StatDefinition _definition;
        public StatDefinition Definition
        {
            get { if (!_definition) _definition = UserPropertyAsset.Ins.properties.FindStat(statId); return _definition; }
            set { _definition = value; }
        }
#endif

        public bool Satify(long n) => n >= value;
    }
}