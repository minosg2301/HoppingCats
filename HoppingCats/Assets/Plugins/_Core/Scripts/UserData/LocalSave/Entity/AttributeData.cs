using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public class AttributeData
    {
        [NonSerialized] public int id;
        [NonSerialized] public string name;
        [NonSerialized] public bool sync;
        [SerializeField] private AttributeType type;
        [SerializeField] private string text;
        [SerializeField] private Sprite sprite;

        public AttributeType Type => type;

        public AttributeData(AttributeValue initValue)
        {
            type = initValue.Type;

            if(initValue.Type == AttributeType.String) text = initValue.AsString;
            else if(initValue.Type == AttributeType.Sprite) sprite = initValue.AsSprite;
            else throw new ArgumentException("initValue only Sprite and String type");
        }

        public static implicit operator AttributeValue(AttributeData data)
        {
            if(data.type == AttributeType.String) return data.text;
            if(data.type == AttributeType.Sprite) return data.sprite;
            return new AttributeValue();
        }

        /// <summary>
        /// The string value for this attribute if its type is 'String'.
        /// </summary>
        /// <param name="value">AttributeValue to retrieve string value from.</param>
        public static implicit operator string(AttributeData value)
        {
            if(value.type == AttributeType.String) return value.text;

            throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a string, because it contains a {value.type} value.");
        }

        /// <summary>
        /// The sprite value for this attribute if its type is 'Sprite'.
        /// </summary>
        /// <param name="value">AttributeValue to retrieve sprite value from.</param>
        public static implicit operator Sprite(AttributeData value)
        {
            if(value.type == AttributeType.Sprite) return value.sprite;

            throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a sprite, because it contains a {value.type} value.");
        }

        internal void SetValue(Sprite value)
        {
            if(type != AttributeType.Sprite) throw new ArgumentException("SetValue for Sprite type only");

            sprite = value;
        }

        internal void SetValue(string value)
        {
            if(type != AttributeType.String) throw new ArgumentException("SetValue for string type only");

            text = value;
        }
    }
}