using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.Events;

namespace moonNest
{
    public class UIRating : MonoBehaviour
    {
        public int initStar = 4;
        public int openStoreStar = 4;
        public UIRatingStar[] stars;
        public UIButton ratingButton;
        public UnityEvent<int> onRated;

        int currentRating = 0;

        void Start()
        {
            stars.ForEach(starNode => starNode.OnClick += OnStartSelect);
            ratingButton.OnClick.OnTrigger.Event.AddListener(DoRate);
            OnStartSelect(initStar);
        }

        void DoRate()
        {
            if (currentRating > openStoreStar) ApplicationExt.OpenStoreReview();

            onRated.Invoke(currentRating);
        }

        void OnStartSelect(int star)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].starNode.SetActive(stars[i].star <= star);
            }
            currentRating = star;
        }
    }
}
