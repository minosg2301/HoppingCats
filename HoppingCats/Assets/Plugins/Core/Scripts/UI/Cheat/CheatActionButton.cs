using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Button))]
    public class CheatActionButton : CheatButton
    {
        public ActionId actionId;
        public int time = 1;

#if UNITY_EDITOR
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                var uiquest = transform.parent.GetComponent<UIQuest>();
                if (uiquest)
                {
                    actionId = uiquest.Quest.ActionRequire.action.id;
                }
                CoreHandler.DoAction(new ActionData(actionId), time);
            });
        }
#endif
    }
}