using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpManager : MonoBehaviour
{
    public List<List<JumpStep>> jumpSteps;
    public Transform container;

    public void Clear()
    {
        container.RemoveAllChildren();
        jumpSteps.Clear();
    }

    public void InitJumpSteps()
    {

    }
}
