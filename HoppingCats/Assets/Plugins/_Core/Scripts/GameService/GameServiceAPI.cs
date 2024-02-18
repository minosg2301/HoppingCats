using Cysharp.Threading.Tasks;

namespace moonNest
{
    internal class GameServiceAPI
    {
        static readonly string url;
        static readonly string xxteakey;

        static GameServiceAPI()
        {
            xxteakey = "DGDd@$8*N1+@R?v";
            url = GlobalConfig.Ins.GameServiceURL;
        }

        internal static async UniTask<T> Get<T>(string request)
        {
            return await RestAPI.GET<T>(url + request);
        }
    }
}