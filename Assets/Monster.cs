using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType
{
    Normal,
    Sleep,
    Die,
}

public class Actor : MonoBehaviour
{

}

public class Monster : Actor
{
    public string nickName; // 몬스터의 이름
    public float hp;
    public float mp;
    public StatusType status;
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        // 몬스터가 서 있는 블록에 몬스터 타입도 추가 시켜 주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
    }
}
