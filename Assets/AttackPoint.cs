using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);    // 어택포인트를 불러오기위해 파괴하는것이아니라 꺼줌
    }
}
