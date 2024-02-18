using UnityEngine;

namespace moonNest
{
    public class DetectBrokenDevice : BootStep
    {
        public bool detectEmulator = false;
        public bool detectJailBreak = false;

        void Reset()
        {
            CalledOnlyOnce = true;
        }

        public override void OnStep()
        {
#if UNITY_ANDROID
            if (detectEmulator && ApplicationExt.IsEmulator())
                Application.Quit();

#elif UNITY_IOS
            if (detectJailBreak && ApplicationExt.IsJailBroken())
                Application.Quit();
#endif

            Complete();
        }
    }
}