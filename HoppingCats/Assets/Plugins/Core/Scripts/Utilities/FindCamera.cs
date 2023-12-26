using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class FindCamera : MonoBehaviour
{
    public string cameraName;

    private Canvas _canvas;
    public Canvas Canvas { get { if (_canvas == null) _canvas = GetComponent<Canvas>(); return _canvas; } }

    void Update()
    {
        if (!Canvas.worldCamera)
        {
            var cam = Camera.allCameras.Find(camera => camera.name == cameraName);
            if (cam)
            {
                Debug.Log("Cam name: " + cam.name);
                Canvas.worldCamera = cam;
            }
        }
    }
}
