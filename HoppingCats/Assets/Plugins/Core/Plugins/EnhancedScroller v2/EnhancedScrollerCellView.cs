using UnityEngine;
using System;
using System.Collections;

namespace EnhancedUI.EnhancedScroller
{
    /// <summary>
    /// This is the base class that all cell views should derive from
    /// </summary>
    public class EnhancedScrollerCellView : MonoBehaviour
    {
        /// <summary>
        /// The PrefabInstanceID is a id of prefab
        /// That allows the scroller to handle different types of cells in a single list.
        /// </summary>
        public int PrefabInstanceID { get; set; }

        /// <summary>
        /// The cell index of the cell view
        /// This will differ from the dataIndex if the list is looping
        /// </summary>
        [NonSerialized]
        public int cellIndex;

        /// <summary>
        /// The data index of the cell view
        /// </summary>
        [NonSerialized]
        public int dataIndex;

        /// <summary>
        /// Whether the cell is active or recycled
        /// </summary>
        [NonSerialized]
        public bool active;

        /// <summary>
        /// Custom data that view can hold
        /// </summary>
        [NonSerialized]
        public object customData;

        /// <summary>
        /// This method is called by the scroller when the RefreshActiveCellViews is called on the scroller
        /// You can override it to update your cell's view UID
        /// </summary>
        public virtual void RefreshCellView() { }
    }
}