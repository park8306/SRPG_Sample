using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContextMenuUI : BaseUI<ContextMenuUI>
{
    Dictionary<string, UnityAction> menus = new Dictionary<string, UnityAction>();

    public GameObject baseItem;
    protected override void OnInit()
    {
        baseItem = transform.Find("BG/Button").gameObject;

        menus.Add("턴 종료(F10_", EndTurnPlayer);
        menus.Add("테스트 메뉴(F10_", () => { print("테스트 메뉴"); });

        foreach (var item in menus)
        {
            GameObject go = (GameObject)Instantiate(baseItem, baseItem.transform.parent);
            go.GetComponentInChildren<Text>().text = item.Key;
            go.GetComponent<Button>().AddListener(this, item.Value);
        }
        baseItem.SetActive(false);
    }

    private void EndTurnPlayer()
    {
        Debug.Log("EndTurnPlayer");
    }
}
