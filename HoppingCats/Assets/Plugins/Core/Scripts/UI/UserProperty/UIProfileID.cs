using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIProfileID : MonoBehaviour
    {
        public TextMeshProUGUI profileId;

        void Reset()
        {
            if(!profileId) profileId = GetComponent<TextMeshProUGUI>();
        }

        void OnEnable()
        {
            UserData.Ins.Subscribe(UserData.kProfile, OnProfileUpdated);
        }

        void OnDisable()
        {
            UserData.Ins.Unsubscribe(UserData.kProfile, OnProfileUpdated);
        }

        private void OnProfileUpdated(BaseUserData obj)
        {
            profileId.text = UserData.ProfileId;
        }
    }
}