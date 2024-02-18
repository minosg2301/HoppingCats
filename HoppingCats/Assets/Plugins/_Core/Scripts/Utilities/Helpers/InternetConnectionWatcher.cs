using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace moonNest
{
    public class InternetConnectionWatcher : SingletonMono<InternetConnectionWatcher>
    {
        public float pingInterval = 5f;
        public float checkReachableInterval = 2f;

        public static event Action OnNoConnection = delegate { };
        public static event Action OnHaveConnection = delegate { };

        float checkByPingTime;
        float internetReachableCheckingTime;
        bool InternetReachable => Application.internetReachability != NetworkReachability.NotReachable;

        public bool HaveInternet { get; private set; } = true;


        public static Task<bool> CheckInternet() => Ins.DoPing();

        protected override void Awake()
        {
            base.Awake();
            internetReachableCheckingTime = checkByPingTime = Time.time;
        }

        #region private methods
        void LateUpdate()
        {
            if (CheckInternetReachable())
                CheckByPing();
        }

        async void CheckByPing()
        {
            if (checkByPingTime > Time.time)
                return;

            await DoPing();
        }

        bool CheckInternetReachable()
        {
            if (internetReachableCheckingTime > Time.time)
                return true;

            internetReachableCheckingTime = Time.time + checkReachableInterval;
            HandleResult(InternetReachable);
            return InternetReachable;
        }

#if UNITY_WEBGL
        private Task<bool> DoPing()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            return tcs.Task;
        }
#else
        async Task<bool> DoPing()
        {
            checkByPingTime = Time.time + pingInterval;
            var wr = UnityWebRequest.Get("https://www.google.com");
            try
            {
                await wr.SendWebRequest();
                if (!string.IsNullOrEmpty(wr.error) && wr.responseCode != 429) { HandleResult(false); return false; }
                else { HandleResult(true); return true; }
            }
            catch (Exception) { HandleResult(false); }
            return false;
        }
#endif

        void HandleResult(bool _haveInternet)
        {
            if (HaveInternet == _haveInternet) return;

            HaveInternet = _haveInternet;

            if (!_haveInternet) OnNoConnection();
            else OnHaveConnection();
        }
        #endregion
    }
}