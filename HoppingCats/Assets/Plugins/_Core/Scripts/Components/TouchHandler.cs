using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace moonNest
{
    public class TouchHandler : MonoBehaviour
    {
        public bool freeTouch = false;
        public float sensitive = 1f;
        public GameObject target;
        public bool usePlane;
        public bool useMainCamera = true;
        public new string camera;

        public UnityEvent clickEvent;

        [Header("Ignore Pointer Over UI")]
        [LabelOverride("Ignore")]
        public bool ignorePointerOverUI = false;

        public delegate void TouchEvent(TouchData touchData);
        public TouchEvent onTouchBegan = delegate { };
        public TouchEvent onTouchMoved = delegate { };
        public TouchEvent onTouchEnded = delegate { };
        public TouchEvent onClick = delegate { };

        bool isTouched = false;

        public Ray Ray { get; private set; }
        public bool Drag { get; private set; }

        Vector3 touchPosition;
        Vector2 touchPoint;

        private Camera _camera;
        public Camera Camera
        {
            get
            {
                if (!_camera)
                {
                    _camera = useMainCamera || camera.Length == 0
                        ? Camera.main :
                        Camera.allCameras.Find(c => c.name == camera);
                }

                return _camera;
            }
        }

        void Update()
        {
            if (!ignorePointerOverUI && IsPointerOverGameObject()) return;

            ListenTouch();
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_WEBGL
            ListenMouse();
#endif
        }

        static bool IsPointerOverGameObject()
        {
            //check mouse
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            //check touch
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return true;
            }

            return false;
        }

        private void ListenMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Drag = false;
                HandleTouchDown(Input.mousePosition);
            }
            else if (isTouched && Input.GetMouseButton(0))
            {
                HandleTouchMoved(Input.mousePosition);
            }
            else if (isTouched && Input.GetMouseButtonUp(0))
            {
                HandleTouchEnded(Input.mousePosition);
            }
        }

        private void ListenTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    HandleTouchDown(touch.position);
                }
                else if (isTouched && touch.phase == TouchPhase.Moved)
                {
                    HandleTouchMoved(touch.position);
                }
                else if (isTouched && touch.phase == TouchPhase.Ended)
                {
                    HandleTouchEnded(touch.position);
                }
            }
        }

        private void HandleTouchDown(Vector2 screenPoint)
        {
            Ray ray = Camera.ScreenPointToRay(screenPoint);
            GameObject _target = target == null ? gameObject : target;
            if ((Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, 1 << _target.layer) && hit.collider.gameObject == _target)
                || freeTouch)
            {
                TouchData touchData = new TouchData
                {
                    touchPoint = screenPoint,
                    position = _target.transform.position,
                    ray = ray
                };

                touchPoint = screenPoint;
                touchPosition = freeTouch ? _target.transform.position : hit.point;
                isTouched = true;
                onTouchBegan(touchData);

                TouchPlane.Ins.transform.position = touchPosition;
            }
        }

        private void HandleTouchMoved(Vector2 screenPoint)
        {
            Ray ray = Camera.ScreenPointToRay(screenPoint);
            if (touchPoint != screenPoint)
            {
                Drag = true;
                touchPoint = screenPoint;

                var hits = Physics.RaycastAll(ray, 400);
                if (hits.Length > 0)
                {
                    var hit = usePlane
                        ? hits.Find(hit => hit.collider.gameObject == TouchPlane.Ins.gameObject)
                        : hits.Find(hit => hit.collider.gameObject == target);
                    if (hit.collider != null)
                    {
                        var hitPosition = hit.point;
                        var deltaTranslation = (hitPosition - touchPosition) * sensitive;
                        TouchData touchData = new TouchData
                        {
                            touchPoint = screenPoint,
                            position = hitPosition,
                            ray = ray,
                            deltaTranslation = deltaTranslation
                        };
                        touchPosition = hitPosition;
                        onTouchMoved(touchData);
                    }
                }
            }
        }

        private void HandleTouchEnded(Vector2 screenPoint)
        {
            Ray ray = Camera.ScreenPointToRay(screenPoint);
            var hits = Physics.RaycastAll(ray, 200);
            if (hits.Length > 0)
            {
                var hit = usePlane
                    ? hits.Find(hit => hit.collider.gameObject == TouchPlane.Ins.gameObject)
                    : hits.Find(hit => hit.collider.gameObject == target);
                if (hit.collider != null)
                {
                    var hitPosition = hit.point;
                    var deltaTranslation = (hitPosition - touchPosition) * sensitive;
                    TouchData touchData = new TouchData
                    {
                        touchPoint = screenPoint,
                        position = hitPosition,
                        ray = ray,
                        deltaTranslation = deltaTranslation
                    };
                    onTouchEnded(touchData);
                    isTouched = false;
                    if (!Drag)
                    {
                        onClick(touchData);
                        clickEvent.Invoke();
                    }
                }
            }
        }

        public class TouchData
        {
            public Vector2 touchPoint;
            public Vector3 position;
            public Vector3 deltaTranslation;
            public Ray ray;
        }
    }
}