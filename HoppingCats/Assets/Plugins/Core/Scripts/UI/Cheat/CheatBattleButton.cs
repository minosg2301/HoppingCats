using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Button))]
    public class CheatBattleButton : CheatButton
    {
        public int value = 20;

#if UNITY_EDITOR
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => UserData.AddStat(ArenaAsset.Ins.requireStatId, value));
        }
#endif
    }
}