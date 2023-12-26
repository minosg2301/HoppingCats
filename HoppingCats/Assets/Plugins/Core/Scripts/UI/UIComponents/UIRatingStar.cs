using Doozy.Engine.UI;
using System;
using UnityEngine;

namespace moonNest
{
    [RequireComponent(typeof(UIButton))]
    public class UIRatingStar : MonoBehaviour
    {
        public int star;
        public GameObject starNode;
        public event Action<int> OnClick = delegate { };

        void Start()
        {
            GetComponent<UIButton>().OnClick.OnTrigger.Event.AddListener(() => OnClick(star));
        }
    }
}