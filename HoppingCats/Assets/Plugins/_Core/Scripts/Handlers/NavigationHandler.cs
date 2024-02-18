using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using moonNest;

namespace moonNest
{
    public class NavigationHandler : SingletonMono<NavigationHandler>, IObservable
    {
        #region observer
        private ObserverProvider<NavigationHandler> provider = new ObserverProvider<NavigationHandler>();

        public void Subscribe(string scope, Action<NavigationHandler> handler, bool notify = true)
        {
            provider.Subcribe(scope, handler);
            if (notify) handler.Invoke(this);
        }

        public void Unsubscribe(string scope, Action<NavigationHandler> handler)
        {
            provider.Unsubscribe(scope, handler);
        }

        public void Notify(params string[] scopes) => provider.Notify(this, scopes);

        #endregion

        public void DoNavigate(int navigationId)
        {
            UIPopup.HideAllPopups();
            var navigationData = NavigationAsset.Ins.FindNavigationData(navigationId);
            StartCoroutine(DoNavigateWithDelayTime(navigationData));

        }

        IEnumerator DoNavigateWithDelayTime(NavigationData data)
        {
            foreach (var path in data.paths)
            {
                if (path.delayTime > 0)
                    yield return new WaitForSeconds(path.delayTime);
                else yield return 0;

                Navigate(path.gameEventId);
            }
        }

        private void Navigate(int pathId)
        {
            GameEventMessage.SendEvent(GameDefinitionAsset.Ins.FindGameEvent(pathId).name);
        }
    }
}
