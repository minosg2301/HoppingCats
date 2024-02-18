// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
#endif

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Used by a Progressor to update the values of an Layout Element component
    /// </summary>
    ///
    [AddComponentMenu(MenuUtils.ProgressTargetLayout_AddComponentMenu_MenuName, MenuUtils.ProgressTargetLayout_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESS_TARGET_TRANSFORM)]
    public class ProgressTargetLayout : ProgressTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressTargetLayout_MenuItem_ItemName, false, MenuUtils.ProgressTargetLayout_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ProgressTargetTransform>(MenuUtils.ProgressTargetLayout_GameObject_Name, false, true); }
#endif

        #endregion

        #region Public Variables

        /// <summary> Target Layout Element component </summary>
        public LayoutElement Target;

        /// <summary> Flag for updating flexible width </summary>
        public bool UpdatePreferredWidth;

        /// <summary> Scale Value for progress</summary>
        public float UpdateWidthBy = 10f;

        /// <summary> Flag for updating flexible width </summary>
        //public bool UpdateFlexibleWidth;

        /// <summary> Scale Value for progress</summary>
        //public float UpdateWidthBy = 10f;

        /// <summary> Flag for updating flexible height </summary>
        public bool UpdatePreferredHeight;

        /// <summary> Scale Value for progress</summary>
        public float UpdateHeightBy = 10f;

        /// <summary> Flag for updating flexible height </summary>
        //public bool UpdateFlexibleHeight;

        /// <summary> Scale Value for progress</summary>
        //public float UpdateHeightBy = 10f;

        /// <summary> Progress direction to be used (Progress or InverseProgress) </summary>
        public TargetProgress TargetProgress;

        #endregion

        #region Private Variables

        private float startPreferredWidth = 0f;
        private float startPreferredHeight = 0f;

        private bool updateStartValue = true;
        #endregion

        #region Public Methods

        public override void UpdateTarget(Progressor progressor)
        {
            base.UpdateTarget(progressor);

            if(Target == null) return;

            if(updateStartValue)
            {
                startPreferredWidth = Target.preferredWidth;
                startPreferredHeight = Target.preferredHeight;
                updateStartValue = false;
            }

            if(UpdatePreferredWidth)
                Target.preferredWidth = startPreferredWidth + UpdateWidthBy * (TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress);

            if(UpdatePreferredHeight)
                Target.preferredHeight = startPreferredHeight + UpdateHeightBy * (TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress);

            //if(UpdateFlexibleWidth)
            //    Target.flexibleWidth = startWidth + UpdateWidthBy * (TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress);

            //if(UpdateFlexibleWidth)
            //    Target.flexibleHeight = startHeight + UpdateHeightBy * (TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress);
        }

        #endregion

        #region Private Methods

        private void Reset() { UpdateReference(); }

        private void UpdateReference()
        {
            if(!Target)
                Target = GetComponent<LayoutElement>();
        }

        #endregion
    }
}
