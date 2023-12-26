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

        IInitializePotentialDragHandler parentInitializePotentialDragHandler;
        IBeginDragHandler parentBeginDragHandler;
        IDragHandler parentDragHandler;
        IEndDragHandler parentEndDragHandler;

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
            parentInitializePotentialDragHandler = null;
            parentBeginDragHandler = null;
            parentDragHandler = null;
            parentEndDragHandler = null;

            Transform tr = transform;
            // Find the first parent that implements all of the interfaces
            while((tr = tr.parent) && parentInitializePotentialDragHandler == null)
            {
                parentInitializePotentialDragHandler = tr.GetComponent(typeof(IInitializePotentialDragHandler)) as IInitializePotentialDragHandler;
                if(parentInitializePotentialDragHandler == null)
                    continue;

                parentBeginDragHandler = parentInitializePotentialDragHandler as IBeginDragHandler;
                if(parentBeginDragHandler == null)
                {
                    parentInitializePotentialDragHandler = null;
                    continue;
                }

                parentDragHandler = parentInitializePotentialDragHandler as IDragHandler;
                if(parentDragHandler == null)
                {
                    parentInitializePotentialDragHandler = null;
                    parentBeginDragHandler = null;
                    continue;
                }

                parentEndDragHandler = parentInitializePotentialDragHandler as IEndDragHandler;
                if(parentEndDragHandler == null)
                {
                    parentInitializePotentialDragHandler = null;
                    parentBeginDragHandler = null;
                    parentDragHandler = null;
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
            if(!cachedParent) FindAndStoreNestedParent();
            if(parentInitializePotentialDragHandler == null) return;
            parentInitializePotentialDragHandler.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!Horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
                RouteToParent = true;
            else if(!Vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                RouteToParent = true;
            else
                RouteToParent = false;

            if(parentBeginDragHandler == null) return;
            if(RouteToParent) parentBeginDragHandler.OnBeginDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if(parentBeginDragHandler == null) return;
            if(RouteToParent) parentDragHandler.OnDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if(parentBeginDragHandler == null) return;
            if(RouteToParent) parentEndDragHandler.OnEndDrag(eventData);
            RouteToParent = false;
        }
    }
}
