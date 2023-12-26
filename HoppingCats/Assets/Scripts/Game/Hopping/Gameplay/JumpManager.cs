using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpManager : MonoBehaviour
{
    public List<List<JumpStep>> JumpStepContainer;
    public JumpStep currentJumpStep;


}

public class JumpStep
{
    public JumpConfig config;
    public Vector2 jumpPosition;
}