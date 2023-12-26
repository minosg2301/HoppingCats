using UnityEngine;

namespace moonNest
{
    public class UILocation : MonoBehaviour
    {
        public LocationId location;

        void OnEnable()
        {
            CoreHandler.EnterLocation(location);
        }

        void OnDisable()
        {
            CoreHandler.ExitLocation(location);
        }
    }
}