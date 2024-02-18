namespace moonNest
{
    public class CollectCurrencyHandler : CollectHandler
    {
        public UICurrency uiCurrency;

        void OnValidate()
        {
            if (type != CollectType.Currency) type = CollectType.Currency;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            uiCurrency.CollectHandlerListening = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            uiCurrency.CollectHandlerListening = false;
        }

        protected override void PlayAnim()
        {
            var request = CollectingManager.Ins.FindCurrency(uiCurrency.Currency.Id, andRemove: true);
            if (request != null)
            {
                uiCurrency.ShowLastValue();
                DoPlay(request);
            }
        }

        protected override void OnAnimDone(int count, float percent)
        {
            if (collectSfx) collectSfx.Play();

            uiCurrency.ShowLastValueWithPercent(percent);
            uiCurrency.Currency.NewValueChanged = false;
            UserCurrency.Ins.dirty = true;

            if (percent >= 1)
            {
                HandleCollectingEnd();
            }
        }
    }
}