using System;
using UnityEngine;

public class GameInputEvent : MonoBehaviour
{
    public event Action onUpdate = delegate { };

    public void DoActive(bool active)
    {
        enabled = active;
    }

    private void Update()
    {
        onUpdate();
    }
}
