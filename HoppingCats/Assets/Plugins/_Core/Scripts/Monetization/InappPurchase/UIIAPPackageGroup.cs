using UnityEngine;

namespace moonNest
{
    public class UIIAPPackageGroup : MonoBehaviour, IObserver
    {
        public IAPGroupId groupId = -1;
        public UICountDownTime refreshTime;
        public bool statusSort;

        protected UIPackageContainer container = new UIPackageContainer();

        InStorePackageGroup group;

        void OnValidate()
        {
            if(groupId == -1) return;

            IAPPackageGroup packageGroup = IAPPackageAsset.Ins.FindGroup(groupId);
            gameObject.name = "UIIAPGroup - " + (packageGroup != null ? packageGroup.name : "");
        }

        void OnEnable()
        {
            if(groupId != -1)
            {
                group = UserStore.Ins.FindGroup(groupId);
                UserStore.Ins.Subscribe(this, groupId.ToString());
            }
        }

        void OnDisable()
        {
            if (groupId != -1)
            {
                UserStore.Ins.Unsubscribe(this);
            }
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            var inStorePackages = UserStore.Ins.FindActiveInGroup(groupId);
            container.SetList(transform, inStorePackages);

            if(statusSort)
            {
                container.UIList.SortDesc(uiPackage => uiPackage.InStorePackage.Detail.free ? 0 : (uiPackage.InStorePackage.OutStock ? -1 : 1));
                container.UIList.ForEach((uiPackage, i) => uiPackage.transform.SetSiblingIndex(i));
            }

            OnPostUpdatePackages();

            if(refreshTime)
            {
                refreshTime.gameObject.SetActive(group.Detail.refreshConfig.enabled);
                refreshTime.StartWithDuration((float)group.LastSeconds);
            }
        }

        protected virtual void OnPostUpdatePackages() { }

        protected class UIPackageContainer : UIListContainer<InStorePackage, UIIAPPackage>
        {
            public UIPackageContainer() : base() 
            {
                replacePrefab = true;
            }

            protected override UIIAPPackage GetPrefab(InStorePackage element, int index)
            {
                if(element.Detail.customPrefab) return element.Detail.customPrefab;
                else return UIList[0];
            }
        }
    }
}