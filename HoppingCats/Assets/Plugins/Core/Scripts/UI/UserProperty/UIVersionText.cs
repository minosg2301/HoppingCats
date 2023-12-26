using TMPro;
using UnityEngine;

public class UIVersionText : MonoBehaviour
{
    public TextMeshProUGUI versionTxt;

    void Awake()
    {
        versionTxt.text = "v" + Application.version;
    }
}