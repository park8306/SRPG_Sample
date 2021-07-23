using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType
{
    Normal,
    Sleep,
    Die,
}

public enum ActorTypeEnum
{
    NotInit,
    Player,
    Monster,
}

// Player와 Monster는 기본적인 정보를 가지고 있는다
public class Actor : MonoBehaviour
{
    public virtual ActorTypeEnum ActorType { get => ActorTypeEnum.NotInit; }
    public string nickName = "이름 입력해주세요"; // 몬스터의 이름
    public string iconName;
    public int power = 10;
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
        // 먼저 처음 지정한 공격범위를 가져오자
        var attackPoints = GetComponentsInChildren<AttackPoint>();

        // 앞쪽에 있는 공격 포인트
        foreach (var item in attackPoints)
        {
            // 공격포인트가 위치한 로컬 위치 정보들을 가져온다.
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
    internal virtual void TakeHit(int power)
    {
        hp -= power;
    }
}

public class Monster : Actor
{
    public override ActorTypeEnum ActorType { get => ActorTypeEnum.Monster; }
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        // 몬스터가 서 있는 블록에 몬스터 타입도 추가 시켜 주자
        GroundManager.Instance.AddBlockInfo(transform.position, BlockType.Monster, this);
        animator = GetComponentInChildren<Animator>();
    }
    // 피격 당할 시 실행
    internal override void TakeHit(int power)
    {
        // 맞은 데미지 표시하자.
        GameObject damageTextGo = (GameObject)Instantiate(Resources.Load("DamageText"), transform);
        damageTextGo.transform.localPosition = new Vector3(0, 1.3f, 0);
        damageTextGo.GetComponent<TextMeshPro>().text = power.ToString();
        Destroy(damageTextGo, 2);

        hp -= power;
        animator.Play("TakeHit");
    }
}
