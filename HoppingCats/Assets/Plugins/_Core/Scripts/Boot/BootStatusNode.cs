using Doozy.Engine;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(TickInterval))]
    public class BootStatusNode : MonoBehaviour
    {
        public string statusEvent;
        public TextMeshProUGUI statusText;
        public bool useDotAnim = true;

        private Localize _statusLoc;
        public Localize StatusLoc { get { if (!_statusLoc && statusText) _statusLoc = statusText.GetComponent<Localize>(); return _statusLoc; } }

        int dot = 0;
        string text = "";

        void Reset()
        {
            if (!statusText) statusText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start()
        {
            dot = 0;
            Message.AddListener<EventMessage>(statusEvent, OnStatusUpdate);
            if (useDotAnim) GetComponent<TickInterval>().onTick = OnTick;
        }

        void OnDestroy()
        {
            Message.RemoveListener<EventMessage>(statusEvent, OnStatusUpdate);
        }

        void OnTick()
        {
            if (text.Length == 0) return;

            statusText.text = text.PadRight(text.Length + dot, '.');
            dot = (dot + 1) % 4;
        }

        void OnStatusUpdate(EventMessage msg)
        {
            statusText.gameObject.SetActive(true);
            SetStatus(msg.content);
        }

        public void SetStatus(string content)
        {
            if (content.StartsWith("TXT_"))
                content = content.ToLocalized();

            statusText.text = content;
            text = content;
            dot = 0;
        }
    }
}