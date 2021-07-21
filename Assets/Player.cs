using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Actor
{
    static public Player SelectPlayer;
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        SelectPlayer = this;
        animator = GetComponentInChildren<Animator>();
        // 플레이어가 서 있는 블록은 처음에 타입이 walkable밖에 설정되어있지 않다. AddBlockInfo를 실행하여 플레이어의 타입도 넣어주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Player, this);
    }

    public void PlayAnimation(string nodeName)
    {
        animator.Play(nodeName, 0, 0);
    }
    internal void OnTouch(Vector3 position)
    {
        Vector2Int findPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
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
        var map = GroundManager.Instance.map;
        // 길 찾아주는 로직을 활용하여 길을 찾자
        List<Vector2Int> path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
        {
            // 길이 없다면 로그를 띄워줌
            Debug.Log("길이 없다");
        }
        else
        {
            // 원래 위치에선 플레이어 정보 삭제
            GroundManager.Instance.RemoveBlockInfo(Player.SelectPlayer.transform.position, BlockType.Player);
            // 길이 있다면
            // 애니메이션 Walk를 실행
            Player.SelectPlayer.PlayAnimation("Walk");
            // FollowTarget의 SetTarget을 실행시켜 선택된 캐릭터를 카메라가 따라가게 하자
            FollowTarget.Instance.SetTarget(Player.SelectPlayer.transform);
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
            Player.SelectPlayer.PlayAnimation("Idle");
            // null을 주어 카메라가 따라가지 않도록 하자
            FollowTarget.Instance.SetTarget(null);
            // 이동한 위치에는 플레이어 정보 추가
            GroundManager.Instance.AddBlockInfo(Player.SelectPlayer.transform.position, BlockType.Player,this);
        }
    }
    // 이거는 아직 사용하는 곳이 없다
    internal bool OnMoveable(Vector3 position)
    {
        Vector2Int goalPos = position.ToVector2Int();
        Vector2Int playerPos = transform.position.ToVector2Int();
        var map = GroundManager.Instance.map;
        var path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
            Debug.Log("길 업따 !");
        else if (path.Count > 5)
            Debug.Log("이동모태 !");
        else
            return true;

        return false;
    }

    public Ease moveEase = Ease.Linear;
    public float moveTimePerUnit = 0.3f;
}
