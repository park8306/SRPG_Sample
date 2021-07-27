using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : SingletonMonoBehavior<FollowTarget>
{
    public Transform target;
    // 카메라의 위치를 임시 조정했다 원래 제대로 카메라의 위치를 지정했다면 해줄 이유가 없다.?
    public Vector3 offset = new Vector3(0, 0, -7);  
    public void SetTarget(Transform target) // 타겟의 transform을 가져와 target멤버변수 값 할당
    {
        this.target = target;
        if (target)
        {
            var pos = target.position;
            //pos.y = transform.position.y;
            // 기존 카메리 높이를 유지 해야지 카메라가 땅으로 가서 렌더링 안되는 버그를 막는다
            transform.position = pos + offset;
        }
    }

    //void LateUpdate()
    //{
    //    if (target == null) // 타겟이 없으면 나가자
    //        return;

    //    // 카메라의 위치를 정해주자
    //    var newPos = target.position + offset;
    //    newPos.x = transform.position.x;
    //    newPos.y = transform.position.y;
    //    transform.position = newPos;
    //}
}
