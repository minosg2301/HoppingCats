using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UITutorialDialog : MonoBehaviour
    {
        [Header("Displayer")]
        public TextMeshProUGUI header;
        public TextMeshProUGUI content;

        [Header("Animator")]
        public Animator animator;
        public string closeTrigger = "Close";

        int closeKey;

        public event Action OnHide = delegate { };

        void Start()
        {
            closeKey = Animator.StringToHash(closeTrigger);
        }

        public void Show(string header, string content)
        {
            this.header.text = header;
            this.content.text = content;

            gameObject.SetActive(true);
        }

        public void HideAfter(float seconds)
        {
            DOVirtual.DelayedCall(seconds, Hide);
        }

        public void Hide()
        {
            if (animator && closeTrigger.Length > 0)
            {
                animator.SetTrigger(closeKey);
            }
            else
            {
                gameObject.SetActive(false);
                OnHide();
            }
        }

        public void CloseAnimationDone()
        {
            gameObject.SetActive(false);
            OnHide();
        }
    }
}