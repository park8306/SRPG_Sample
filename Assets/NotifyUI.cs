using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NotifyUI : SingletonMonoBehavior<NotifyUI>
{
    Text contentsText;
    CanvasGroup canvasGroup;

    protected override void OnInit()
    {
        contentsText = transform.Find("ContentsText").GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    internal void Show(string text, float visibleTime = 3)
    {
        base.Show();
        canvasGroup.DOKill();
        canvasGroup.alpha = 1;
        contentsText.text = text;

        canvasGroup.DOFade(0, 1f).SetDelay(visibleTime).OnComplete(Close);

    }
}
