using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moonNest
{
    public class UIListContainer<T, UI> where UI : BaseUIData<T>
    {
        public List<UI> UIList = new List<UI>();
        public bool destroyRedundant = false;
        public bool replacePrefab;

        public ReadOnlyCollection<T> List { get; private set; }

        private Action<UI> onUpdated;
        private Transform listContainer;
        private List<UI> oldUIList;

        public void SetList(Transform container, List<T> list, Action<UI> onUpdated = null)
        {
            UIList.Clear();
            if (!container) return;

            this.onUpdated = onUpdated;
            listContainer = container;
            UIList = container.GetComponentsInChildren<UI>(true).ToList();
            oldUIList = new List<UI>();

            List = list.AsReadOnly();
            if (replacePrefab) List.ForEach(UpdateItemWithReplaceRefab);
            else List.ForEach(UpdateItem);

            oldUIList.ForEach(ui => Object.Destroy(ui.gameObject));

            CheckRedundant(list);
        }

        public void SetList(GameObject container, List<T> list, Action<T, UI, int> onUpdate)
        {
            if (UIList.Count == 0) UIList = container.GetComponentsInChildren<UI>().ToList();

            list.ForEach((ele, i) =>
            {
                if (UIList.Count <= i) UIList.Add(Object.Instantiate(UIList[0], container.transform));

                UIList[i].gameObject.SetActive(true);
                onUpdate.Invoke(ele, UIList[i], i);
            });

            CheckRedundant(list);
        }

        void UpdateItem(T data, int index)
        {
            if (index >= UIList.Count)
                UIList.Add(Object.Instantiate(GetPrefab(data, index), listContainer));

            UIList[index].gameObject.SetActive(true);
            UIList[index].SetData(data);
            onUpdated?.Invoke(UIList[index]);
        }

        void UpdateItemWithReplaceRefab(T data, int index)
        {
            if (index >= UIList.Count)
            {
                UIList.Add(Object.Instantiate(GetPrefab(data, index), listContainer));
                UIList[index].SetData(data);
                onUpdated?.Invoke(UIList[index]);
            }
            else
            {
                UI oldUI = UIList[index];
                oldUIList.Add(oldUI);
                UIList[index] = Object.Instantiate(GetPrefab(data, index), listContainer);
                UIList[index].gameObject.SetActive(true);
                UIList[index].SetData(data);
                onUpdated?.Invoke(UIList[index]);
            }
        }

        protected virtual UI GetPrefab(T element, int index) => UIList[0];

        void CheckRedundant(List<T> list)
        {
            if (destroyRedundant)
            {
                // remove unused ui
                for (int i = list.Count; i < UIList.Count; i++)
                {
                    if (i == 0)
                    {
                        UIList[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        Object.Destroy(UIList[i].gameObject);
                    }
                }
            }
            else
            {
                // hide unused ui
                for (int i = list.Count; i < UIList.Count; i++)
                    UIList[i].gameObject.SetActive(false);
            }
        }
    }
}