using UnityEngine;

namespace moonNest
{
    public class CheatButton : MonoBehaviour
    {
        protected virtual void Awake()
        {
#if ENABLE_CHEAT
            gameObject.SetActive(true);
#else
            gameObject.SetActive(false);
#endif
        }

        void OnValidate()
        {
            gameObject.name = "Cheat Button";
        }
    }
}