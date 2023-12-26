using System;
using UnityEngine;

namespace moonNest
{
    public enum CurrencyType { Hard, Soft }
    public enum NumericType { Number, BigNumber }

    [Serializable]
    public class CurrencyDefinition : BaseDefinition
    {
        public CurrencyType type;
        public NumericType numericType;
        public Sprite icon;
        public Sprite bigIcon;
        public int initialValue = 0;
        public double maxValue = 99999999;
        public string description = "";

        public bool growOverTime = false;
        public int growValue = 1;
        public int growMaxValue = 20;
        public int growPeriod = 600;
        public bool sync = true;

        public CurrencyDefinition(string name) : base(name) { }
    }
}