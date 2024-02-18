namespace moonNest
{
    public class UIHapticToggle : UIBaseToggle
    {
        protected override bool Value
        {
            get { return UserData.HapticOn; }
            set
            {
                UserData.HapticOn = value;
            }
        }

        protected override void OnStateChanged(bool value) { }
    }
}