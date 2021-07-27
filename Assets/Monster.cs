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
    new protected void OnDestroy()
    {
        base.OnDestroy();
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
        }// 공격 가능한 위치에 있지 않으면
        else
        {
            // 가장 가까이에 있는 플레이어로 이동하고
            yield return FindPathCo(enemyPlayer.transform.position.ToVector2Int());

            // 공격 범위 안에 들어오면 공격하자
            if (IsInAttackableArea(enemyPlayer.transform.position))
            {
                yield return AttackToTargetCo(enemyPlayer);
            }
        }


        yield return null;
    }

    private Player GetNearestPlayer()
    {
        var myPosition = transform.position;
        var nearestPlayer= Player.Players
            .Where(x => x.status != StatusType.Die)
            .OrderBy(x => Vector3.Distance(x.transform.position, myPosition))
            .First();
        return nearestPlayer;
    }

    void Start()
    {
        // 몬스터가 서 있는 블록에 몬스터 타입도 추가 시켜 주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
        animator = GetComponentInChildren<Animator>();
    }
    // 피격 당할 시 실행
    
    public override BlockType GetBlockType()
    {
        return BlockType.Monster;
    }
    protected override void OnDie()
    {
        // 몬스터가 죽은 경우.
        // 죽인 플레이어한테 경험치 주기
        // 몬스터 GameObject 파괴.
        // 모든 몬스터가 죽었는지 파악해서 다 죽었다면 스테이지 클리어
        if (Monsters.Where(x => x.status != StatusType.Die).Count() == 0)
        {
            CenterNotifyUI.Instance.Show("Stage Clear");
        }
        Destroy(gameObject, 1);
    }
}
