using Doozy.Engine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using moonNest;

public class NavigationScrollFocusItem : BaseNavigationListener
{
    public UnityEvent<RectTransform> unityEvent;

    public override string PrefixName => "Focus";

    protected override void UnregisterListener()
    {
        base.UnregisterListener();
        unityEvent.RemoveAllListeners();
    }

    protected override void InvokeEvent(GameEventMessage message)
    {
        TryGetComponent<FixContentSizeFitter>(out var fixContentSizeFitter);
        TryGetComponent<ContentSizeFitter>(out var contentSizeFitter);

        if (fixContentSizeFitter || contentSizeFitter)
        {
            StartCoroutine(WaitContentFitSize());
        }
        else unityEvent.Invoke(RectTransform);
    }

    IEnumerator WaitContentFitSize()
    {
        var lastRectPos = new Rect();
        while (lastRectPos != RectTransform.rect)
        {
            lastRectPos = RectTransform.rect;
            yield return 0;
        }
        unityEvent.Invoke(RectTransform);
    }
}