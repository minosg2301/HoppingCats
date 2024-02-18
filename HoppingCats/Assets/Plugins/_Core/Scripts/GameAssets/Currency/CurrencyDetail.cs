using System;

namespace moonNest
{
    [Serializable]
    public class CurrencyDetail : BaseDetail<CurrencyDefinition>
    {
        public int value;

        public CurrencyDetail(CurrencyDefinition definition) : base(definition)
        {
        }

        protected override CurrencyDefinition GetDefinition(int definitionId) => GameDefinitionAsset.Ins.FindCurrency(definitionId);
    }
}