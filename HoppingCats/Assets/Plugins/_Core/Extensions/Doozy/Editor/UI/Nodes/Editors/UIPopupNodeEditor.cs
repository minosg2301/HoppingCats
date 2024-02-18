// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Internal;
using Doozy.Editor.Nody.Editors;
using Doozy.Engine.UI;
using Doozy.Engine.UI.Nodes;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static Doozy.Editor.DGUI;
using Doozy.Engine.Extensions;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.UI.Base;
using Doozy.Engine.Nody.Models;
using System;
using Doozy.Editor.Nody;

namespace Doozy.Editor.UI.Nodes
{
    [CustomEditor(typeof(UIPopupNode))]
    public class UIPopupNodeEditor : BaseNodeEditor
    {
        private const string ERROR_NO_DRAWER_NAME = "ErrorNoDrawerName";

        private UIPopupNode TargetNode { get { return (UIPopupNode)target; } }

        private static UIPopupDatabase Database { get { return UIPopupSettings.Database; } }
        private static NamesDatabase ButtonsDatabase { get { return UIButtonSettings.Database; } }

        private SerializedProperty
            m_popupName,
            m_action;

        private List<UIConnection> m_outputValues;
        private List<UIConnection> OutputValues
        {
            get
            {
                if(m_outputValues != null) return m_outputValues;
                m_outputValues = new List<UIConnection>();
                foreach(Socket socket in TargetNode.OutputSockets)
                    OutputValues.Add(UIConnection.GetValue(socket));
                return m_outputValues;
            }
        }

        protected override void LoadSerializedProperty()
        {
            base.LoadSerializedProperty();

            m_popupName = GetProperty(PropertyName.PopupName);
            m_action = GetProperty(PropertyName.Action);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            AddInfoMessage(ERROR_NO_DRAWER_NAME, new InfoMessage(InfoMessage.MessageType.Error, UILabels.MissingDrawerNameTitle, UILabels.MissingDrawerNameMessage, TargetNode.ErrorNoDrawerName, Repaint));
        }

        protected override void OnGraphEvent(GraphEvent graphEvent) { UpdateNodeData(); }

        protected override void UpdateNodeData()
        {
            base.UpdateNodeData();
            m_outputValues = null;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawHeader(Styles.GetStyle(Styles.StyleName.ComponentHeaderUIDrawerNode), MenuUtils.UIDrawerNode_Manual, MenuUtils.UIDrawerNode_YouTube);
            DrawDebugMode(true);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawNodeName();
            DrawRenameNodeButton();
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawInputSockets(TargetNode);
            GUILayout.Space(DGUI.Properties.Space(8));
            DrawOutputSockets();
            GUILayout.Space(DGUI.Properties.Space(16));
            EditorGUI.BeginChangeCheck();
            DrawOptions();
            if(EditorGUI.EndChangeCheck()) NodeUpdated = true;
            GUILayout.Space(DGUI.Properties.Space(2));
            serializedObject.ApplyModifiedProperties();
            SendGraphEventNodeUpdated();
        }

