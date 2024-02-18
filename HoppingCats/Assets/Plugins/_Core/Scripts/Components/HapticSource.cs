using System;
using UnityEngine;

namespace moonNest
{
    public class HapticSource : MonoBehaviour
    {
        public HapticType type;
        public float interval = 1f;

        private DateTime lastTime;

        private void Start()
        {
            lastTime = DateTime.MinValue;
        }

        public void Play()
        {
            if(!UserData.HapticOn) return;

            if(lastTime.AddSeconds(interval) < DateTime.UtcNow)
            {
                lastTime = DateTime.UtcNow;
                DoHaptic(type);
            }
        }

        void DoHaptic(HapticType type)
        {
#if UNITY_IPHONE
          if(Vibration.HapticsSupported()) Vibration.DoHaptic(type);
#else
          Vibration.DoHaptic(type);
#endif
        }
    }
}