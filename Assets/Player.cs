using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Player : Actor
{
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Player; }
    static public Player SelectedPlayer;
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        //SelectedPlayer = this;
        animator = GetComponentInChildren<Animator>();
        // 플레이어가 서 있는 블록은 처음에 타입이 walkable밖에 설정되어있지 않다. AddBlockInfo를 실행하여 플레이어의 타입도 넣어주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
        FollowTarget.Instance.SetTarget(transform);
    }

    public void PlayAnimation(string nodeName)
    {
        animator.Play(nodeName, 0, 0);
    }
    internal void MoveToPosition(Vector3 position)
    {
        Vector2Int findPos = position.ToVector2Int();
        FindPath(findPos);
    }
    void FindPath(Vector2Int goalPos)
    {
        StopAllCoroutines();
        StartCoroutine(FindPathCo(goalPos));
    }
    public BlockType passableValues = BlockType.Walkable | BlockType.Water;
    IEnumerator FindPathCo(Vector2Int goalPos)
    {
        Transform player = transform;
        Vector2Int playerPos = new Vector2Int(Mathf.RoundToInt(player.position.x),
            Mathf.RoundToInt(player.position.z));
        playerPos.x = Mathf.RoundToInt(player.position.x);   // 플레이어의 위치 저장
        playerPos.y = Mathf.RoundToInt(player.position.z);
        var map = GroundManager.Instance.blockInfoMap;
        // 길 찾아주는 로직을 활용하여 길을 찾자
        List<Vector2Int> path = PathFinding2D.find4(playerPos, goalPos, (Dictionary<Vector2Int, BlockInfo>)map, passableValues);
        if (path.Count == 0)
        {
            // 길이 없다면 로그를 띄워줌
            Debug.Log("길이 없다");
        }
        else
        {
            // 원래 위치에선 플레이어 정보 삭제
            GroundManager.Instance.RemoveBlockInfo(Player.SelectedPlayer.transform.position, BlockType.Player);
            // 길이 있다면
            // 애니메이션 Walk를 실행
            Player.SelectedPlayer.PlayAnimation("Walk");
            // FollowTarget의 SetTarget을 실행시켜 선택된 캐릭터를 카메라가 따라가게 하자
            FollowTarget.Instance.SetTarget(Player.SelectedPlayer.transform);
            // 경로의 첫 지점의 자신의 지점이니 없애주자
            path.RemoveAt(0);
            // path에 저장되어있는 위치를 하나씩 불러와 이동 시키자
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, 0, item.y);
                // 플레이어가 이동할 방향으로 바라보자
                player.LookAt(playerNewPos);
                // 플레이어가 움직일 때 자연스럽게 움직이도록 하자
                // DOMove함수는 DOTween을 임포트하여 가져온 함수
                player.DOMove(playerNewPos, moveTimePerUnit).SetEase(moveEase);
                // 움직이는 시간 만큼 기다리자
                yield return new WaitForSeconds(moveTimePerUnit);
            }
            // 이동이 끝나면 Idle애니메이션을 실행시키자
            Player.SelectedPlayer.PlayAnimation("Idle");
            // null을 주어 카메라가 따라가지 않도록 하자
            FollowTarget.Instance.SetTarget(null);
            // 이동한 위치에는 플레이어 정보 추가
            GroundManager.Instance.AddBlockInfo(Player.SelectedPlayer.transform.position, BlockType.Player,this);

            bool existAttackTarget = ShowAttackableArea();
            if (existAttackTarget)
            {
                StageManager.GameState = GameStateType.SelectToAttackTarget;
            }
            else
            {
                StageManager.GameState = GameStateType.SelectPlayer;
            }
        }
    }
    internal bool CanAttackTarget(Actor actor)
    {
        // 같은 팀을 공격대상으로 하지 않기
        if (actor.ActorType != ActorTypeEnum.Monster)
        {
            return false;
        }

        return true;
    }

    internal void AttackToTarget(Actor actor)
    {
        StartCoroutine(AttackTargetCo(actor));
    }

    public float attackTime = 5;
    private IEnumerator AttackTargetCo(Actor actor)
    {
        animator.Play("Attack");
        actor.TakeHit(power);
        yield return new WaitForSeconds(attackTime);
        StageManager.GameState = GameStateType.SelectPlayer;
    }

    internal bool ShowAttackableArea()
    {
        bool existEnemy = false;
        Vector2Int currentPos = transform.position.ToVector2Int();

        var map = GroundManager.Instance.blockInfoMap;

        // 공격 가능한 지역에 적이 있는지 확인하자.
        foreach (var item in attackablePoints)
        {
            Vector2Int pos = item + currentPos; // item의 월드 지역 위치
            //if(item) 지역에 적이 있는가?
            if (map.ContainsKey(pos))
            {
                if (IsEnemyExist(map[pos]))
                {
                    map[pos].ToChangeColor(Color.red);
                    existEnemy = true;
                }
            }
        }
        return existEnemy;
    }

    private bool IsEnemyExist(BlockInfo blockInfo)
    {
        if (blockInfo.actor == null)
        {
            return false;
        }
        if (blockInfo.blockType.HasFlag(BlockType.Monster) == false)
        {
            return false;
        }
        return true;
    }

    // 이거는 아직 사용하는 곳이 없다
    internal bool OnMoveable(Vector3 position, float maxDistance)
    {
        Vector2Int goalPos = position.ToVector2Int();
        Vector2Int playerPos = transform.position.ToVector2Int();
        // map은 그냥 모든 블록의 값을 가지고 있다
        var map = GroundManager.Instance.blockInfoMap;
        // 플레이어로부터 모든 블록의 경로들을 path가 가지고 있다.
        var path = PathFinding2D.find4(playerPos, goalPos, (Dictionary<Vector2Int, BlockInfo>)map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else if (path.Count > maxDistance+1)
            Debug.Log("이동모태 !");
        else
            return true;

        return false;
    }

    public Ease moveEase = Ease.Linear;
    public float moveTimePerUnit = 0.3f;
}
