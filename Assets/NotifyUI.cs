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
        canvasGroup.DOKill();   // 트윈 중지
        canvasGroup.alpha = 1;  // 알파를 1로 설정
        contentsText.text = text;

        canvasGroup.DOFade(0, 1f).SetDelay(visibleTime).OnComplete(Close);

    }
}
