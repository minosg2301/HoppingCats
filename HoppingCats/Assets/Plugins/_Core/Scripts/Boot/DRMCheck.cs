namespace moonNest
{
    public class DRMCheck
    {
        const string kServerParams = "/aa?!&;BASE64_STR";
        const string kParams = "me=APP_ID&you=CLIENT_PACKAGE";

        static string GetURL() => GlobalConfig.Ins.DRMServer + kServerParams;

        public static void DoCheck() { }
    }
}