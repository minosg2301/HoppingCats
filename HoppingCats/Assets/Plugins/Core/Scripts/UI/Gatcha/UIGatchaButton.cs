using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace moonNest
{
    public class UIGatchaButton : MonoBehaviour, IObserver
    {
        public UIButton button;
        public UICountDownTime countDownTime;
        public GameObject spinNode;
        public GameObject luckyBoxesNode;
        public UnityEvent onSpinClick;
        public UnityEvent onLuckyBoxClick;

        void Reset()
        {
            if(!button) button = GetComponent<UIButton>();
        }

        void OnValidate()
        {
            gameObject.name = "UIGatchaButton";
        }

        void Start()
        {
            button.OnClick.OnTrigger.Event.AddListener(HandleClick);
            UserGatcha.Ins.Subscribe(this);
        }

        private void HandleClick()
        {
            if(UserGatcha.Ins.Spin != null) onSpinClick.Invoke();
            else onLuckyBoxClick.Invoke();
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            UserGatcha instance = UserGatcha.Ins;
            spinNode.SetActive(instance.Spin != null);
            luckyBoxesNode.SetActive(instance.LuckyBoxes != null);
            countDownTime.StartWithDuration((float) instance.LastSeconds);
        }
    }
}