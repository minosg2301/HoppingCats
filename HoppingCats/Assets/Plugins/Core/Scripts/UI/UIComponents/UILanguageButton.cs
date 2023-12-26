using Doozy.Engine.UI;
using I2.Loc;
using System;
using TMPro;
using UnityEngine;
using moonNest;

public class UILanguageButton : BaseUIData<moonNest.Language>
{
    public UIButton button;
    public TextMeshProUGUI nameText;
    public GameObject highlightNode;

    private Localize _nameLoc;
    public Localize NameLoc { get { if(!_nameLoc && nameText) _nameLoc = nameText.GetComponentInChildren<Localize>(); return _nameLoc; } }

    private bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            if(highlightNode) highlightNode.SetActive(_selected);
        }
    }

    public moonNest.Language Language { get; private set; }

    public Action<UILanguageButton> onClick;

    void Reset()
    {
        if(!button) button = GetComponent<UIButton>();
        if(!nameText) nameText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        if(button) button.OnClick.OnTrigger.Event.AddListener(() => onClick?.Invoke(this));
        if(highlightNode) highlightNode.SetActive(_selected);
    }

    public override void SetData(moonNest.Language language)
    {
        Language = language;
        NameLoc.Term = language.text;
    }
}
