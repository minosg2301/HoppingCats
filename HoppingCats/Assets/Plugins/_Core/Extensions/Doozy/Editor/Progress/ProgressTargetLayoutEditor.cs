// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Engine.Extensions;
using Doozy.Engine.Progress;
using Doozy.Engine.Utils;
using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Doozy.Editor.Progress
{
    [CustomEditor(typeof(ProgressTargetLayout))]
    [CanEditMultipleObjects]
    public class ProgressTargetLayoutEditor : BaseEditor
    {
        protected override ColorName ComponentColorName { get { return DGUI.Colors.ProgressorColorName; } }

        private SerializedProperty
            m_updatePreferredWidth,
            m_updateWidthBy,
            m_updatePreferredHeight,
            m_updateHeightBy,
            m_target,
            m_targetProgress;

        private AnimBool
            m_updateWidthExpanded,
            m_updateHeightExpanded;


        private bool HasReference { get { return m_target.objectReferenceValue != null; } }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_updatePreferredWidth = GetProperty(PropertyName.UpdatePreferredWidth);
            m_updateWidthBy = GetProperty(PropertyName.UpdateWidthBy);
            m_updatePreferredHeight = GetProperty(PropertyName.UpdatePreferredHeight);
            m_updateHeightBy = GetProperty(PropertyName.UpdateHeightBy);
            m_target = GetProperty(PropertyName.Target);
            m_targetProgress = GetProperty(PropertyName.TargetProgress);
        }

        protected override void InitAnimBool()
        {
            base.InitAnimBool();

            m_updateWidthExpanded = GetAnimBool(m_updatePreferredWidth.propertyPath, m_updatePreferredWidth.boolValue);
            m_updateHeightExpanded = GetAnimBool(m_updatePreferredHeight.propertyPath, m_updatePreferredHeight.boolValue);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderProgressTarget), MenuUtils.ProgressTargetImage_Manual, MenuUtils.ProgressTargetImage_YouTube);
            GUILayout.Space(DGUI.Properties.Space(2));

            bool hasReference = HasReference;
            ColorName colorName = hasReference ? ComponentColorName : ColorName.Red;

            DrawUpdateValue(m_updateWidthExpanded, UILabels.UpdatePreferredWidth, m_updateWidthBy, m_updatePreferredWidth);

            GUILayout.Space(DGUI.Properties.Space());
            DrawUpdateValue(m_updateHeightExpanded, UILabels.UpdatePreferredHeight, m_updateHeightBy, m_updatePreferredHeight);

            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_target, UILabels.Target, colorName);
            GUI.enabled = hasReference;

            GUILayout.Space(DGUI.Properties.Space());
            DGUI.Property.Draw(m_targetProgress, UILabels.TargetProgress, colorName);

            GUI.enabled = true;

            GUILayout.Space(DGUI.Properties.Space(4));
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawUpdateValue(AnimBool animBool, string label, SerializedProperty valueProperty, SerializedProperty enabledProperty)
        {
            animBool.target = enabledProperty.boolValue;

            ColorName backgroundColorName = DGUI.Colors.GetBackgroundColorName(enabledProperty.boolValue, ComponentColorName);
            ColorName textColorName = DGUI.Colors.GetTextColorName(enabledProperty.boolValue, ComponentColorName);

            float alpha = GUI.color.a;
            DGUI.Line.Draw(true, backgroundColorName,
                () =>
                {
                    DGUI.Toggle.Switch.Draw(enabledProperty, backgroundColorName, DGUI.Properties.SingleLineHeight);
                    GUILayout.Space(DGUI.Properties.Space());
                    GUI.color = GUI.color.WithAlpha(DGUI.Properties.TextIconAlphaValue(enabledProperty.boolValue));
                    DGUI.Label.Draw(label, Size.S, textColorName, DGUI.Properties.SingleLineHeight);
                    GUI.color = GUI.color.WithAlpha(alpha);
                    if(DGUI.AlphaGroup.Begin(animBool.faded))
                    {
                        GUILayout.Space(DGUI.Properties.Space());
                        DGUI.Label.Draw(UILabels.By, Size.S, ComponentColorName, DGUI.Properties.SingleLineHeight);
                        DGUI.Property.Draw(valueProperty, ComponentColorName, DGUI.Properties.DefaultFieldWidth, DGUI.Properties.SingleLineHeight);
                    }

                    DGUI.AlphaGroup.End(alpha);
                },
                GUILayout.FlexibleSpace);
            GUILayout.Space(DGUI.Properties.Space());

        }
    }
}