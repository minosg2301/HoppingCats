using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UITutorialFocus : MonoBehaviour
    {
        public Image lockTouchMask;
        public bool zoomIn = true;
        public float zoomDuration = 0.5f;
        public Ease zoomEase = Ease.InQuad;
        public Vector2 padding = new Vector2(20, 20);
        public GameObject unmaskedNode;

        private RectTransform _rect;
        public RectTransform Rect { get { if (!_rect) _rect = GetComponent<RectTransform>(); return _rect; } }

        private Canvas canvas;
        public Canvas Canvas { get { if (!canvas) canvas = GetComponentInParent<Canvas>(); return canvas; } }


        private Image _unmaskedImage;
        public Image UnmaskedImage { get { if (!_unmaskedImage) _unmaskedImage = unmaskedNode.GetComponent<Image>(); return _unmaskedImage; } }


        public bool ShowMaskedFocus { get; set; } = true;

        public bool UseOverridedPadding { get; set; } = false;
        public Vector2 OverridedPadding { get; set; }

        public bool UsePositionOffset { get; set; } = false;
        public Vector3 PositionOffset { get; set; }

        public bool UseAnchorPosOffset { get; set; } = false;
        public Vector2 AnchorPosOffset { get; set; }

        public bool NonForce { get; internal set; }

        public void FocusToTarget(RectTransform target)
        {
            Rect.position = UsePositionOffset ? target.position + PositionOffset : target.position;
            Rect.localScale = target.localScale;
            Rect.pivot = target.pivot;

            var targetSize = target.rect.size + (UseOverridedPadding ? OverridedPadding : padding);

            if (zoomIn)
            {
                lockTouchMask.raycastTarget = true;
                Rect.sizeDelta = Canvas.pixelRect.size;
                Rect.DOSizeDelta(targetSize, zoomDuration)
                    .SetEase(zoomEase)
                    .onComplete = OnZoomInCompleted;
            }
            else
            {
                Rect.sizeDelta = targetSize;
            }

            if (!NonForce)
            {
                unmaskedNode.SetActive(true);
                gameObject.SetActive(true);
            }

            var color = UnmaskedImage.color;
            color.a = ShowMaskedFocus ? 1f : 0f;
            UnmaskedImage.color = color;
        }

        public void FocusToPosition(Vector2 position, Vector2 size)
        {
            Rect.anchoredPosition = UseAnchorPosOffset ? position + AnchorPosOffset : position;
            Rect.localScale = Vector3.one;

            if (zoomIn)
            {
                lockTouchMask.raycastTarget = true;
                Rect.sizeDelta = Canvas.pixelRect.size;
                Rect.DOSizeDelta(size, zoomDuration)
                    .SetEase(zoomEase)
                    .onComplete = OnZoomInCompleted;
            }
            else
            {
                Rect.sizeDelta = size;
            }


            unmaskedNode.SetActive(true);
            gameObject.SetActive(true);

            var color = UnmaskedImage.color;
            color.a = ShowMaskedFocus ? 1f : 0f;
            UnmaskedImage.color = color;
        }

        public void Hide()
        {
            unmaskedNode.SetActive(false);
            gameObject.SetActive(false);
        }

        void OnZoomInCompleted()
        {
            lockTouchMask.raycastTarget = false;
        }

        internal void SetStepConfig(TutorialStep currentStep)
        {
            if (currentStep == null) return;

            zoomIn = !currentStep.skipZoomIn;

            ShowMaskedFocus = !NonForce && (!currentStep.showHandFocus || currentStep.showMaskedFocus);

            UseOverridedPadding = currentStep.overridePadding;
            OverridedPadding = currentStep.zoomPadding;

            UsePositionOffset = currentStep.overridePosOffset;
            PositionOffset = currentStep.zoomPosOffset;

            UseAnchorPosOffset = currentStep.overrideAnchorPosOffset;
            AnchorPosOffset = currentStep.zoomAnchorPosOffset;
        }
    }
}