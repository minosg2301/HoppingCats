using UnityEngine;

namespace moonNest
{
    /// <summary>
    /// Prevent Safe Area from Parent 
    /// Make this node always full screen
    /// </summary>
    public class InvertSafeArea : MonoBehaviour
    {
        public bool useNormal;

        private RectTransform _rectTransform;
        public RectTransform RectTransform { get { if(!_rectTransform) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }

        public SafeArea ParentSafeArea;

        private void Awake()
        {
            if (!ParentSafeArea) ParentSafeArea = GetComponentInParent<SafeArea>();
        }

        void Start()
        {
            if(!ParentSafeArea) return;

            ParentSafeArea.onApplied += Apply;
            Apply();
        }

        private void Apply()
        {
            if (useNormal)
            {
                var parentPanel = ParentSafeArea.Panel;
                Vector2 originSize = parentPanel.rect.size / (parentPanel.anchorMax - parentPanel.anchorMin);
                Vector2 offsetMin = -(originSize * ParentSafeArea.Panel.anchorMin);
                Vector2 offsetMax = originSize * (Vector2.one - parentPanel.anchorMax);
                RectTransform.anchorMin = Vector2.zero;
                RectTransform.anchorMax = Vector2.one;
                if (ParentSafeArea.ApplyBottom) RectTransform.offsetMin = offsetMin;
                if (ParentSafeArea.ApplyTop) RectTransform.offsetMax = offsetMax;
            }
            else
            {
                var parentPanel = ParentSafeArea.Panel;
                Vector2 originSize = parentPanel.rect.size / (parentPanel.anchorMax - parentPanel.anchorMin);
                Vector2 offsetMin = -(originSize * ParentSafeArea.Panel.anchorMin);
                Vector2 offsetMax = originSize * (Vector2.one - parentPanel.anchorMax);
                if (ParentSafeArea.ApplyBottom) RectTransform.offsetMin = offsetMin;
                if (ParentSafeArea.ApplyTop) RectTransform.offsetMax = offsetMax;
                RectTransform.sizeDelta += new Vector2(RectTransform.rect.height, 0);
            }
        }
    }
}