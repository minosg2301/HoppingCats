namespace moonNest
{
    public class UIMusicToggle : UIBaseToggle
    {
        void Reset()
        {
            gameObject.name = "UIMusicToggle";
        }

        protected override bool Value
        {
            get { return UserData.MusicOn; }
            set
            {
                UserData.MusicOn = value;
            }
        }

        protected override void OnStateChanged(bool value)
        {
            if(value) Soundy.ToUnmute(Soundy.MusicParam);
            else Soundy.ToMute(Soundy.MusicParam);
        }
    }
}