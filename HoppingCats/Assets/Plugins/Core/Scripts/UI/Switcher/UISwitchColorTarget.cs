using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Image))]
    public class UISwitchColorTarget : UISwitchTarget
    {
        public Color on = Color.white;
        public Color off = Color.red;

        private Image _image;
        public Image Image { get { if(_image == null) _image = GetComponent<Image>(); return _image; } }

        public override bool On
        {
            get { return base.On; }
            set
            {
                base.On = value;
                Image.color = _on ? on : off;
            }
        }
    }
}