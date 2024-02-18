using UnityEngine.Events;

namespace moonNest
{
    public class AuthenticateUser : BootStep
    {
        public UnityEvent requireAuthEvent;

        public override string ToString() => "Authenticate User";

        public override void OnStep()
        {
            if (!GlobalConfig.Ins.AuthenticateUser)
            {
                CreateOfflineUser();
                Complete();
                return;
            }
        }

        void CreateOfflineUser()
        {
            if (UserData.UserId.Length == 0)
            {
                UserData.UserId = StringExt.RandomAlphabet(12);
                UserData.ProfileId = StringExt.RandomAlphabetWithNumber(6);
            }
        }
    }
}