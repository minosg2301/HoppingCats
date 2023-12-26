using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace moonNest
{
    public class UILanguageScroller : MonoBehaviour
    {
        public ScrollRect scrollRect;
        public Transform container;

        private readonly UIListContainer<Language, UILanguageButton> listContainer = new UIListContainer<Language, UILanguageButton>();
        private UILanguageButton newLanguageButton;
        private UILanguageButton selectedButton;

        void Start()
        {
            listContainer.SetList(container, GlobalConfig.Ins.languages, ui => ui.onClick = OnLanguageSelected);
            newLanguageButton = selectedButton = listContainer.UIList.Find(ui => ui.Language.code == UserData.Language);
            newLanguageButton.Selected = true;
            newLanguageButton.transform.SetAsFirstSibling();
        }

        private void Reset()
        {
            if(!scrollRect) scrollRect = GetComponent<ScrollRect>();
            if(scrollRect && !container) container = scrollRect.content;
        }

        private void OnLanguageSelected(UILanguageButton languageButton)
        {
            newLanguageButton.Selected = false;
            newLanguageButton = languageButton;
            newLanguageButton.Selected = true;
        }

        public void Confirm()
        {
            if(newLanguageButton != selectedButton)
            {
                LocalizationManager.CurrentLanguage = newLanguageButton.Language.code;
                UserData.Language = newLanguageButton.Language.code;
            }
        }
    }
}