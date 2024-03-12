using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPillow : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image icon;

    [Header("Pillow Properties")]
    public bool isCustom = false;
    public float pillowScaleMax = 0;
    public float pillowScaleMin = 0f;
    public float pillowRotateMax = 0f;
    public float pillowRotateMin = 0f;
}
