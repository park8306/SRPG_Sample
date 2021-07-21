using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType
{
    Normal,
    Sleep,
    Die,
}

// Player와 Monster는 기본적인 정보를 가지고 있는다
public class Actor : MonoBehaviour
{
    public string nickName = "이름 입력해주세요"; // 몬스터의 이름
    public float hp = 20;
    public float mp = 20;
    public float maxHp = 20;
    public float maxMp = 20;
    public StatusType status;

    public int moveDistance = 5;
}

public class Monster : Actor
{
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        // 몬스터가 서 있는 블록에 몬스터 타입도 추가 시켜 주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
    }
}
