using System;
using UnityEngine;

namespace moonNest
{
    [Serializable]
    public struct AttributeValue
    {
        [SerializeField] private AttributeType type;
        [SerializeField] private string text;
        [SerializeField] private Sprite sprite;
        [SerializeField] private GameObject prefab;
        
        public AttributeType Type => type;

        /// <summary>
        /// Construct a AttributeValue from a string.
        /// </summary>
        /// <param name="text">float value to place into StatValue.</param>
        public static implicit operator AttributeValue(string text) => new AttributeValue
        {
            type = AttributeType.String,
            text = text
        };

        /// <summary>
        /// The string value for this attribute if its type is 'String'.
        /// </summary>
        /// <param name="value">AttributeValue to retrieve string value from.</param>
        public static implicit operator string(AttributeValue value)
        {
            if(value.type == AttributeType.String)
            {
                return value.text;
            }

            throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a string, because it contains a {value.type} value.");
        }

        /// <summary>
        /// Construct a AttributeValue from a sprite.
        /// </summary>
        /// <param name="sprite">float value to place into StatValue.</param>
        public static implicit operator AttributeValue(Sprite sprite) => new AttributeValue
        {
            type = AttributeType.Sprite,
            sprite = sprite
        };

        /// <summary>
        /// The sprite value for this attribute if its type is 'Sprite'.
        /// </summary>
        /// <param name="value">AttributeValue to retrieve sprite value from.</param>
        public static implicit operator Sprite(AttributeValue value)
        {
            if(value.type == AttributeType.Sprite)
            {
                return value.sprite;
            }

            throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a sprite, because it contains a {value.type} value.");
        }

        /// <summary>
        /// Construct a AttributeValue from a GameObject.
        /// </summary>
        /// <param name="text">float value to place into StatValue.</param>
        public static implicit operator AttributeValue(GameObject go) => new AttributeValue
        {
            type = AttributeType.GameObject,
            prefab = go
        };

        /// <summary>
        /// The prefab value for this attribute if its type is 'Prefab'.
        /// </summary>
        /// <param name="value">AttributeValue to retrieve prefab value from.</param>
        public static implicit operator GameObject(AttributeValue value)
        {
            if(value.type == AttributeType.GameObject)
            {
                return value.prefab;
            }

            throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a prefab, because it contains a {value.type} value.");
        }

        public void SetValue(GameObject gameObject)
        {
            if(type == AttributeType.GameObject) prefab = gameObject;

            else throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a GameObject, because it contains a {type} value.");
        }
        public void SetValue(Sprite sprite)
        {
            if(type == AttributeType.Sprite) this.sprite = sprite;

            else throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a Sprite, because it contains a {type} value.");
        }
        public void SetValue(string text)
        {
            if(type == AttributeType.String) this.text = text;

            else throw new InvalidCastException
                ($"Cannot cast this {nameof(AttributeValue)} instance into a Sprite, because it contains a {type} value.");
        }

        public Sprite AsSprite => sprite;
        public GameObject AsPrefab => prefab;
        public string AsString => text;
    }
}