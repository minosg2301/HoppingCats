// Copyright (c) 2015 - 2019 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using Doozy.Engine.Nody.Attributes;
using Doozy.Engine.Nody.Connections;
using Doozy.Engine.Nody.Models;
using Doozy.Engine.UI.Base;
using Doozy.Engine.UI.Connections;
using Doozy.Engine.Utils;
using System;
using UnityEngine;

namespace Doozy.Engine.UI.Nodes
{
    /// <summary>
    ///     The UIDrawer Node opens, closes or toggles a target UIDrawer (identified by name) and jumps instantly to the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [NodeMenu(MenuUtils.UIPopupNode_CreateNodeMenu_Name, MenuUtils.UIPopupNode_CreateNodeMenu_Order)]
    public class UIPopupNode : Node
    {
#if UNITY_EDITOR
        public override bool HasErrors { get { return base.HasErrors || ErrorNoDrawerName; } }
        public bool ErrorNoDrawerName;
#endif

        [NonSerialized] private bool m_timerIsActive;
        [NonSerialized] private double m_timerStart;
        [NonSerialized] private float m_timeDelay;
        [NonSerialized] private Socket m_activeSocketAfterTimeDelay;

        public float TimerProgress { get { return Mathf.Clamp01(m_timerIsActive ? (float)(Time.realtimeSinceStartup - m_timerStart) / m_timeDelay : 0f); } }

        public enum PopupAction
        {
            Open,
            Close
        }

        public bool AddInQueue = false;
        public string PopupName = UIPopup.DefaultPopupName;
        public PopupAction Action = PopupAction.Open;

        public override void OnCreate()
        {
            base.OnCreate();
            CanBeDeleted = true;
            SetNodeType(NodeType.General);
            SetName(UILabels.UIPopupNodeName);
            SetAllowDuplicateNodeName(true);
        }

        public override void AddDefaultSockets()
        {
            base.AddDefaultSockets();
            AddInputSocket(ConnectionMode.Multiple, typeof(PassthroughConnection), false, false);
            AddOutputSocket(ConnectionMode.Override, typeof(PassthroughConnection), false, false);
            UIConnection value = UIConnection.GetValue(OutputSockets[0]);
            value.Trigger = UIConnectionTrigger.ButtonClick;
            value.ButtonCategory = NamesDatabase.GENERAL;
            value.ButtonName = NamesDatabase.BACK;
            UIConnection.SetValue(OutputSockets[0], value);
        }

        public override void CopyNode(Node original)
        {
            base.CopyNode(original);
            var node = (UIPopupNode)original;
            PopupName = node.PopupName;
            Action = node.Action;
        }

        public override void Activate(Graph portalGraph)
        {
            if(m_activated) return;
            base.Activate(portalGraph);
            AddListeners();
        }

        public override void Deactivate()
        {
            if(!m_activated) return;
            base.Deactivate();
            RemoveListeners();
        }

        public override void OnEnter(Node previousActiveNode, Connection connection)
        {
            base.OnEnter(previousActiveNode, connection);
            Activate(ActiveGraph);
            LookForTimeDelay();
            ExecuteActions();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if(!m_timerIsActive) return;
            if(TimerProgress < 1) return;
            m_timerIsActive = false;
            m_timerStart = Time.realtimeSinceStartup;
            ActivateOutputSocketInputNode(m_activeSocketAfterTimeDelay);
        }

        public override void OnExit(Node nextActiveNode, Connection connection)
        {
            base.OnExit(nextActiveNode, connection);
            Deactivate();
        }

        private void AddListeners()
        {
            Message.AddListener<UIButtonMessage>(OnButtonMessage);
            Message.AddListener<GameEventMessage>(OnGameEventMessage);
        }

        private void RemoveListeners()
        {
            Message.RemoveListener<UIButtonMessage>(OnButtonMessage);
            Message.RemoveListener<GameEventMessage>(OnGameEventMessage);
        }

