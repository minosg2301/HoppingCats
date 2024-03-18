using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class FindCamera : MonoBehaviour
{
    public string cameraName;

    private Canvas _canvas;
    public Canvas Canvas { get { if (_canvas == null) _canvas = GetComponent<Canvas>(); return _canvas; } }

    void Update()
    {
        if(!Canvas.worldCamera)
        {
            Canvas.worldCamera = Camera.allCameras.Find(camera => camera.name == cameraName);
        }
    }
}