using Doozy.Engine.UI;
using Doozy.Engine.UI.Settings;

namespace moonNest
{
    public class UpdatePostLogin : BootStep
    {
        public override string ToString() => "Update Post Login";

        public override void OnStep()
        {
            if (!UserData.MusicOn) Soundy.ToMute(Soundy.MusicParam, 0f);
            if (!UserData.SfxOn) Soundy.ToMute(Soundy.SfxParam, 0f);

            UIButton.OnUIButtonAction += OnButtonAction;

            IAPManager.Ins.Init();

            var database = UIPopupSettings.Database;

#if UNITY_ANDROID
            GameUpdator.Ins.CheckForUpdate();
#endif
            Complete();
        }

        void OnButtonAction(UIButton button, UIButtonBehaviorType type)
        {

        }
    }
}