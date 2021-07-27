using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : SingletonMonoBehavior<FollowTarget>
{
    public Transform target;
    // 타겟의 위치로부터 카메라의 거리를 지정해주자, 인스펙터에서 조정이 되어있다
    public Vector3 offset = new Vector3(0, 0, -7);  
    public void SetTarget(Transform target) // 타겟의 transform을 가져와 target멤버변수 값 할당
    {
        this.target = target;
        if (target) // 타겟이 존재하면
        {
            var pos = target.position;  // 위치를 타겟의 위치에 가져가고
            transform.position = pos + offset;  // 타겟의 위치에서 카메라의 원래 위치로 이동시켜준다
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
