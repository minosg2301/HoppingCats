using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace moonNest
{
    [Serializable]
    public class LuckyBoxes
    {
        [SerializeField] private int id;
        [SerializeField] private LuckyBox[] boxes;
        [SerializeField] private List<int> openedBoxIds = new List<int>();

        public int Id => id;
        public LuckyBox[] Boxes => boxes;

        private LuckyBoxesDetail _detail;
        public LuckyBoxesDetail Detail { get { if(!_detail) _detail = GatchaAsset.Ins.FindNineBoxes(id); return _detail; } }

        public Action<LuckyBox> onBoxOpened;

        public LuckyBoxes(LuckyBoxesDetail detail)
        {
            id = detail.id;
            boxes = detail.boxes.Map(boxDetail => new LuckyBox()).ToArray();
        }

        public LuckyBoxConfig OpenRandomBox()
        {
            List<LuckyBoxConfig> sortedBoxes = Detail.boxes.ToList().FindAll(box => !openedBoxIds.Contains(box.id));
            if(sortedBoxes.Count == 1) return sortedBoxes[0];

            sortedBoxes.SortAsc(box => box.weight);
            int totalWeight = sortedBoxes.Sum(box => box.weight);
            float ran = Random.Range(0f, totalWeight);
            float k = 0;
            foreach(LuckyBoxConfig box in sortedBoxes)
            {
                k += box.weight;
                if(ran < k) return box;
            }
            return sortedBoxes[0];
        }

        public void SetBoxOpened(LuckyBox luckyBox, LuckyBoxConfig boxConfig)
        {
            openedBoxIds.Add(boxConfig.id);
            luckyBox.Open(boxConfig.id);

            UserGatcha.Ins.dirty = true;

            onBoxOpened?.Invoke(luckyBox);
        }
    }

    [Serializable]
    public class LuckyBox
    {
        [SerializeField] private int id;

        public int Id => id;
        public bool Opened => id != -1;

        private LuckyBoxConfig _config;
        public LuckyBoxConfig Config
        {
            get
            {
                LuckyBoxes luckyBoxes = UserGatcha.Ins.LuckyBoxes;
                if(!_config && luckyBoxes != null)
                {
                    _config = luckyBoxes.Detail.boxes.Find(box => box.id == id);
                }
                return _config;
            }
        }

        public LuckyBox()
        {
            id = -1;
        }

        public void Open(int id)
        {
            this.id = id;
        }
    }
}