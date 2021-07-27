using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        animator = GetComponentInChildren<Animator>();
        // 먼저 처음 지정한 공격범위를 가져오자
        var attackPoints = GetComponentsInChildren<AttackPoint>();

        // 앞쪽에 있는 공격 포인트
        foreach (var item in attackPoints)
        {
            // 공격포인트가 위치한 로컬 위치 정보들을 가져온다.
            attackableLocalPositions.Add((item.transform.position - transform.position).ToVector2Int());
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
    protected void OnDestroy()
    {
        GroundManager.Instance.RemoveBlockInfo(transform.position, GetBlockType());
    }

    public float takeHitTime = 0.7f;
    internal IEnumerator TakeHitCO(int power)
    {
        // 맞은 데미지 표시하자.
        GameObject damageTextGoInResource = (GameObject)Resources.Load("DamageText");
        var pos = transform.position;   // 공격을 당한 녀석의 위치값
        pos.y = 1.3f;   // 공격 당한 녀석의 머리 쪽으로 위치 수정
        // 공격당한 녀석에게 데미지를 표시하자. 마지막 transform은 맞은 녀석의 부모로 가지고 생성된다
        GameObject damageTextGo = Instantiate(damageTextGoInResource,
            pos,
            damageTextGoInResource.transform.rotation, transform);
        // 데미지 텍스트에 power값 만큼 텍스트에 표시하자
        damageTextGo.GetComponent<TextMeshPro>().text = power.ToString();
        Destroy(damageTextGo, 2);
        // hp를 power만큼 깍고
        hp -= power;
        animator.Play("TakeHit"); // 맞는 모션을 취하고
        yield return new WaitForSeconds(takeHitTime);
        // hp가 0과 같거나 작으면 죽는 모션을 하고
        if (hp <= 0)
        {
            animator.Play("Die");
            status = StatusType.Die; // 상태도 죽었다고 표시해주자
            // 자식들이 오버라이드해서 구현하려는 함수(monster와 player가 행동해야할 내용이 다르다)
            OnDie();
        }
    }
    
    protected virtual void OnDie()
    {
        Debug.LogError("자식들이 오버라이드 해서 구현해야함");
    }

    protected IEnumerator FindPathCo(Vector2Int destPos)
    {
        Transform myTr = transform; // 이동하기전 자신의 Transform을 넣어주고
        // 이동하기전 자신 위치 저장
        Vector2Int myPos = myTr.position.ToVector2Int(); 
        Vector3 myPosVector3 = myTr.position;
        // map : 모든 맵들의 정보
        var map = GroundManager.Instance.blockInfoMap;
        // 길 찾아주는 로직을 활용하여 길을 찾자, path에는 목적지까지 가는 블록들의 위치가 담겨있다
        List<Vector2Int> path = PathFinding2D.find4(myPos, destPos, map, passableValues);
        if (path.Count == 0)
        {
            // 길이 없다면 로그를 띄워줌
            Debug.Log("길이 없다");
        }
        else
        {
            // 자신이 서 있던 자리의 정보를 없애준다
            GroundManager.Instance.RemoveBlockInfo(myPosVector3, GetBlockType());
            PlayAnimation("Walk");
            // FollowTarget의 SetTarget을 실행시켜 선택된 캐릭터를 카메라가 따라가게 하자
            FollowTarget.Instance.SetTarget(myTr);
            // 경로의 첫 지점의 자신의 지점이니 없애주자
            path.RemoveAt(0);

            // 움직일 경로를 찾은 대상이 몬스터 일 때는 마지막 지점을 삭제한다.(플레이어와 겹치지 않기 위해) 
            if (ActorType == ActorTypeEnum.Monster)
            {
                path.RemoveAt(path.Count - 1);  // 마지막 지점 삭제
            }
            if (path.Count>moveDistance)    // 움직일 경로의 수가 moveDistance보다 크면
            {
                path.RemoveRange(moveDistance, path.Count - moveDistance);  // 해당 경로에서 moveDistance만큼만 움직이게 한다.
            }
            // path에 저장되어있는 위치를 하나씩 불러와 이동 시키자
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, myPosVector3.y, item.y);
                // 플레이어가 이동할 방향으로 바라보자
                myTr.LookAt(playerNewPos);
                // 플레이어가 움직일 때 자연스럽게 움직이도록 하자
                // DOMove함수는 DOTween을 임포트하여 가져온 함수
                // moveTimePerUnit시간동안 자연스럽게 움직이도록 한다
                myTr.DOMove(playerNewPos, moveTimePerUnit);
                // 움직이는 시간 만큼 기다리자
                yield return new WaitForSeconds(moveTimePerUnit);
            }
            // 이동이 끝나면 Idle애니메이션을 실행시키자
            PlayAnimation("Idle");
            // null을 주어 카메라가 따라가지 않도록 하자
            FollowTarget.Instance.SetTarget(null);
            // 이동한 위치에는 플레이어 정보 추가
            GroundManager.Instance.AddBlockInfo(myTr.position, GetBlockType(), this);


            completeMove = true;
            OnCompleteMove();
        }
    }
    // 대상의 블록타입을 받아오자
    public virtual BlockType GetBlockType()
    {
        Debug.LogError("자식에서 GetBlockType함수 오버라이드 해야함");
        return BlockType.None;
    }

    protected  virtual void OnCompleteMove()
    {
        
    }
    // 애니메이션을 실행하는 함수(nodeName, 0, 0) 이부분이 귀찮다... 함수로 만드는게 좋아보인다
    public void PlayAnimation(string nodeName)
    {
        animator.Play(nodeName, 0, 0);
    }

    // 적 위치가 공격 가능한 범위에 있는지 판단
    protected bool IsInAttackableArea(Vector3 enemyPosition)
    {
        // 적과 나의 위치를 Vector2Int로 바꿔주자
        Vector2Int enemyPositionVector2 = enemyPosition.ToVector2Int();
        Vector2Int currentPos = transform.position.ToVector2Int();

        // 공격 가능한 지역에 적이 있는지 확인하자., 모든 공격 위치에 몬스터가 있는지 확인한다
        foreach (var item in attackableLocalPositions)
        {
            // pos : 공격 가능한 월드 포지션
            // item에는 공격 가능한 범위의 로컬 포지션이 들어 있다. 현재 포지션을 더해 월드 포지션으로 바꿔주자
            Vector2Int pos = item + currentPos;
            // 공격 위치와 적 위치가 같으면
            if (pos == enemyPositionVector2)
            {
                return true;    // 공격 가능한 범위에 있다고 알려준다.
            }
        }
        return false;   // 그 외에는 공격 범위에 없다고 알려준다.
    }
    protected IEnumerator AttackToTargetCo(Actor attackTarget)
    {
        transform.LookAt(attackTarget.transform);   // 공격 할 대상을 바라보고

        animator.Play("Attack");
        StartCoroutine(attackTarget.TakeHitCO(power));  // 공격 타겟의 TakeHitCo를 실행시키자
        yield return new WaitForSeconds(attackTime);    // 일정 공격 시간이 지나고
        completeAct = true; // Act행동을 완료하자
        
    }
} 
