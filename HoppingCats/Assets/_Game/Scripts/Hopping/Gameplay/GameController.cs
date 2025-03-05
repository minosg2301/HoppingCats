using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlatformManager platformManager;
    public CatController cat;

    [Header("Properties")]
    public Vector3 startCatPos;

    private int maxTempJump = 1;
    private List<JumpType> tempJumps = new();

    private bool isInitstialized = false;

    public bool IsIngame
    {
        get
        {
            return GameStateManager.Ins.CurrentGameState != GameState.LOBBY &&
                GameStateManager.Ins.CurrentGameState != GameState.LOSE;
        }
    }

    private void Awake()
    {
        GameEventManager.Ins.OnSetupLevel += OnSetupLevel;

        if (!isInitstialized)
        {
            OnSetupLevel();
            isInitstialized = true;
        }
    }
    

    private void Update()
    {
        if (!IsIngame) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            DoMove(JumpType.Left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            DoMove(JumpType.Right);
        }

        //Mouse
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition = Input.mousePosition;
            CheckScreenTouch(touchPosition);
        }
        //Mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            CheckScreenTouch(touchPosition);
        }

        if (!cat.IsJumping && tempJumps.Count > 0)
        {
            DoMove(tempJumps[0]);
        }
    }

    #region puplic methods
    public void LoseHandle()
    {
        GameStateManager.Ins.ChangeGameState(GameState.LOSE);
        LosePopup.Show();
    }

    #endregion

    private void OnSetupLevel()
    {
        GameStateManager.Ins.ChangeGameState(GameState.WAITING);
        platformManager.Clear();
        platformManager.InitPlatforms();
        cat.transform.position = startCatPos;
    }

    private void CheckScreenTouch(Vector2 position)
    {
        float screenWidth = Screen.width;
        if (position.x < screenWidth / 2) 
        {
            DoMove(JumpType.Left);
        }
        else 
        {
            DoMove(JumpType.Right);
        }
    }

    private void DoMove(JumpType moveType)
    {
        if (GameStateManager.Ins.CurrentGameState != GameState.INGAME)
        {
            GameStateManager.Ins.ChangeGameState(GameState.INGAME);
            GameEventManager.Ins.OnStartGame();
        }

        if (cat.IsJumping)
        {
            if (tempJumps.Count < maxTempJump)
            {
                tempJumps.Add(moveType);
            }
            return;
        }
        else 
        {
            if (tempJumps.Count > 0)
            {
                tempJumps.RemoveAt(0);
            }
        };

        cat.DoJump(moveType, 
            ()=> { 
                //start jump
            },
            ()=> {
                //end jump
                GenerateNextJumpStep(moveType == JumpType.Left);
            });
    }

    private void GenerateNextJumpStep(bool moveLeft)
    {
        platformManager.RemoveFirstPlatform();
        platformManager.SpawnNextPlatforms(moveLeft);
    }

    
}
