using System;
using TMPro;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(TickInterval))]
    public class UICountDownTime : MonoBehaviour
    {
        public TextMeshProUGUI timeText;

        private TickInterval _tickInterval;
        public TickInterval TickInterval { get { if (_tickInterval == null) _tickInterval = GetComponent<TickInterval>(); return _tickInterval; } }

        bool counting;
        float seconds = 0;
        public float Seconds => seconds;

        public event Action OnEndCountdown = delegate { };

        private void OnEnable() => TickInterval.onTick += OnTick;
        private void OnDisable() => TickInterval.onTick -= OnTick;

        void Reset()
        {
            if (!timeText)
            {
                timeText = GetComponent<TextMeshProUGUI>();
                gameObject.name = "UICountDownTime";
            }
        }

        public void StartWithDuration(float seconds)
        {
            if (seconds <= 0) return;

            counting = true;
            this.seconds = Mathf.Max(0, seconds);
            if (timeText) timeText.text = ToTimeFormat(this.seconds);
            TickInterval.Restart();
        }

        protected virtual string ToTimeFormat(float seconds)
        {
            return Helper.ToHourFormat(Mathf.RoundToInt(this.seconds));
        }

        void OnTick()
        {
            if (seconds > 0)
            {
                seconds = Mathf.Max(0, seconds - TickInterval.TimeEslaped);
                if (timeText) timeText.text = ToTimeFormat(seconds);
            }
            else if (counting)
            {
                counting = false;
                if (timeText) timeText.text = ToTimeFormat(0);
                OnEndCountdown();
            }
        }
    }
}