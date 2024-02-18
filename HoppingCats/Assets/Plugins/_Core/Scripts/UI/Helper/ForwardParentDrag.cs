using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace moonNest
{
    public class ForwardParentDrag : IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public bool RouteToParent { get; set; }
        public bool Horizontal { get; set; } = true;
        public bool Vertical { get; set; } = false;

        public IInitializePotentialDragHandler ParentInitializePotentialDragHandler { get; private set; }
        public IBeginDragHandler ParentBeginDragHandler { get; private set; }
        public IDragHandler ParentDragHandler { get; private set; }
        public IEndDragHandler ParentEndDragHandler { get; private set; }

        bool cachedParent;
        readonly Transform transform;

        public ForwardParentDrag(Transform transform)
        {
            this.transform = transform;
        }

        /// <summary>
        /// Do action for all parents
        /// </summary>
        public void FindAndStoreNestedParent()
        {
            ParentInitializePotentialDragHandler = null;
            ParentBeginDragHandler = null;
            ParentDragHandler = null;
            ParentEndDragHandler = null;

            Transform tr = transform;
            // Find the first parent that implements all of the interfaces
            while ((tr = tr.parent) && ParentInitializePotentialDragHandler == null)
            {
                ParentInitializePotentialDragHandler = tr.GetComponent(typeof(IInitializePotentialDragHandler)) as IInitializePotentialDragHandler;
                if (ParentInitializePotentialDragHandler == null)
                    continue;

                ParentBeginDragHandler = ParentInitializePotentialDragHandler as IBeginDragHandler;
                if (ParentBeginDragHandler == null)
                {
                    ParentInitializePotentialDragHandler = null;
                    continue;
                }

                ParentDragHandler = ParentInitializePotentialDragHandler as IDragHandler;
                if (ParentDragHandler == null)
                {
                    ParentInitializePotentialDragHandler = null;
                    ParentBeginDragHandler = null;
                    continue;
                }

                ParentEndDragHandler = ParentInitializePotentialDragHandler as IEndDragHandler;
                if (ParentEndDragHandler == null)
                {
                    ParentInitializePotentialDragHandler = null;
                    ParentBeginDragHandler = null;
                    ParentDragHandler = null;
                    continue;
                }
            }

            cachedParent = true;
        }

        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (!cachedParent) FindAndStoreNestedParent();
            if (ParentInitializePotentialDragHandler == null) return;
            ParentInitializePotentialDragHandler.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                RouteToParent = true;
            else if (!Vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                RouteToParent = true;
            else
                RouteToParent = false;

            if (!RouteToParent) return;
            ParentBeginDragHandler?.OnBeginDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!RouteToParent) return;
            ParentDragHandler?.OnDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!RouteToParent) return;
            ParentEndDragHandler?.OnEndDrag(eventData);
            RouteToParent = false;
        }
    }
}