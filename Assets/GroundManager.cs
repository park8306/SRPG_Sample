using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public Vector2Int playerPos;
    public Vector2Int goalPos;
    public Dictionary<Vector2Int, int> map = new Dictionary<Vector2Int, int>();
    public List<int> passableValues = new List<int>();
    // Start is called before the first frame update
    public Transform player;
    public Transform goal;

    [ContextMenu("길찾기 테스트")]
    void Start()
    {
        FindPathCo();
    }
    IEnumerator FindPathCo()
    { 
        passableValues = new List<int>();
        passableValues.Add((int)BlockType.Walkable);    // 지나갈 수 있는 타입을 int형으로 변환하여 저장

        var blockInfos = GetComponentsInChildren<BlockInfo>();

        foreach (var item in blockInfos)
        {
            var pos = item.transform.position;  // 블록들의 위치 값 저장
            Vector2Int intPos = new Vector2Int((int)pos.x, (int)pos.z); // 블록들의 x,z 좌표 저장
            map[intPos] = (int)item.blockType;  // dictionary에 (블록의 위치, 블록의 타입) 저장
        }
        playerPos.x = (int)player.position.x;   // 플레이어의 위치 저장
        playerPos.y = (int)player.position.z;

        goalPos.x = (int)goal.position.x;   // 목표지점의 위치 저장
        goalPos.y = (int)goal.position.z;
        // 길 찾아주는 로직을 활용하여 길을 찾자
        List<Vector2Int> path = PathFinding2D.find4(playerPos, goalPos, map, passableValues);
        if (path.Count == 0)
        {
            // 길이 없다면 로그를 띄워줌
            Debug.Log("길이 없다");
        }
        else
        {
            // 길이 있다면
            // path에 저장되어있는 위치를 하나씩 불러와 이동 시키자
            foreach (var item in path)
            {
                Vector3 playerNewPos = new Vector3(item.x, 0, item.y);
                player.position = playerNewPos;
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
