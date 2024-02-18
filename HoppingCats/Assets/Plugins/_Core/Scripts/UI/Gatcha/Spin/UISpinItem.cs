namespace moonNest
{
    public class UISpinItem : BaseUIData<SpinItemConfig>
    {
        public UISpin spinController;
        public UIRewardDetail reward;

        public SpinItemConfig Config { get; set; }

        void Reset()
        {
            if(!spinController) spinController = GetComponentInParent<UISpin>();
        }

        void OnValidate()
        {
            gameObject.name = "UISpinItem";
        }

        public override void SetData(SpinItemConfig config)
        {
            Config = config;
            reward.SetData(config.reward);
        }
    }
}