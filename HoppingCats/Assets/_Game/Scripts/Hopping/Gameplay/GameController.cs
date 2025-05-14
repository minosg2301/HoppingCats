using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Ins;

    public CameraFollow cameraFollow;
    public PlatformManager platformManager;
    public CatController cat;
    public GameInputEvent gameInputEvent;

    [HideInInspector] public bool isHoverUI;

    [Header("Properties")]
    public Vector3 startCatPos;

    private int maxTempJump = 1;
    private List<JumpType> tempJumps = new();
    private bool isInitstialized = false;

    public bool pausing { get; private set; } = false;

    public bool IsIngame
    {
        get
        {
            return GameStateManager.Ins.CurrentGameState == GameState.INGAME ||
                GameStateManager.Ins.CurrentGameState == GameState.WAITING;
        }
    }

    private void Awake()
    {
        if (!Ins) Ins = this;
        //gameInputEvent.DoActive(false);
        gameInputEvent.onUpdate += OnUpdate;
        GameEventManager.Ins.OnSetupLevel += OnSetupLevel;
        if (!isInitstialized)
        {
            OnSetupLevel();
            isInitstialized = true;
        }
    }

    private void OnUpdate()
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
            Debug.Log("----- click ");
            Vector2 touchPosition = Input.mousePosition;
            CheckScreenTouch(touchPosition);
        }
        //Mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("----- click ");
            Vector2 touchPosition = Input.GetTouch(0).position;
            CheckScreenTouch(touchPosition);
        }

        if (!cat.IsJumping && tempJumps.Count > 0)
        {
            DoMove(tempJumps[0]);
        }
    }

    #region puplic methods
    public void Pause(bool pause)
    {
        pausing = pause;
        gameInputEvent.DoActive(!pause);
    }

    public void LoseHandle()
    {
        gameInputEvent.DoActive(false);
        GameStateManager.Ins.ChangeGameState(GameState.LOSEGAME);
        GameEventManager.Ins.OnGameLose();
        cat.animationController.DoDie();

        DOVirtual.DelayedCall(1f, () => {
            LosePopup.Show();
        });
    }
    #endregion

    private void OnSetupLevel()
    {
        GameStateManager.Ins.ChangeGameState(GameState.WAITING);
        LevelManager.Ins.InitLevel();
        platformManager.Clear();
        platformManager.InitPlatforms();

        if (cat)
        {
            Destroy(cat.gameObject);
            cat = null;
        }

        var catInst = Instantiate(SkinManager.Ins.CurrentSkinConfig.catPrefab, transform);
        cat = catInst;

        cameraFollow.DoFollow(cat.transform);
        cat.transform.position = startCatPos;
        cat.animationController.DoIdle();
        pausing = false;
        gameInputEvent.DoActive(true);
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
                LevelManager.Ins.UpdateLevel();
            });
    }

    private void GenerateNextJumpStep(bool moveLeft)
    {
        platformManager.RemovePlatforms();
        platformManager.SpawnNextPlatforms(moveLeft);
    }
}
