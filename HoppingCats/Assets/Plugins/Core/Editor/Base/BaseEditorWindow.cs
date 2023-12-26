using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moonNest
{
    public abstract class BaseEditorWindow : EditorWindow
    {
        private bool firstEnterAfterFocus;
        private Vector2 scrollPos;
        public bool disabled = false;
        private bool drawingWindow;
        private DateTime lastTime;
        private double autoSaveInterval = 60;

        protected virtual void OnEnable()
        {
            disabled = false;
            lastTime = DateTime.UtcNow;
        }

        protected virtual void OnDisable() { }

        private void OnFocus()
        {
            firstEnterAfterFocus = true;
        }

        private void OnGUI()
        {
            if (firstEnterAfterFocus)
            {
                RemoveInputFocus();
                firstEnterAfterFocus = false;
            }

            if (Event.current.commandName == "UndoRedoPerformed") OnUndoRedoPerformed();

            OnDrawToolbar();

            Draw.Space();

            scrollPos = Draw.BeginScrollView(scrollPos);

            EditorGUI.BeginDisabledGroup(disabled || drawingWindow);
            OnDraw();
            EditorGUI.EndDisabledGroup();

            Draw.EndScrollView();

            BeginWindows();
            drawingWindow = OnDrawWindow();
            EndWindows();

            TrackChange();
            AutoSave();
        }

        protected virtual void OnUndoRedoPerformed() { }
        protected virtual void OnDrawToolbar() { }
        protected virtual bool OnDrawWindow() { return false; }
        protected abstract void OnDraw();
        protected abstract Object GetTarget();

        private static void RemoveInputFocus()
        {
            GUIUtility.keyboardControl = 0;
        }

        private void TrackChange()
        {
            //if we typed in other values in the editor window,
            //we need to repaint it in order to display the new values
            if (GUI.changed)
            {
                //we have to tell Unity that a value of our script has changed
                //http://unity3d.com/support/documentation/ScriptReference/EditorUtility.SetDirty.html
                if (GetTarget() != null) EditorUtility.SetDirty(GetTarget());

                //repaint editor GUI window
                Repaint();
            }
        }

        private void AutoSave()
        {
            if (DateTime.UtcNow.Subtract(lastTime).TotalSeconds >= autoSaveInterval)
            {
                lastTime = DateTime.UtcNow;
                AssetDatabase.SaveAssets();
            }
        }
    }
}