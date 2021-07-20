﻿using DG.Tweening;
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

    

    public List<GameObject> debugTextGos = new List<GameObject>();
    

    

    private void ContainingText(StringBuilder sb, BlockInfo item, BlockType walkable)
    {
        if (item.blockType.HasFlag(walkable))
        {
            sb.AppendLine(walkable.ToString());
        }
    }

   

    public void AddBlockInfo(Vector3 position, BlockType addBlockType)
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
