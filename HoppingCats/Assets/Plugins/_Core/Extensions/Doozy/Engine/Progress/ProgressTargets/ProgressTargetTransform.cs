// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Engine.Utils;
using UnityEngine;
using Doozy.Engine.UI.Animation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Doozy.Engine.Progress
{
    /// <inheritdoc />
    /// <summary>
    /// Used by a Progressor to update the values of an Transform component
    /// </summary>
    ///
    [AddComponentMenu(MenuUtils.ProgressTargetTransform_AddComponentMenu_MenuName, MenuUtils.ProgressTargetTransform_AddComponentMenu_Order)]
    [DefaultExecutionOrder(DoozyExecutionOrder.PROGRESS_TARGET_TRANSFORM)]
    public class ProgressTargetTransform : ProgressTarget
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [MenuItem(MenuUtils.ProgressTargetTransform_MenuItem_ItemName, false, MenuUtils.ProgressTargetTransform_MenuItem_Priority)]
        private static void CreateComponent(MenuCommand menuCommand) { DoozyUtils.AddToScene<ProgressTargetTransform>(MenuUtils.ProgressTargetTransform_GameObject_Name, false, true); }
#endif

        #endregion

        #region Public Variables

        /// <summary> Target Transform component </summary>
        public Transform Transform;

        /// <summary> Scale Value for progress</summary>
        public float ScaleBy = 0.4f;

        /// <summary> Delta Position for progress</summary>
        public Vector2 MoveBy = Vector3.zero;

        /// <summary> Progress direction to be used (Progress or InverseProgress) </summary>
        public TargetProgress TargetProgress;

        #endregion

        #region Private Variables

        private Vector3 StartScale = UIAnimator.DEFAULT_START_SCALE;
        private Vector2 StartPosition = UIAnimator.DEFAULT_START_SCALE;

        private bool UpdateStartValue = true;

        private RectTransform _rectTransform;
        public RectTransform RectTransform { get { if (!_rectTransform) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

        #endregion

        #region Public Methods

        public override void UpdateTarget(Progressor progressor)
        {
            base.UpdateTarget(progressor);

            if(Transform == null) return;

            if(UpdateStartValue)
            {
                StartScale = Transform.localScale;
                StartPosition = RectTransform.anchoredPosition;
                UpdateStartValue = false;
            }

            Transform.localScale = StartScale + (TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress) * ScaleBy * Vector3.one;
            RectTransform.anchoredPosition = StartPosition + (TargetProgress == TargetProgress.Progress ? progressor.Progress : progressor.InverseProgress) * MoveBy;
        }

        #endregion

        #region Private Methods

        private void Reset() { UpdateReference(); }

        private void UpdateReference()
        {
            if(!Transform)
                Transform = GetComponent<Transform>();
        }

        #endregion
    }
}
