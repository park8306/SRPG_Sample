using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Actor
{
    public static List<Monster> Monsters = new List<Monster>();
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Monster; }
    // Start is called before the first frame update
    Animator animator;
    new protected void Awake()
    {
        base.Awake();
        Monsters.Add(this);
    }
    protected void OnDestroy()
    {
        Monsters.Remove(this);
    }

    internal IEnumerator AutoAttackCo()
    {
        yield return null;
    }
    void Start()
    {
        // 몬스터가 서 있는 블록에 몬스터 타입도 추가 시켜 주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
        animator = GetComponentInChildren<Animator>();
    }
    // 피격 당할 시 실행
    internal override void TakeHit(int power)
    {
        // 맞은 데미지 표시하자.
        GameObject damageTextGo = (GameObject)Instantiate(Resources.Load("DamageText"), transform);
        damageTextGo.transform.localPosition = new Vector3(0, 1.3f, 0);
        damageTextGo.GetComponent<TextMeshPro>().text = power.ToString();
        Destroy(damageTextGo, 2);

        hp -= power;
        animator.Play("TakeHit");
    }
}