        private void OnButtonMessage(UIButtonMessage message)
        {
            if(ActiveGraph != null && !ActiveGraph.Enabled) return;
            if(OutputSockets == null || OutputSockets.Count == 0) return;

            UIConnectionTrigger trigger;
            switch(message.Type)
            {
                case UIButtonBehaviorType.OnClick:
                    trigger = UIConnectionTrigger.ButtonClick;
                    break;
                case UIButtonBehaviorType.OnDoubleClick:
                    trigger = UIConnectionTrigger.ButtonDoubleClick;
                    break;
                case UIButtonBehaviorType.OnLongClick:
                    trigger = UIConnectionTrigger.ButtonLongClick;
                    break;
                case UIButtonBehaviorType.OnRightClick:
                case UIButtonBehaviorType.OnPointerEnter:
                case UIButtonBehaviorType.OnPointerExit:
                case UIButtonBehaviorType.OnPointerDown:
                case UIButtonBehaviorType.OnPointerUp:
                case UIButtonBehaviorType.OnSelected:
                case UIButtonBehaviorType.OnDeselected:
                    return;
                default: throw new ArgumentOutOfRangeException();
            }

            foreach(Socket socket in OutputSockets)
            {
                if(!socket.IsConnected) continue;
                UIConnection value = UIConnection.GetValue(socket);
                if(value.Trigger != trigger) continue;
                if(!value.ButtonName.Equals(message.Button != null ? message.Button.ButtonName : message.ButtonName)) continue;
                ActivateOutputSocketInputNode(socket);
                break;
            }
        }

        private void OnGameEventMessage(GameEventMessage message)
        {
            if(ActiveGraph != null && !ActiveGraph.Enabled) return;
            if(OutputSockets == null || OutputSockets.Count == 0) return;

            foreach(Socket socket in OutputSockets)
            {
                if(!socket.IsConnected) continue;
                UIConnection value = UIConnection.GetValue(socket);
                if(value.Trigger != UIConnectionTrigger.GameEvent) continue;
                if(!value.GameEvent.Equals(message.EventName)) continue;
                ActivateOutputSocketInputNode(socket);
                break;
            }
        }

        private void ActivateTimer(float timeDelay, Socket socket)
        {
            m_timerIsActive = true;
            m_timerStart = Time.realtimeSinceStartup;
            m_timeDelay = timeDelay;
            m_activeSocketAfterTimeDelay = socket;
            UseUpdate = true;
        }

        private void ActivateOutputSocketInputNode(Socket socket)
        {
            if(ActiveGraph == null || socket == null) return; //sanity check
            ActiveGraph.SetActiveNodeByConnection(socket.Connections[0]);
        }

        private void LookForTimeDelay()
        {
            m_timerIsActive = false;
            UseUpdate = false;
            if(OutputSockets == null || OutputSockets.Count == 0) return;
            foreach(Socket socket in OutputSockets)
            {
                if(!socket.IsConnected) continue;
                UIConnection value = UIConnection.GetValue(socket);
                if(value.Trigger != UIConnectionTrigger.TimeDelay) continue;
                if(value.TimeDelay < 0) continue; //sanity check
                ActivateTimer(value.TimeDelay, socket);
                break;
            }
        }

        private void ExecuteActions()
        {
            // baontp - added
            // prevent show new popup if current popup is this popup
            if(UIPopupManager.VisiblePopups.Find(popup => popup.PopupName == PopupName)) return;
            // baontp - end

            UIPopup popup = UIPopupManager.ShowPopup(PopupName, AddInQueue, false);
            if(popup)
            {
                popup.HideBehavior.OnFinished.Event.AddListener(OnPopupHide);
            }
        }

        private void OnPopupHide()
        {
            if(ActiveGraph == null) return;
            if(!FirstOutputSocket.IsConnected) return;
            ActiveGraph.SetActiveNodeByConnection(FirstOutputSocket.FirstConnection);
        }

        public override void CheckForErrors()
        {
            base.CheckForErrors();
#if UNITY_EDITOR
            ErrorNoDrawerName = string.IsNullOrEmpty(PopupName.Trim());
#endif
        }
    }
}