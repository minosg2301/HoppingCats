using UnityEngine;

namespace moonNest
{
    public class Label
    {
        private GUIContent content = new GUIContent();

        private string _text = "";
        private Vector2 _size = new Vector2();

        public string Text { get { return _text; } set { UpdateText(value); } }
        public float x => _size.x;

        private void UpdateText(string value)
        {
            _text = value;
            content.text = _text;
            _size = GUI.skin.label.CalcSize(content);
        }
    }
}