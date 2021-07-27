using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class Player : Actor
{
    public static List<Player> Players = new List<Player>();
    // Start is called before the first frame update
    new protected void Awake()
    {
        base.Awake();
        Players.Add(this);
    }
    new protected void OnDestroy()
    {
        base.OnDestroy();
        Players.Remove(this);
    }


    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Player; }
    static public Player SelectedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // 플레이어가 서 있는 블록은 처음에 타입이 walkable밖에 설정되어있지 않다. AddBlockInfo를 실행하여 플레이어의 타입도 넣어주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
        // 처음 시작할 때 플레이어 쪽으로 카메라를 옮기자
        FollowTarget.Instance.SetTarget(transform);
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

    internal bool CanAttackTarget(Actor enemy)
    {
        // 같은 팀을 공격대상으로 하지 않기
        if (enemy.ActorType != ActorTypeEnum.Monster)
        {
            return false;
        }
        if (IsInAttackableArea(enemy.transform.position) == false)
        {
            return false;
        }

        return true;
    }

    internal void AttackToTarget(Monster actor)
    {
        ClearEnemyExitPosint();
        StartCoroutine( AttackToTargetCo_(actor));
    }

    private IEnumerator AttackToTargetCo_(Monster monster)
    {
        yield return AttackToTargetCo(monster);
        //if(monster.status == StatusType.Die)
        //{
        //    AddExp(monster.rewardExp);
        //}
        StageManager.GameState = GameStateType.SelectPlayer;
    }

    public void ClearEnemyExitPosint()
    {
        enemyExistPoint.ForEach(x => x.ToChangeOriginalColor());
        enemyExistPoint.Clear();
    }

    protected override void OnCompleteMove()
    {
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

    public List<BlockInfo> enemyExistPoint = new List<BlockInfo>();
    internal bool ShowAttackableArea()
    {
        // 선택된 캐릭터의 현재 위치가 저장된다
        Vector2Int currentPos = transform.position.ToVector2Int();

        var map = GroundManager.Instance.blockInfoMap;

        // 공격 가능한 지역에 적이 있는지 확인하자., 모든 공격 위치에 몬스터가 있는지 확인한다
        foreach (var item in attackableLocalPositions)
        {
            // 캐릭터의 원래위치 + 로컬위치니 월드 지역 위치가 된다
            Vector2Int pos = item + currentPos; // item의 월드 지역 위치
            if (map.ContainsKey(pos))   // 맵 정보에 공격 지역 위치의 블록 정보가 들어있는가
            {
                if (IsEnemyExist(map[pos])) // 공격 범위에 적(몬스터)가 있는가?
                {
                    enemyExistPoint.Add(map[pos]);
                }
            }
        }
        enemyExistPoint.ForEach(x => x.ToChangeColor(Color.red));

        //map[pos].ToChangeColor(Color.red);  // 있으면 해당 위치의 블록 색을 바꾸자
        //existEnemy = true;  // 적이 있다

        return enemyExistPoint.Count > 0;
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
        if (path.Count == 0 || path.Count > maxDistance +1)
            return false;
        // 만약 maxDistance보다 경로의 수가 많으면 이동 못한다고 로그를 띄워준다.
        // 여기서 +1을 해주는 이유는 처음 경로는 자기 자신이기 때문이다(?)
        // 만약 path.Count가 6이라면 자기자신의 경로도 포함이 되기 때문에 최대 갈 수 있는 거리는 5가된다.

        return true;
    }
    public override BlockType GetBlockType()
    {
        return BlockType.Player;
    }
    protected override void OnDie()
    {
        // 플레이어가 죽은 경우.
        // 모든 플레이어가 죽었다면 GameOver표시
        if(Players.Where(x => x.status != StatusType.Die).Count() == 0)
        {
            CenterNotifyUI.Instance.Show("유다이 Game Over");
        }
    }
}
