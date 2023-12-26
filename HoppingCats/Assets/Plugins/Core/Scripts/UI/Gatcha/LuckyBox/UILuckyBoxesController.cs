using System.Linq;
using UnityEngine;

namespace moonNest
{
    public class UILuckyBoxesController : MonoBehaviour
    {
        public Transform boxContainer;
        public UIPrice price;
        public UICountDownTime countDownTime;

        protected LuckyBoxes luckyBoxes;
        protected LuckyBoxesDetail luckyBoxesDetail;
        protected bool opening = false;
        protected bool initialized = false;

        readonly UIListContainer<LuckyBox, UILuckyBox> listContainer = new UIListContainer<LuckyBox, UILuckyBox>();

        void Reset()
        {
            gameObject.name = "UILuckyBoxes";
            if(!boxContainer) boxContainer = transform;
        }

        void Start()
        {
            UpdateBoxDetails();
            if(countDownTime) countDownTime.StartWithDuration(UserGatcha.Ins.LastSeconds);
            initialized = true;
        }

        void OnEnable()
        {
            if(initialized) UpdateBoxDetails();
        }

        private void UpdateBoxDetails()
        {
            if(luckyBoxes != UserGatcha.Ins.LuckyBoxes && UserGatcha.Ins.LuckyBoxes != null)
            {
                luckyBoxes = UserGatcha.Ins.LuckyBoxes;
                luckyBoxesDetail = luckyBoxes.Detail;
                listContainer.SetList(boxContainer, luckyBoxes.Boxes.ToList(), ui => ui.onBoxClicked = OnBoxClicked);
                price.SetPrice(luckyBoxesDetail.cost);
            }
        }

        protected virtual void OnBoxClicked(UILuckyBox ui)
        {
            if(price.UserCanPay)
            {
                LuckyBoxConfig boxConfig = luckyBoxes.OpenRandomBox();
                luckyBoxes.SetBoxOpened(ui.LuckyBox, boxConfig);
                ui.UpdateBoxOpened();
            }
        }
    }
}