using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    [RequireComponent(typeof(Image))]
    public class UISwitchImageTarget : UISwitchTarget
    {
        public Sprite on;
        public Sprite off;

        private Image _image;
        public Image Image { get { if(_image == null) _image = GetComponent<Image>(); return _image; } }

        public override bool On
        {
            get { return base.On; }
            set
            {
                base.On = value;
                Image.sprite = _on ? on : off;
            }
        }
    }
}