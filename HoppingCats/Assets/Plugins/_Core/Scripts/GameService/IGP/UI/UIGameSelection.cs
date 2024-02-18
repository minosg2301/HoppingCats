using UnityEngine;

namespace moonNest
{
    public class UIGameSelection : MonoBehaviour
    {
        public UIGameSelectionInfo game1;
        public UIGameSelectionInfo game2;

        bool fechtDoneListenning;

        void OnEnable()
        {
            if (IGP.Fetching)
            {
                if (fechtDoneListenning) return;

                fechtDoneListenning = true;
                game1.gameObject.SetActive(false);
                game2.gameObject.SetActive(false);
                IGP.OnFetchDone += OnFetchDone;
                return;
            }

            UpdateUI();
        }

        void OnDisable()
        {
            fechtDoneListenning = false;
            if (IGP.Fetching)
                IGP.OnFetchDone -= OnFetchDone;
        }

        void UpdateUI()
        {
            if (!IGP.HaveGameSelection)
            {
                game1.gameObject.SetActive(false);
                game2.gameObject.SetActive(false);
                return;
            }

            game1.gameObject.SetActive(true);
            game2.gameObject.SetActive(true);

            var gameInfo1 = IGP.GameSelectionInfos.Random();
            game1.SetData(gameInfo1);

            var gameInfo2 = IGP.GameSelectionInfos.Random(gameInfo1);
            game2.SetData(gameInfo2);
        }

        void OnFetchDone()
        {
            fechtDoneListenning = false;
            IGP.OnFetchDone -= OnFetchDone;
            UpdateUI();            
        }
    }
}