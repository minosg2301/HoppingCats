using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class CheatCurrencyBtn : MonoBehaviour
    {
        public string currencyName;
        public int value = 100000;

        private Button _button;
        public Button Button { get { if(_button == null) _button = GetComponent<Button>(); return _button; } }

        public void Awake()
        {
#if ENABLE_CHEAT
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
            Button.onClick.AddListener(CheatMoney);
        }

        public void CheatMoney()
        {
            if(UserCurrency.Get(currencyName) != null)
                UserCurrency.Get(currencyName).AddValue(value);
        }
    }
}