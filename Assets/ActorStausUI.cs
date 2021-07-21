using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorStausUI : SingletonMonoBehavior<ActorStausUI>
{
    Text status;
    Text nickName; 
    RectTransform mPBarGauge;
    RectTransform mPBar;
    RectTransform hPBarGauge;
    RectTransform hPBar;

    Image mPBarGaugeImage;
    Image hPBarGaugeImage;


    internal void Show(Actor actor)
    {
        base.Show();
        status = transform.Find("Status").GetComponent<Text>();
        nickName = transform.Find("Name").GetComponent<Text>();

        mPBarGauge = transform.Find("MPBar/MPBarGauge").GetComponent<RectTransform>();
        mPBar = transform.Find("MPBar/MPBarBG").GetComponent<RectTransform>();
        hPBarGauge = transform.Find("HPBar/HPBarGauge").GetComponent<RectTransform>();
        hPBar = transform.Find("HPBar/HPBarBG").GetComponent<RectTransform>();

        mPBarGaugeImage = mPBarGauge.GetComponent<Image>();
        hPBarGaugeImage = hPBarGauge.GetComponent<Image>();

        var size = mPBarGauge.sizeDelta;
        size.x = actor.maxHp;

        mPBarGauge.sizeDelta = size;
        size.x = actor.maxHp;
        mPBarGauge.sizeDelta = size;
        mPBar.sizeDelta = size;

        hPBarGauge.sizeDelta = size;
        size.x = actor.maxHp;
        hPBarGauge.sizeDelta = size;
        hPBar.sizeDelta = size;

        mPBarGaugeImage.fillAmount = actor.mp / actor.maxMp;
        hPBarGaugeImage.fillAmount = actor.hp / actor.maxHp;

        nickName.text = actor.nickName;
        status.text = actor.status.ToString();

    }
}
