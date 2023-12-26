using System;

namespace moonNest
{
    [Serializable]
    public class ActionDetail
    {
        public int id = -1;
        public ActionParam[] actionParams = new ActionParam[3];

        public ActionDetail() { }
        public ActionDetail(ActionDefinition definition)
        {
            id = definition.id;
        }

#if UNITY_EDITOR
        [NonSerialized] private ActionDefinition _definition;
        public ActionDefinition Definition
        {
            get { if (!_definition) _definition = GameDefinitionAsset.Ins.FindAction(id); return _definition; }
            set { _definition = value; }
        }
#endif
    }

    [Serializable]
    public class ActionParam
    {
        public ActionParamType type;
        public int content;
        public string enumType;

        public bool Satify(int value)
        {
            if (type == ActionParamType.IntValue) return value >= content;
            return type == ActionParamType.None || this.content == -1 || value == -1 || value == this.content;
        }
    }
}