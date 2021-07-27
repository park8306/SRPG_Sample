﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType
{
    Normal,
    Sleep,
    Die,
}

public enum ActorTypeEnum
{
    NotInit,
    Player,
    Monster,
}

// Player와 Monster는 기본적인 정보를 가지고 있는다
public class Actor : MonoBehaviour
{
    public virtual ActorTypeEnum ActorType { get => ActorTypeEnum.NotInit; }
    public string nickName = "이름 입력해주세요"; // 몬스터의 이름
    public string iconName;
    public int power = 10;
    public float hp = 20;
    public float mp = 20;
    public float maxHp = 20;
    public float maxMp = 20;
    public StatusType status;

    public int moveDistance = 5;

    public bool completeMove;
    public bool completeAct;

    public bool CompleteTurn { get => completeMove && completeAct; }

    // 공격 범위를 모아두자.
    public List<Vector2Int> attackableLocalPositions = new List<Vector2Int>();

    public float moveTimePerUnit = 0.3f;
    protected Animator animator;
    protected BlockType passableValues = BlockType.Walkable | BlockType.Water;

    public float attackTime = 5;

    protected void Awake()
    {
        // 먼저 처음 지정한 공격범위를 가져오자
        var attackPoints = GetComponentsInChildren<AttackPoint>();

        // 앞쪽에 있는 공격 포인트
        foreach (var item in attackPoints)
        {
            // 공격포인트가 위치한 로컬 위치 정보들을 가져온다.
            attackableLocalPositions.Add(item.transform.localPosition.ToVector2Int());
        }
        // 오른쪽에 있는 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackableLocalPositions.Add((item.transform.position - transform.position).ToVector2Int());
        // 뒤쪽에 있는 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackableLocalPositions.Add((item.transform.position - transform.position).ToVector2Int());
        // 왼쪽에 있는 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackableLocalPositions.Add((item.transform.position - transform.position).ToVector2Int());

        // 다시 앞쪽 보도록 돌림.
        transform.Rotate(0, 90, 0);
    }
    internal virtual void TakeHit(int power)
    {
        hp -= power;
    }
    protected IEnumerator FindPathCo(Vector2Int destPos)
    {
        Transform myTr = transform;
        Vector2Int myPos = myTr.position.ToVector2Int();
        Vector3 myPosVector3 = myTr.position;
        var map = GroundManager.Instance.blockInfoMap;
        // 길 찾아주는 로직을 활용하여 길을 찾자
        List<Vector2Int> path = PathFinding2D.find4(myPos, destPos, map, passableValues);
        if (path.Count == 0)
        {
            // 길이 없다면 로그를 띄워줌
            Debug.Log("길이 없다");
        }
        else
        {
            // 원래 위치에선 플레이어 정보 삭제
            GroundManager.Instance.RemoveBlockInfo(myPosVector3, GetBlockType());
            // 길이 있다면
            // 애니메이션 Walk를 실행
            PlayAnimation("Walk");
            // FollowTarget의 SetTarget을 실행시켜 선택된 캐릭터를 카메라가 따라가게 하자
            FollowTarget.Instance.SetTarget(myTr);
            // 경로의 첫 지점의 자신의 지점이니 없애주자
            path.RemoveAt(0);
            // path에 저장되어있는 위치를 하나씩 불러와 이동 시키자
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, myPosVector3.y, item.y);
                // 플레이어가 이동할 방향으로 바라보자
                myTr.LookAt(playerNewPos);
                // 플레이어가 움직일 때 자연스럽게 움직이도록 하자
                // DOMove함수는 DOTween을 임포트하여 가져온 함수
                myTr.DOMove(playerNewPos, moveTimePerUnit);
                // 움직이는 시간 만큼 기다리자
                yield return new WaitForSeconds(moveTimePerUnit);
            }
            // 이동이 끝나면 Idle애니메이션을 실행시키자
            PlayAnimation("Idle");
            // null을 주어 카메라가 따라가지 않도록 하자
            FollowTarget.Instance.SetTarget(null);
            // 이동한 위치에는 플레이어 정보 추가
            GroundManager.Instance.AddBlockInfo(myPosVector3, GetBlockType(), this);


            completeMove = true;
            OnCompleteMove();
        }
    }

    public virtual BlockType GetBlockType()
    {
        Debug.LogError("자식에서 GetBlockType함수 오버라이드 해야함");
        return BlockType.None;
    }

    protected  virtual void OnCompleteMove()
    {
        
    }

    public void PlayAnimation(string nodeName)
    {
        animator.Play(nodeName, 0, 0);
    }

    protected bool IsInAttackableArea(Vector3 enemyPosition)
    {
        Vector2Int enemyPositionVector2 = enemyPosition.ToVector2Int();
        Vector2Int currentPos = transform.position.ToVector2Int();

        // 공격 가능한 지역에 적이 있는지 확인하자., 모든 공격 위치에 몬스터가 있는지 확인한다
        foreach (var item in attackableLocalPositions)
        {
            // pos : 공격 가능한 월드 포지션
            Vector2Int pos = item + currentPos;

            if (pos == enemyPositionVector2)
            {
                return true;
            }
        }
        return false;
    }
    protected IEnumerator AttackToTargetCo(Actor attackTarget)
    {
        transform.LookAt(attackTarget.transform);

        animator.Play("Attack");
        attackTarget.TakeHit(power);
        yield return new WaitForSeconds(attackTime);
        completeAct = true;
        StageManager.GameState = GameStateType.SelectPlayer;
    }
}
