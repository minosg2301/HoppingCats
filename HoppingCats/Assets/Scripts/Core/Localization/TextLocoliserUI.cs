using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocoliserUI : MonoBehaviour
{
    TextMeshProUGUI textField;
    public string key;


    private void Start()
    {
        UpdateLanguage();
    }

    public void UpdateLanguage()
    {
        textField = GetComponent<TextMeshProUGUI>();
        string value = LocalizationSystem.GetLocalisedValue(key);
        textField.text = value;
    }

}