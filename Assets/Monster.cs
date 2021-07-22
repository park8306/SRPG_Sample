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
    public string iconName;
    public float hp = 20;
    public float mp = 20;
    public float maxHp = 20;
    public float maxMp = 20;
    public StatusType status;

    public int moveDistance = 5;

    // 공격 범위를 모아두자.
    public List<Vector2Int> attackablePoints = new List<Vector2Int>();
    private void Awake()
    {
        var attackPoints = GetComponentsInChildren<AttackPoint>();

        // 앞쪽에 있는 공격 포인트
        foreach (var item in attackPoints)
        {
            attackablePoints.Add(item.transform.localPosition.ToVector2Int());
        }
        // 오른쪽에 있는 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackablePoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 뒤쪽에 있는 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackablePoints.Add((item.transform.position - transform.position).ToVector2Int());
        // 왼쪽에 있는 공격 포인트
        transform.Rotate(0, 90, 0);
        foreach (var item in attackPoints)
            attackablePoints.Add((item.transform.position - transform.position).ToVector2Int());

        // 다시 앞쪽 보도록 돌림.
        transform.Rotate(0, 90, 0);
    }
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
