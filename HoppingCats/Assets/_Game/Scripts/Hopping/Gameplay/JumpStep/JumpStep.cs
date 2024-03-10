using UnityEngine;

public class JumpStep : MonoBehaviour
{
    public JumpStepData data;

    private bool deadJump;
    public bool DeadJump => deadJump;

    public Item item;

    public JumpStep(JumpStepData data)
    {
        this.data = data;
    }
    public void SetData(JumpStepData data)
    {
        this.data = data;
    }
    private void Start()
    {
        CreateItem();
    }
    private void CreateItem()
    {
        if(data.config.safeJumpType && ShouldExecuteRandomly(.1f))
        {
            item.gameObject.SetActive(true);
        }        
    }

    protected virtual void Active()
    {

    }

    protected virtual void Deactive()
    {

    }

    protected virtual void ApplyJumpStep()
    {

    }

    bool ShouldExecuteRandomly(float percentage)
    {
        float randomNumber = Random.value;
        return randomNumber <= percentage;
    }

}
