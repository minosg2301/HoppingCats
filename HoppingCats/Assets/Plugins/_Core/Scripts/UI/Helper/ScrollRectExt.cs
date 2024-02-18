using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace moonNest
{
    public class ScrollRectExt : ScrollRect
    {
        ForwardParentDrag forwardDrag;

        protected override void Awake()
        {
            base.Awake();
            forwardDrag = new ForwardParentDrag(transform)
            {
                Vertical = vertical,
                Horizontal = horizontal
            };
        }

        /// <summary>
        /// Always route initialize potential drag event to parents
        /// </summary>
        public override void OnInitializePotentialDrag(PointerEventData eventData)
        {
            forwardDrag.OnInitializePotentialDrag(eventData);
            base.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        /// Begin drag event
        /// </summary>
        public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            forwardDrag.OnBeginDrag(eventData);
            if(forwardDrag.RouteToParent) return;
            base.OnBeginDrag(eventData);
        }

        /// <summary>
        /// Drag event
        /// </summary>
        public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(forwardDrag.RouteToParent)
                forwardDrag.OnDrag(eventData);
            else
                base.OnDrag(eventData);
        }

        /// <summary>
        /// End drag event
        /// </summary>
        public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if(forwardDrag.RouteToParent)
                forwardDrag.OnEndDrag(eventData);
            else
                base.OnEndDrag(eventData);
        }
    }
}