using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlatformManager platformManager;
    public CatController cat;

    private bool isIngame = false;

    private void Awake()
    {
        StartGame();
    }

    private void Update()
    {
        if (!isIngame) return;

        //Handle cat jump
        if (!cat.IsJumping)
        {
            //Key
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
        }
        
    }


    public void StartGame()
    {
        if (isIngame) return;
        InitPlatforms();
        isIngame = true;
    }

    public void EndGame()
    {
        //if (!isIngame) return;
        //isIngame = false;
        StartGame();
    }

    private void InitPlatforms()
    {
        platformManager.Clear();
        platformManager.InitPlatforms();
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
        if (cat.IsJumping) return;
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
