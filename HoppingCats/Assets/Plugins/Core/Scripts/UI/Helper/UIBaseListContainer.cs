using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moonNest
{
    public abstract class UIBaseListContainer<T, UI> : MonoBehaviour where UI : BaseUIData<T>
    {
        public Transform container;
        public UI prefab;

        List<UI> uiList = new List<UI>();
        bool destroyRedundant = false;
        bool replacePrefab;

        public ReadOnlyCollection<T> List { get; private set; }

        Action<UI> onUpdated;
        List<UI> oldUIList;

        public void SetList(List<T> list, Action<UI> onUpdated = null)
        {
            uiList.Clear();
            if (!container) return;

            this.onUpdated = onUpdated;
            uiList = container.GetComponentsInChildren<UI>(true).ToList();
            oldUIList = new List<UI>();

            List = list.AsReadOnly();
            if (replacePrefab) List.ForEach(UpdateItemWithReplaceRefab);
            else List.ForEach(UpdateItem);

            oldUIList.ForEach(ui => Destroy(ui.gameObject));

            CheckRedundant(list);
        }

        void UpdateItem(T data, int index)
        {
            if (index >= uiList.Count)
                uiList.Add(Instantiate(GetPrefab(data, index), container));

            uiList[index].gameObject.SetActive(true);
            uiList[index].SetData(data);
            onUpdated?.Invoke(uiList[index]);
        }

        void UpdateItemWithReplaceRefab(T data, int index)
        {
            if (index >= uiList.Count)
            {
                uiList.Add(Object.Instantiate(GetPrefab(data, index), container));
                uiList[index].SetData(data);
                onUpdated?.Invoke(uiList[index]);
            }
            else
            {
                UI oldUI = uiList[index];
                oldUIList.Add(oldUI);
                uiList[index] = Object.Instantiate(GetPrefab(data, index), container);
                uiList[index].gameObject.SetActive(true);
                uiList[index].SetData(data);
                onUpdated?.Invoke(uiList[index]);
            }
        }

        protected virtual UI GetPrefab(T element, int index) => prefab;

        public void SetList(GameObject container, List<T> list, Action<T, UI, int> onUpdate)
        {
            if (uiList.Count == 0) uiList = container.GetComponentsInChildren<UI>().ToList();

            list.ForEach((ele, i) =>
            {
                if (uiList.Count <= i) uiList.Add(Instantiate(prefab, container.transform));

                uiList[i].gameObject.SetActive(true);
                onUpdate.Invoke(ele, uiList[i], i);
            });

            CheckRedundant(list);
        }

        void CheckRedundant(List<T> list)
        {
            if (destroyRedundant)
            {
                // remove unused ui shop item
                for (int i = list.Count; i < uiList.Count; i++)
                    Destroy(uiList[i].gameObject);
            }
            else
            {
                // remove unused ui shop item
                for (int i = list.Count; i < uiList.Count; i++)
                    uiList[i].gameObject.SetActive(false);
            }
        }
    }
}