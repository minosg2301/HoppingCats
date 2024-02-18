using System;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace moonNest
{
    internal class ConsentInfoUpdator : SingletonMono<ConsentInfoUpdator>
    {
        const string androidClassName = "com.vgames.ConsentInfoUpdator";

        public static event Action<bool> OnUpdateCompleted = delegate { };

        protected override void Start()
        {
            base.Start();

            name = "ConsentInfoUpdator";
        }

        public void UpdateConsentInfo()
        {
#if UNITY_EDITOR
            OnCompleted("true");
#else

#if UNITY_ANDROID
            AndroidUtil.CallJavaStaticMethod(androidClassName, "UpdateConsentInfo");
#elif UNITY_IOS
            ConsentInfoUpdator_UpdateConsentInfo();
#else
            OnCompleted("true");
#endif
#endif
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void ConsentInfoUpdator_UpdateConsentInfo();
#endif

        // Called from native
        void OnCompleted(string result)
        {
            bool ret = bool.TryParse(result, out var value);
            OnUpdateCompleted(value);
        }
    }
}
