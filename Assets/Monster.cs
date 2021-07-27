using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Actor
{
    public static List<Monster> Monsters = new List<Monster>();
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Monster; }
    // Start is called before the first frame update
    //Animator animator;
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
        // 가장 가까이에 있는 플레이어를 찾자
        Player enemyPlayer = GetNearestPlayer();
        // 공격 가능한 위치에 있다면 바로 공격하자
        if (IsInAttackableArea(enemyPlayer.transform.position))
        {
            yield return AttackToTargetCo(enemyPlayer);
        }
        else
        {
            // Player쪽으로 이동하자.
            yield return FindPathCo(enemyPlayer.transform.position.ToVector2Int());

            // 공격할 수 있으면 공격하자
            yield return AttackToTargetCo(enemyPlayer);
        }


        yield return null;
    }

    private bool ISAttackablePosition(Vector3 position)
    {
        throw new NotImplementedException();
    }

    private Player GetNearestPlayer()
    {
        var myPosition = transform.position;
        var nearestPlayer= Player.Players
            .Where(x => x.status != StatusType.Die)
            .OrderBy(x => Vector3.Distance(x.transform.position, myPosition))
            .Single();
        return nearestPlayer;
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
    public override BlockType GetBlockType()
    {
        return BlockType.Monster;
    }
}
