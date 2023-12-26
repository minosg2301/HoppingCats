using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace moonNest
{
    public class UIUserAttribute : MonoBehaviour, IObserver
    {
        public UserAttributeId attribute;
        public Text target;
        public TextMeshProUGUI tmTarget;

        void Reset()
        {
            if(!tmTarget) tmTarget = GetComponent<TextMeshProUGUI>();
            if(!target) target = GetComponent<Text>();
        }

        void OnValidate()
        {
            if(attribute != -1)
            {
                AttributeDefinition attrDef = UserPropertyAsset.Ins.properties.FindAttribute(attribute);
                gameObject.name = "User Attr - " + (attrDef ? attrDef.name : "");
            }
        }

        void OnEnable()
        {
            if(attribute != -1) UserData.Ins.Subscribe(this, attribute.id.ToString());
        }

        void OnDisable()
        {
            UserData.Ins?.Unsubscribe(this);
        }

        public void OnNotify(IObservable data, string[] scopes)
        {
            string text = UserData.Attr(attribute).AsString;
            if(target) target.text = text;
            if(tmTarget) tmTarget.text = text;
        }
    }
}