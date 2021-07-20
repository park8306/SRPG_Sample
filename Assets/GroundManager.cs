using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GroundManager : SingletonMonoBehavior<GroundManager>
{
    public Vector2Int playerPos;


    public Dictionary<Vector2Int, BlockType> map = new Dictionary<Vector2Int, BlockType>(); // A*에서 사용
    public Dictionary<Vector2Int, BlockInfo> blockInfoMap = new Dictionary<Vector2Int, BlockInfo>(); // A*에서 사용

    internal void OnTouch(Vector3 position)
    {
        Vector2Int findPos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        FindPath(findPos);
    }
    // 지나갈 수 있는 타입을 미리 저장해 맵 정보에 사용할 수 있도록 하자. 전의 코드는 int형으로 저장을 했었다
    // Walkable 과 Water(둘중 하나라도? 아마 "|" 때문에)로 지정된 블록은 지나다닐 수 있는 블록이다.
    public BlockType passableValues = BlockType.Walkable | BlockType.Water;
    // Start is called before the first frame update
    public Transform player;

    public bool useDebugMode = false;
    public GameObject debugTextPrefab;

    new private void Awake()
    {
        base.Awake();
        var blockInfos = GetComponentsInChildren<BlockInfo>();
        debugTextGos.ForEach(x => Destroy(x));  // 블럭에 기존에 있던 디버그용 텍스트 삭제
        debugTextGos.Clear();
        foreach (var item in blockInfos)
        {
            var pos = item.transform.position;  // 블록들의 위치 값 저장
            Vector2Int intPos = new Vector2Int((int)pos.x, (int)pos.z); // 블록들의 x,z 좌표 저장
            map[intPos] = item.blockType;  // dictionary에 (블록의 위치, 블록의 타입) 저장

            if (useDebugMode)
            {
                item.UpdateDebugINfo();
            }
            blockInfoMap[intPos] = item;
            //StringBuilder debugText = new StringBuilder();
            ////ContainingText(debugText, item, BlockType.Walkable);
            //ContainingText(debugText, item, BlockType.Water);
            //ContainingText(debugText, item, BlockType.Player);
            //ContainingText(debugText, item, BlockType.Monster);

            //GameObject textMeshGo = Instantiate(debugTextPrefab, item.transform);
            //debugTextGos.Add(textMeshGo);
            //textMeshGo.transform.localPosition = Vector3.zero;
            //TextMesh textMesh = textMeshGo.GetComponentInChildren<TextMesh>();
            //textMesh.text = debugText.ToString();
        }
    }

    void FindPath(Vector2Int goalPos)
    {
        StopAllCoroutines();
        StartCoroutine(FindPathCo(goalPos));
    }

    public List<GameObject> debugTextGos = new List<GameObject>();
    IEnumerator FindPathCo(Vector2Int goalPos)
    { 
        playerPos.x = Mathf.RoundToInt(player.position.x);   // 플레이어의 위치 저장
        playerPos.y = Mathf.RoundToInt(player.position.z);

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
            RemoveBlockInfo(Player.SelectPlayer.transform.position, BlockType.Player);
            // 길이 있다면
            // 애니메이션 Walk를 실행
            Player.SelectPlayer.PlayAnimation("Walk");
            // FollowTarget의 SetTarget을 실행시켜 선택된 캐릭터를 카메라가 따라가게 하자
            FollowTarget.Instance.SetTarget(Player.SelectPlayer.transform);
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
            AddBlockInfo(Player.SelectPlayer.transform.position, BlockType.Player);
        }
    }

    

    private void ContainingText(StringBuilder sb, BlockInfo item, BlockType walkable)
    {
        if (item.blockType.HasFlag(walkable))
        {
            sb.AppendLine(walkable.ToString());
        }
    }

    public Ease moveEase = Ease.Linear;
    public float moveTimePerUnit = 0.3f;

    internal void AddBlockInfo(Vector3 position, BlockType addBlockType)
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        if (map.ContainsKey(pos) == false)
        {
            Debug.LogError($"{pos} 위치에 맵이 없다");
        }

        //map[pos] = map[pos] | addBlockType;
        map[pos] |= addBlockType;
        blockInfoMap[pos].blockType |= addBlockType;
        if (useDebugMode)
        {
            blockInfoMap[pos].UpdateDebugINfo();
        }
    }
    private void RemoveBlockInfo(Vector3 position, BlockType removeBlockType)
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        if (map.ContainsKey(pos) == false)
        {
            Debug.LogError($"{pos} 위치에 맵이 없다");
        }

        //map[pos] = map[pos] | addBlockType;
        map[pos] &= ~removeBlockType;
        blockInfoMap[pos].blockType &= ~removeBlockType;
        if (useDebugMode)
        {
            blockInfoMap[pos].UpdateDebugINfo();
        }
    }
}
