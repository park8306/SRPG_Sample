using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActorStatusUI : SingletonMonoBehavior<ActorStatusUI>
{
    Text status;
    Text nickName; 
    RectTransform mPBarGauge;
    RectTransform mPBar;
    RectTransform hPBarGauge;
    RectTransform hPBar;

    Image mPBarGaugeImage;
    Image hPBarGaugeImage;
    Image icon;

    internal void Show(Actor actor)
    {
        // 부모의 Show 함수는 아마도 오브젝트 활성화 비활성화를 시켜주는거 같다...?
        base.Show();
        // 블록이 플레이어의 정보를 받았고 블록에 마우스가 올라가있으면 플레이어 정보를 UI에 띄워준다.
        status = transform.Find("Status").GetComponent<Text>();
        nickName = transform.Find("Name").GetComponent<Text>();
        icon = transform.Find("Icon").GetComponent<Image>();

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

        // UI에 플레이어의 현재 HP나 MP만큼 BarGaugeImage를 조정
        mPBarGaugeImage.fillAmount = actor.mp / actor.maxMp;
        hPBarGaugeImage.fillAmount = actor.hp / actor.maxHp;

        icon.sprite = Resources.Load<Sprite>("Icon/" + actor.iconName);
        nickName.text = actor.nickName;
        status.text = actor.status.ToString();

    }
}
