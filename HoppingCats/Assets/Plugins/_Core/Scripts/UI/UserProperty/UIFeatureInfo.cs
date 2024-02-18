using Doozy.Engine.UI;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace moonNest
{
    public class UIFeatureInfo : MonoBehaviour
    {
        /// <summary>
        /// UI Node to update according to Feature
        /// </summary>
        public GameObject uiNode;

        /// <summary>
        /// Node will displayed when feature is locked
        /// </summary>
        public GameObject lockNode;

        /// <summary>
        /// Which buttons will be disable when feature locked
        /// </summary>
        public UIButton[] buttons;

        /// <summary>
        /// Feature Id need to update UI
        /// </summary>
        public FeatureId featureId;

        /// <summary>
        /// Hide gameobject while feature is locked
        /// </summary>
        public bool hideOnLocked = true;

        /// <summary>
        /// Description for locked
        /// </summary>
        public TextMeshProUGUI lockDescriptionText;

        private Localize _lockDescriptionLoc;
        public Localize LockDescriptionLoc
        {
            get { if (!_lockDescriptionLoc && lockDescriptionText) _lockDescriptionLoc = lockDescriptionText.GetComponent<Localize>(); return _lockDescriptionLoc; }
        }

        Feature feature;

        void OnValidate()
        {
            var feature = GameDefinitionAsset.Ins.features.Find(f => f.id == featureId);
            gameObject.name = feature ? $"{feature.name} - Feature" : "No Feature";
        }

        void Reset()
        {
            if (!lockDescriptionText) lockDescriptionText = GetComponent<TextMeshProUGUI>();
        }

        void OnEnable()
        {
            feature = UserData.Ins.FindFeature(featureId);
            if (feature != null)
                UserData.Ins.Subscribe(feature.ToString(), OnFeatureUpdated);
        }

        void OnDisable()
        {
            if (feature != null)
                UserData.Ins.Unsubscribe(feature.ToString(), OnFeatureUpdated);
        }

        protected virtual void OnFeatureUpdated(BaseUserData obj)
        {
            uiNode.SetActive(true);

            if (feature.Locked) uiNode.SetActive(!hideOnLocked);
            if (lockNode) lockNode.SetActive(feature.Locked);

            foreach (var button in buttons)
                button.Interactable = !feature.Locked;

            if (LockDescriptionLoc) LockDescriptionLoc.Term = feature.Config.description;
            else if (lockDescriptionText) lockDescriptionText.text = feature.Config.description;
        }
    }
}