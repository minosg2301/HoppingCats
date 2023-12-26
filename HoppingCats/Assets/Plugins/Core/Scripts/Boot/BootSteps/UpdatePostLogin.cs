using Doozy.Engine.UI.Settings;

namespace moonNest
{
    public class UpdatePostLogin : BootStep
    {
        public override string ToString() => "Update Post Login";

        public override void OnStep()
        {
            if(!UserData.MusicOn) Soundy.ToMute(Soundy.MusicParam, 0f);
            if(!UserData.SfxOn) Soundy.ToMute(Soundy.SfxParam, 0f);

            IAPManager.Ins.Init();

            var database = UIPopupSettings.Database;

            Complete();
        }
    }
}