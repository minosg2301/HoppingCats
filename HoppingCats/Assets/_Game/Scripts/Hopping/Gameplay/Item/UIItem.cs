using UnityEngine;

public class UIItem : MonoBehaviour
{
    public virtual void ItemTrigger()
    {
        gameObject.SetActive(false);
    }
}
