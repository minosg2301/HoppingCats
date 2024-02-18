using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UIGameRankPoint : MonoBehaviour
    {
        public Image[] stars;

        void Reset()
        {
            if (stars == null || stars.Count() == 0)
            {
                stars = GetComponentsInChildren<Image>();
                foreach (var star in stars)
                {
                    star.type = Image.Type.Filled;
                    star.fillMethod = Image.FillMethod.Horizontal;
                    star.fillOrigin = 0;
                    star.fillAmount = 1;
                }
            }
        }

        internal void SetValue(float value)
        {
            for (int i = 0, count = stars.Count(); i < count; i++)
            {
                if (i + 1 < value)
                    stars[i].fillAmount = 1;
                else if (i < value + 1f)
                    stars[i].fillAmount = value % i;
                else
                    stars[i].fillAmount = 0;
            }
        }
    }
}