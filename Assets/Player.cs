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
        // 처음 시작할 때 플레이어 쪽으로 카메라를 옮기자
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

            bool existAttackTarget = ShowAttackableArea(); // 공격 범위에 적이 있는가
            if (existAttackTarget) // 있으면
            {   // 게임상태를 GameStateType.SelectToAttackTarget로 바꾼다
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
    private IEnumerator AttackTargetCo(Actor attackTarget)
    {
        transform.LookAt(attackTarget.transform);

        animator.Play("Attack");
        attackTarget.TakeHit(power);
        yield return new WaitForSeconds(attackTime);
        StageManager.GameState = GameStateType.SelectPlayer;
    }

    internal bool ShowAttackableArea()
    {
        bool existEnemy = false;
        // 선택된 캐릭터의 현재 위치가 저장된다
        Vector2Int currentPos = transform.position.ToVector2Int();

        var map = GroundManager.Instance.blockInfoMap;

        // 공격 가능한 지역에 적이 있는지 확인하자., 모든 공격 위치에 몬스터가 있는지 확인한다
        foreach (var item in attackablePoints)
        {
            // 캐릭터의 원래위치 + 로컬위치니 월드 지역 위치가 된다
            Vector2Int pos = item + currentPos; // item의 월드 지역 위치
            if (map.ContainsKey(pos))   // 맵 정보에 공격 지역 위치의 블록 정보가 들어있는가
            {
                if (IsEnemyExist(map[pos])) // 공격 범위에 적(몬스터)가 있는가?
                {
                    map[pos].ToChangeColor(Color.red);  // 있으면 해당 위치의 블록 색을 바꾸자
                    existEnemy = true;  // 적이 있다
                }
            }
        }
        return existEnemy;
    }
    // 몬스터가 있는가
    private bool IsEnemyExist(BlockInfo blockInfo)
    {
        // 블록에 캐릭터나 몬스터가 없다
        if (blockInfo.actor == null)
        {
            return false;
        }
        if (blockInfo.blockType.HasFlag(BlockType.Monster) == false) // 블록 정보에 몬스터가 없다
        {
            return false;
        }
        return true;
    }

    // 블록의 위치와 최대 거리를 입력받아 움직일 수 있는 거리를 표시하자
    internal bool OnMoveable(Vector3 position, float maxDistance)
    {
        Vector2Int goalPos = position.ToVector2Int();
        Vector2Int playerPos = transform.position.ToVector2Int();
        // map은 그냥 모든 블록의 값을 가지고 있다
        var map = GroundManager.Instance.blockInfoMap;
        // 블록들의 정보를 이용해 해당 블록과 플레이어의 경로를 찾는다
        var path = PathFinding2D.find4(playerPos, goalPos, (Dictionary<Vector2Int, BlockInfo>)map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        // 만약 maxDistance보다 경로의 수가 많으면 이동 못한다고 로그를 띄워준다.
        // 여기서 +1을 해주는 이유는 처음 경로는 자기 자신이기 때문이다(?)
        // 만약 path.Count가 6이라면 자기자신의 경로도 포함이 되기 때문에 최대 갈 수 있는 거리는 5가된다.
        else if (path.Count > maxDistance+1)
            Debug.Log("이동모태 !");
        else
            return true;

        return false;
    }

    public Ease moveEase = Ease.Linear;
    public float moveTimePerUnit = 0.3f;
}
