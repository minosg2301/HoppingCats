using UnityEngine;

namespace moonNest
{
    public class UIIAPPackageGroupRefreshTime : MonoBehaviour, IObserver
    {
        public IAPGroupId groupId = -1;
        public UICountDownTime refreshTime;

        InStorePackageGroup group;

        void Reset()
        {
            if(!refreshTime) refreshTime = GetComponentInChildren<UICountDownTime>();
        }

        void Start()
        {
            if(groupId != -1)
            {
                group = UserStore.Ins.FindGroup(groupId);
                gameObject.SetActive(group.Detail.refreshConfig.enabled);
                if(group.Detail.refreshConfig.enabled)
                {
                    UserStore.Ins.Subscribe(this, groupId.ToString());
                }
            }
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            refreshTime.StartWithDuration((float)group.LastSeconds);
        }
    }
}