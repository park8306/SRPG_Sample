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
    public Dictionary<Vector2Int, BlockInfo> blockInfoMap = new Dictionary<Vector2Int, BlockInfo>(); // 맵 정보 에서 사용

    
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
        var blockInfos = GetComponentsInChildren<BlockInfo>(); // 블록들의 정보들을 가져온 리스트
        debugTextGos.ForEach(x => Destroy(x));  // 블럭에 기존에 있던 디버그용 텍스트 삭제
        debugTextGos.Clear();
        foreach (var item in blockInfos)
        {
            var pos = item.transform.position;  // 블록들의 위치 값 저장
            Vector2Int intPos = new Vector2Int((int)pos.x, (int)pos.z); // 블록들의 x,z 좌표 저장
            map[intPos] = item.blockType;  // dictionary에 (블록의 위치, 블록의 타입) 저장

            // GroundMaanger의 useDebugMode가 true라면
            if (useDebugMode)
            {
                // 블록들의 UpdateDebugINfo를 실행시켜 3D Text에 정보를 넣어 활성화 하자
                item.UpdateDebugINfo();
            }
            blockInfoMap[intPos] = item;    // dictionary에 (블록들의 위치값, blockInfos(블록들의 정보)) 값을 넣어준다.
        }
    }

    // ...?? 이건 뭐징
    public List<GameObject> debugTextGos = new List<GameObject>();

    // 블록에 추가로 타입을 넣어주기위한 함수
    public void AddBlockInfo(Vector3 position, BlockType addBlockType, Actor actor)
    {
        // 실행한 곳의 position 정보를 담고 있는 pos를 생성
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        // 만일 pos의 값이 map에 저장한 블록들의 위치와 일치하는게 없다면
        if (map.ContainsKey(pos) == false)
        {
            // 로그를 발생
            Debug.LogError($"{pos} 위치에 맵이 없다");
        }

        //map[pos] = map[pos] | addBlockType;
        // 맵 정보를 담고있는 dictionary에 AddBlockInfo를 실행한 블록의 블록타입을 넣어주자
        map[pos] |= addBlockType;
        blockInfoMap[pos].blockType |= addBlockType;
        blockInfoMap[pos].actor = actor;
        // 디버그용 텍스트를 띄우기 위해서 사용
        if (useDebugMode)
        {
            blockInfoMap[pos].UpdateDebugINfo();
        }
    }
    // 블록에 추가된 타입을 제거시켜주는 함수, 위의 함수에서 타입만 빼준것
    public void RemoveBlockInfo(Vector3 position, BlockType removeBlockType)
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
