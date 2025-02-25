using UnityEngine;

public class UIPlatform : MonoBehaviour
{
    public Platform data;

    public Item item;

    public UIPlatform(Platform data)
    {
        this.data = data;
    }

    public void SetData(Platform data)
    {
        this.data = data;
    }

    private void Start()
    {
        CreateItem();
    }

    private void CreateItem()
    {
        if (data.config.isSafe && ShouldExecuteRandomly(.1f))
        {
            item.gameObject.SetActive(true);
        }
    }

    protected virtual void Active()
    {
        // Change Type of Jump step (sprite, data)
    }

    protected virtual void Deactive()
    {

    }

    protected virtual void ApplyJumpStep()
    {
        // Temp jump
        // Apply item
        // Apply dead jump
    }

    bool ShouldExecuteRandomly(float percentage)
    {
        float randomNumber = Random.value;
        return randomNumber <= percentage;
    }

}
