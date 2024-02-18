using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Button))]
    public class CheatUserStatButton : MonoBehaviour
    {
        public UserStatId id;
        public int value;

        private Button _button;
        public Button Button { get { if(!_button) _button = GetComponent<Button>(); return _button; } }

        protected virtual void Awake()
        {
#if ENABLE_CHEAT
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif

#if UNITY_EDITOR
            Button.onClick.AddListener(AddUserStat);
#endif
        }

        private void OnValidate()
        {
            gameObject.name = "Cheat Button";
        }

#if UNITY_EDITOR
        public void AddUserStat()
        {
            UserData.AddStat(id, value);
        }
#endif
    }
}