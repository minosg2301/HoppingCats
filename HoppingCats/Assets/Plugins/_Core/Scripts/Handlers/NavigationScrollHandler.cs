using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationScrollHandler : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform contentContainer;

    protected List<GameObject> children;

    void Start()
    {
        children = contentContainer.gameObject.FindAllChildren();
        foreach (var child in children) AddEvent(child);
    }

    void Reset()
    {
        if (!scrollRect)
        {
            scrollRect = GetComponent<ScrollRect>();
            contentContainer = scrollRect.content;
        }
        children = contentContainer.gameObject.FindAllChildren();
        foreach (var child in children)
        {
            if (!child.TryGetComponent<NavigationScrollFocusItem>(out _))
                child.AddComponent<NavigationScrollFocusItem>();
        }
    }

    protected void AddEvent(GameObject go)
    {
        go.GetComponent<NavigationScrollFocusItem>().unityEvent.AddListener(DoAction);
    }

    protected void DoAction(RectTransform rect)
    {
        scrollRect.SmoothScrollTo(rect);
    }
}
