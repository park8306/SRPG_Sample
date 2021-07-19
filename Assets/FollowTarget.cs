using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : SingletonMonoBehavior<FollowTarget>
{
    Transform target;
    public Vector3 offset = new Vector3(0, 0, -7);
    public void SetTarget(Transform target) // 타겟의 transform을 가져와 target멤버변수 값 할당
    {
        this.target = target;
    }

    void LateUpdate()
    {
        if (target == null) // 타겟이 없으면 나가자
            return;

        // 카메라의 위치를 정해주자
        var newPos = target.position + offset;
        newPos.y = transform.position.y;
        transform.position = newPos;
    }
}