        private void DrawOutputSockets()
        {
            SerializedProperty socketsProperty = GetProperty(PropertyName.m_outputSockets);
            List<Socket> sockets = BaseNode.OutputSockets;
            ColorName directionColorName = OutputColorName;
            string directionTitle = UILabels.OutputConnections;
            List<UIConnection> values = OutputValues;
            GUIStyle directionIconStyle = OutputSocketIconStyle;

            if(socketsProperty.arraySize == 0) return;

            if(sockets.Count != values.Count)
            {
                UpdateNodeData();
                return;
            }

            DrawSmallTitleWithBackground(directionIconStyle, directionTitle, directionColorName);
            Color initialColor = GUI.color;
            GUILayout.Space(DGUI.Properties.Space());

            for(int i = 0; i < socketsProperty.arraySize; i++)
            {
                Socket socket = sockets[i];

                string socketName = socket.SocketName;
                float curveModifier = socket.CurveModifier;
                int connectionsCount = socket.Connections.Count;

                UIConnectionTrigger trigger = values[i].Trigger;
                string buttonCategory = values[i].ButtonCategory;
                string buttonName = values[i].ButtonName;
                string gameEvent = values[i].GameEvent;
                float timeDelay = values[i].TimeDelay;

                ColorName colorName = socket.IsConnected ? directionColorName : ColorName.White;

                GUILayout.Space(DGUI.Properties.Space());
                GUILayout.BeginHorizontal(GUILayout.Height(kNodeLineHeight));
                {
                    DrawSocketIndex(i, directionColorName);

                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal(GUILayout.Height(kNodeLineHeight), GUILayout.ExpandWidth(true));
                        {
                            DGUI.Icon.Draw(socket.OverrideConnection
                                               ? Nody.Styles.GetStyle(Nody.Styles.StyleName.ConnectionPointOverrideConnected)
                                               : Nody.Styles.GetStyle(Nody.Styles.StyleName.ConnectionPointMultipleConnected),
                                           kConnectionTypeIconSize,
                                           kNodeLineHeight,
                                           connectionsCount > 0 ? ConnectionTypeIconColor(directionColorName) : NotConnectedIconColor()); //Draw socket connection type icon (Override / Multiple)

                            GUILayout.Space(kConnectionTypeIconPadding);

                            bool valueUpdated = false;
                            EditorGUI.BeginChangeCheck();

                            GUI.color = socket.IsConnected ? FieldsColor(directionColorName) : GUI.color;
                            GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight), GUILayout.Width(120));
                            {
                                GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);
                                trigger = (UIConnectionTrigger)EditorGUILayout.EnumPopup(trigger, GUILayout.ExpandWidth(true), GUILayout.Height(DGUI.Properties.SingleLineHeight)); //Draw output connection trigger popup (Game Event / Button)
                            }
                            GUILayout.EndVertical();
                            GUI.color = initialColor;

                            if(EditorGUI.EndChangeCheck()) valueUpdated = true;

                            GUIStyle triggerIcon;
                            switch(trigger)
                            {
                                case UIConnectionTrigger.ButtonClick:
                                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconButtonClick);
                                    break;
                                case UIConnectionTrigger.ButtonDoubleClick:
                                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconButtonDoubleClick);
                                    break;
                                case UIConnectionTrigger.ButtonLongClick:
                                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconButtonLongClick);
                                    break;
                                case UIConnectionTrigger.GameEvent:
                                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconGameEventListener);
                                    break;
                                case UIConnectionTrigger.TimeDelay:
                                    triggerIcon = Styles.GetStyle(Styles.StyleName.IconTime);
                                    break;

                                default: throw new ArgumentOutOfRangeException();
                            }

                            DGUI.Icon.Draw(triggerIcon, kConnectionTypeIconSize, kNodeLineHeight, connectionsCount > 0 ? TriggerTypeIconColor(directionColorName) : NotConnectedIconColor());

                            switch(trigger)
                            {
                                case UIConnectionTrigger.ButtonClick:
                                case UIConnectionTrigger.ButtonDoubleClick:
                                case UIConnectionTrigger.ButtonLongClick:
                                {
                                    GUI.color = connectionsCount > 0 ? FieldsColor(directionColorName) : GUI.color;

                                    var items = new List<string>();

                                    //BUTTON CATEGORY
                                    int buttonCategorySelectedIndex = ButtonsDatabase.CategoryNames.Contains(buttonCategory)
                                                                          ? ButtonsDatabase.CategoryNames.IndexOf(buttonCategory)
                                                                          : ButtonsDatabase.CategoryNames.IndexOf(NamesDatabase.CUSTOM);

                                    EditorGUI.BeginChangeCheck();

                                    GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight));
                                    {
                                        GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);
                                        buttonCategorySelectedIndex = EditorGUILayout.Popup(buttonCategorySelectedIndex, ButtonsDatabase.CategoryNames.ToArray(), GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                    }
                                    GUILayout.EndVertical();
                                    if(EditorGUI.EndChangeCheck())
                                    {
                                        valueUpdated = true;
                                        buttonCategory = ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex];
                                        if(ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex] != NamesDatabase.CUSTOM)
                                        {
                                            if(string.IsNullOrEmpty(buttonName.Trim()))
                                            {
                                                buttonName = ButtonsDatabase.GetNamesList(ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex])[0];
                                            }
                                            else if(buttonName.Trim() != NamesDatabase.UNNAMED &&
                                                     !ButtonsDatabase.GetNamesList(ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex]).Contains(buttonName.Trim()))
                                            {
                                                if(EditorUtility.DisplayDialog("Add Name",
                                                                                "Add the '" + buttonName.Trim() + "' name to the '" + ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex] + "' category?",
                                                                                "Yes",
                                                                                "No"))
                                                {
                                                    ButtonsDatabase.GetNamesList(ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex], true).Add(buttonName.Trim());
                                                    ButtonsDatabase.SetDirty(true);
                                                }
                                                else if(ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex] == NamesDatabase.GENERAL)
                                                {
                                                    buttonName = NamesDatabase.UNNAMED;
                                                }
                                                else
                                                {
                                                    buttonName = ButtonsDatabase.GetNamesList(ButtonsDatabase.CategoryNames[buttonCategorySelectedIndex])[0];
                                                }
                                            }
                                        }
                                    }

                                    bool hasCustomName = buttonCategory.Equals(NamesDatabase.CUSTOM);
                                    if(!ButtonsDatabase.Contains(buttonCategory)) //database does not contain this category -> reset it to custom
                                    {
                                        hasCustomName = true;
                                        buttonCategory = NamesDatabase.CUSTOM;
                                    }

                                    //BUTTON NAME
                                    if(!hasCustomName)
                                    {
                                        items = ButtonsDatabase.GetNamesList(buttonCategory);
                                        if(items.Count == 0)
                                        {
                                            if(!ButtonsDatabase.GetNamesList(NamesDatabase.GENERAL, true).Contains(NamesDatabase.UNNAMED))
                                            {
                                                ButtonsDatabase.GetNamesList(NamesDatabase.GENERAL, true).Add(NamesDatabase.UNNAMED);
                                                ButtonsDatabase.SetDirty(true);
                                            }

                                            buttonCategory = NamesDatabase.GENERAL;
                                            items = ButtonsDatabase.GetNamesList(buttonCategory);
                                        }
                                    }

                                    if(hasCustomName)
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight));
                                        {
                                            GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);
                                            buttonName = EditorGUILayout.TextField(buttonName, GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                        }
                                        GUILayout.EndVertical();
                                        if(EditorGUI.EndChangeCheck()) valueUpdated = true;
                                    }
                                    else
                                    {
                                        int buttonNameSelectedIndex = 0;
                                        if(items.Contains(buttonName))
                                        {
                                            buttonNameSelectedIndex = items.IndexOf(buttonName);
                                        }
                                        else
                                        {
                                            if(buttonCategory.Equals(NamesDatabase.GENERAL))
                                            {
                                                buttonName = NamesDatabase.UNNAMED;
                                                buttonNameSelectedIndex = items.IndexOf(NamesDatabase.UNNAMED);
                                            }
                                            else if(buttonName != NamesDatabase.UNNAMED &&
                                                     EditorUtility.DisplayDialog("Add Name",
                                                                                 "Add the '" + buttonName + "' name to the '" + buttonCategory + "' category?",
                                                                                 "Yes",
                                                                                 "No"))
                                            {
                                                string cleanName = buttonName.Trim();
                                                if(string.IsNullOrEmpty(cleanName))
                                                {
                                                    buttonName = items[buttonNameSelectedIndex];
                                                }
                                                else if(items.Contains(cleanName))
                                                {
                                                    buttonName = cleanName;
                                                    buttonNameSelectedIndex = items.IndexOf(cleanName);
                                                }
                                                else
                                                {
                                                    ButtonsDatabase.GetNamesList(buttonCategory, true).Add(cleanName);
                                                    ButtonsDatabase.SetDirty(true);
                                                    buttonName = cleanName;
                                                    buttonNameSelectedIndex = items.IndexOf(buttonName);
                                                }
                                            }
                                            else
                                            {
                                                buttonName = items[buttonNameSelectedIndex];
                                            }

                                            valueUpdated = true;
                                        }

                                        EditorGUI.BeginChangeCheck();
                                        GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight));
                                        {
                                            GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);
                                            buttonNameSelectedIndex = EditorGUILayout.Popup(buttonNameSelectedIndex, items.ToArray(), GUILayout.Height(DGUI.Properties.SingleLineHeight));
                                        }
                                        GUILayout.EndVertical();
                                        if(EditorGUI.EndChangeCheck())
                                        {
                                            valueUpdated = true;
                                            buttonName = items[buttonNameSelectedIndex];
                                        }
                                    }

                                    GUI.color = initialColor;
                                }
                                break;
                                case UIConnectionTrigger.GameEvent:
                                {
                                    GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight), GUILayout.ExpandWidth(true));
                                    {
                                        GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);

                                        GUILayout.BeginHorizontal(GUILayout.Height(kNodeLineHeight), GUILayout.ExpandWidth(true));
                                        {
                                            GUILayout.Space(DGUI.Properties.Space(3));
                                            GUI.color = socket.IsConnected ? FieldsColor(directionColorName) : GUI.color;
                                            EditorGUI.BeginChangeCheck();
                                            gameEvent = EditorGUILayout.TextField(gameEvent, GUILayout.Height(DGUI.Properties.SingleLineHeight), GUILayout.ExpandWidth(true));
                                            if(EditorGUI.EndChangeCheck()) valueUpdated = true;
                                            GUI.color = initialColor;
                                            if(DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaPaste),
                                                                                   UILabels.Paste,
                                                                                   Size.S, TextAlign.Left,
                                                                                   colorName, colorName,
                                                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                                            {
                                                gameEvent = EditorGUIUtility.systemCopyBuffer;
                                                valueUpdated = true;
                                            }

                                            GUILayout.Space(DGUI.Properties.Space());
                                            bool enabledState = GUI.enabled;
                                            GUI.enabled = !string.IsNullOrEmpty(gameEvent);
                                            if(DGUI.Button.Dynamic.DrawIconButton(Styles.GetStyle(Styles.StyleName.IconFaCopy),
                                                                                   UILabels.Copy,
                                                                                   Size.S, TextAlign.Left,
                                                                                   colorName, colorName,
                                                                                   DGUI.Properties.SingleLineHeight + DGUI.Properties.Space(2), false))
                                            {
                                                EditorGUIUtility.systemCopyBuffer = gameEvent;
                                                Debug.Log(UILabels.GameEvent + " '" + gameEvent + "' " + UILabels.HasBeenAddedToClipboard);
                                            }

                                            GUI.enabled = enabledState;
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                    GUILayout.EndVertical();
                                    GUI.color = initialColor;
                                }
                                break;
                                case UIConnectionTrigger.TimeDelay:
                                {
                                    EditorGUI.BeginChangeCheck();
                                    GUI.color = connectionsCount > 0 ? FieldsColor(directionColorName) : GUI.color;
                                    GUILayout.BeginVertical(GUILayout.Height(kNodeLineHeight), GUILayout.ExpandWidth(true));
                                    {
                                        GUILayout.Space((kNodeLineHeight - DGUI.Properties.SingleLineHeight) / 2);
                                        timeDelay = EditorGUILayout.FloatField(timeDelay, GUILayout.Height(DGUI.Properties.SingleLineHeight), GUILayout.ExpandWidth(true));
                                    }
                                    GUILayout.EndVertical();
                                    DGUI.Label.Draw(UILabels.SecondsDelay, Size.S, DGUI.Properties.SingleLineHeight);
                                    GUI.color = initialColor;
                                    if(EditorGUI.EndChangeCheck()) valueUpdated = true;
                                }
                                break;
                            }

                            if(valueUpdated)
                            {
                                Undo.RecordObject(TargetNode, "Update Socket Value");
                                socket.CurveModifier = curveModifier;
                                values[i].Trigger = trigger;
                                values[i].ButtonCategory = buttonCategory;
                                values[i].ButtonName = buttonName;
                                values[i].GameEvent = gameEvent;
                                values[i].TimeDelay = timeDelay;
                                UpdateSocketValue(sockets[i], values[i]);
                                NodeUpdated = true;
                            }
                        }
                        GUILayout.EndHorizontal();
                        if(DrawCurveModifier(TargetNode, socket, ShowCurveModifier)) NodeUpdated = true;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(DGUI.Properties.Space());
            }

            GUI.color = initialColor;
        }

        private void UpdateSocketValue(Socket socket, UIConnection value)
        {
            UIConnection.SetValue(socket, value);
            GraphEvent.Send(GraphEvent.EventType.EVENT_NODE_UPDATED, TargetNode.Id);
        }

        private void DrawRenameNodeButton()
        {
            DrawRenameButton(m_popupName.stringValue);
        }

        private void DrawOptions()
        {
            ColorName backgroundColorName = DGUI.Colors.ActionColorName;
            ColorName textColorName = DGUI.Colors.ActionColorName;
            DrawBigTitleWithBackground(Styles.GetStyle(Styles.StyleName.IconAction), UILabels.Actions, backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            GetInfoMessage(ERROR_NO_DRAWER_NAME).Draw(TargetNode.ErrorNoDrawerName, InspectorWidth);
            DGUI.Property.Draw(m_action, UILabels.Action, backgroundColorName, textColorName);
            GUILayout.Space(DGUI.Properties.Space(2));
            DrawPopupNameSelection(m_popupName, UILabels.PopupName, Database, backgroundColorName);
        }

        private void DrawPopupNameSelection(SerializedProperty name, string nameLabel, UIPopupDatabase database, ColorName componentColorName)
        {
            Color color = GUI.color;
            Line.Draw(true, componentColorName, DGUI.Database.NameBackgroundHeight,
                () =>
                {
                    List<string> items = database.PopupNames;
                    int selectedIndex = items.IndexOf(name.stringValue);
                    GUILayout.BeginVertical();
                    {
                        Label.Draw(nameLabel, Size.S, TextAlign.Left, componentColorName); //draw 'Name' label
                        GUILayout.BeginHorizontal(GUILayout.Height(Properties.SingleLineHeight));
                        {
                            GUI.color = Colors.GetDColor(componentColorName).Light.WithAlpha(GUI.color.a);
                            EditorGUI.BeginChangeCheck();
                            selectedIndex = EditorGUILayout.Popup(selectedIndex, items.ToArray()); //draw name selector (dropdown field)
                            if(EditorGUI.EndChangeCheck()) name.stringValue = items[selectedIndex];
                            GUI.color = color;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndVertical();
                });
        }
    }
}