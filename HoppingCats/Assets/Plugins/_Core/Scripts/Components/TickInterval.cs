using UnityEngine;

namespace moonNest
{
    public class TickInterval : MonoBehaviour
    {
        public float interval = 1;
        private float NextTickTime { get; set; }
        private float LastTickTime { get; set; }

        public float TimeEslaped => Time.realtimeSinceStartup - LastTickTime;

        public bool Paused { get; private set; }
        public float PausedTime { get; private set; }

        public delegate void TickEvent();
        public TickEvent onTick = delegate { };

        void Awake()
        {
            LastTickTime = NextTickTime = Time.realtimeSinceStartup;
        }

        void Update()
        {
            if (NextTickTime <= Time.realtimeSinceStartup)
            {
                onTick();
                LastTickTime = Time.realtimeSinceStartup;
                NextTickTime = Time.realtimeSinceStartup + interval;
            }
        }

        public void Restart()
        {
            enabled = true;
            LastTickTime = Time.realtimeSinceStartup;
            NextTickTime = Time.realtimeSinceStartup + interval;
        }

        public void Resume()
        {
            if (!Paused) return;

            enabled = true;
            Paused = false;
            NextTickTime += Time.realtimeSinceStartup - PausedTime;
        }

        public void Pause()
        {
            if (Paused) return;

            enabled = false;
            Paused = true;
            PausedTime = Time.realtimeSinceStartup;
        }
    }
}