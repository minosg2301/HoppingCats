using System;
using UnityEditor;

namespace moonNest
{
    public class ColDrawer<T>
    {
        public string name;
        public float maxWidth;
        public bool editable;
        public Action<T> drawer;
        public Action<T, int> drawerWithIndex;
        public Func<T, T> func;
        public Func<T, int, T> funcWithIndex;
        public Predicate<T> drawCondition;
        public bool enabled = true;
        public AbstractFilterCell<T> filter;

        public ColDrawer(string name, float maxWidth, Action<T> draw, Predicate<T> drawCondition, bool editable = true)
        {
            this.name = name;
            this.maxWidth = maxWidth;
            drawer = draw;
            this.drawCondition = drawCondition;
            this.editable = editable;
        }

        public ColDrawer(string name, float maxWidth, Action<T> draw, bool editable = true)
        {
            this.name = name;
            this.maxWidth = maxWidth;
            drawer = draw;
            this.editable = editable;
        }

        public ColDrawer(string name, float maxWidth, Action<T, int> draw, bool editable = true)
        {
            this.name = name;
            this.maxWidth = maxWidth;
            drawerWithIndex = draw;
            this.editable = editable;
        }

        public ColDrawer(string name, float maxWidth, Func<T, T> draw, bool editable = true)
        {
            this.name = name;
            this.maxWidth = maxWidth;
            func = draw;
            this.editable = editable;
        }

        public ColDrawer(string name, float maxWidth, Func<T, int, T> draw, bool editable = true)
        {
            this.name = name;
            this.maxWidth = maxWidth;
            funcWithIndex = draw;
            this.editable = editable;
        }

        public ColDrawer<T> AddFilter(AbstractFilterCell<T> filterCell)
        {
            filter = filterCell;
            return this;
        }

        internal T OnDraw(T ele, int i, bool forceEnabled = false)
        {
            EditorGUI.BeginDisabledGroup(!editable && !forceEnabled);
            if(drawCondition == null || drawCondition.Invoke(ele))
            {
                drawer?.Invoke(ele);
                drawerWithIndex?.Invoke(ele, i);
                if(func != null) ele = func.Invoke(ele);
                if(funcWithIndex != null) ele = funcWithIndex.Invoke(ele, i);
            }
            else
            {
                Draw.Label("", maxWidth);
            }
            EditorGUI.EndDisabledGroup();
            return ele;
        }
    }

    public class ExpandableColDrawer<T> : ColDrawer<T>
    {
        public string label;
        public Func<T, string> labelGetter;

        public ExpandableColDrawer(string name, string label, float maxWidth, Action<T> draw, bool editable = true)
            : base(name, maxWidth, draw, editable)
        {
            this.label = label;
        }

        public ExpandableColDrawer(string name, Func<T, string> labelGetter, float maxWidth, Action<T> draw, bool editable = true)
            : base(name, maxWidth, draw, editable)
        {
            this.labelGetter = labelGetter;
        }
    }

    public class FilterCell<T, K> : AbstractFilterCell<T>
    {
        readonly Func<K, float, K> drawer;
        readonly Func<T, K, bool> predicate;
        K defaultValue;

        public FilterCell(K defaultValue, Func<K, float, K> drawer, Func<T, K, bool> predicate)
        {
            this.defaultValue = defaultValue;
            this.value = defaultValue;
            this.drawer = drawer;
            this.predicate = predicate;
        }

        public override void Draw(float maxWidth)
        {
            value = drawer.Invoke((K)value, maxWidth);
        }

        public override bool Match(T t)
        {
            return ((K)value).Equals(defaultValue) ? true : predicate.Invoke(t, (K)value);
        }
    }

    public abstract class AbstractFilterCell<T>
    {
        public object value;

        public abstract void Draw(float maxWidth);
        public abstract bool Match(T t);
    }
}