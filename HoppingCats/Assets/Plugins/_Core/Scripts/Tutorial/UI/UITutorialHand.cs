using DG.Tweening;
using UnityEngine;

namespace moonNest
{
    public class UITutorialHand : MonoBehaviour
    {
        public Animator animator;
        public Vector2 positionOffset = Vector2.zero;
        public Vector2 anchorOffset = Vector2.zero;
        public string touchBoolKey = "Touch";
        public string dragBoolKey = "Drag";
        public string dragEndTriggerKey = "DragEnd";

        private RectTransform _rect;
        public RectTransform Rect { get { if (!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

        private Vector2 dragFrom;
        private Vector2 dragTo;
        private float dragDuration;

        private bool onStartDrag = false;

        private Tween doMoveTween;

        void OnValidate()
        {
            gameObject.name = "UITutorialHand";
        }

        public void Show(Transform button)
        {
            transform.position = button.position + positionOffset.ToVector3(0);
            Play();
        }

        public void Show(Vector2 position)
        {
            Rect.anchoredPosition = position + anchorOffset;
            Play();
        }

        public void PlayDrag(Vector2 from, Vector2 to, float duration)
        {
            gameObject.SetActive(true);
            animator.gameObject.SetActive(true);
            animator.SetBool(dragBoolKey, true);

            dragFrom = from;
            dragTo = to;
            dragDuration = duration;
            Rect.anchoredPosition = dragFrom + anchorOffset;
        }

        public void OnDragStart()
        {
            if (!onStartDrag)
            {
                onStartDrag = true;
                doMoveTween = Rect.DOAnchorPos(dragTo + anchorOffset, dragDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    animator.SetTrigger(dragEndTriggerKey);
                    Rect.anchoredPosition = dragFrom + anchorOffset;
                    onStartDrag = false;
                });

            }
        }

        public void Play()
        {
            doMoveTween?.Kill();

            gameObject.SetActive(true);
            animator.gameObject.SetActive(true);
            animator.SetBool(touchBoolKey, true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            animator.SetBool(dragBoolKey, false);
            animator.SetBool(touchBoolKey, false);
            animator.gameObject.SetActive(false);
        }
    }

}