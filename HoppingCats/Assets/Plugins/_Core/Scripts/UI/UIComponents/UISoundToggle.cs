namespace moonNest
{
    public class UISoundToggle : UIBaseToggle
    {
        void Reset()
        {
            gameObject.name = "UISoundToggle";
        }

        protected override bool Value
        {
            get { return UserData.SfxOn; }
            set
            {
                UserData.SfxOn = value;
            }
        }

        protected override void OnStateChanged(bool value)
        {
            if(value) Soundy.ToUnmute(Soundy.SfxParam);
            else Soundy.ToMute(Soundy.SfxParam);
        }
    }
}