using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum BlockType
{
    None        = 0,
    Walkable    = 1 << 0,
    Water       = 1 << 1,
    Player      = 1 << 2,
    Monster     = 1 << 3,
}
public class BlockInfo : MonoBehaviour
{
    public BlockType blockType;

    Vector3 downMousePosition;
    public float clickDistance = 1;
    private void OnMouseDown()
    {
        // 마우스가 클릭되면 position을 저장하자
        downMousePosition = Input.mousePosition;
    }
    void OnMouseUp()
    {
        // 마우스를 떼면 실행
        // 마우스 뗀 위치를 저장하자
        var upMousePosition = Input.mousePosition;
        // 처음 클릭했던 위치와 비교하여 clickDistance보다 크다면 나가자
        if (Vector3.Distance(downMousePosition, upMousePosition) > clickDistance)
        {
            return;
        }
        // clickDistance보다 작으면 GroundManager의 OnTouch함수를 실행하자
        GroundManager.Instance.OnTouch(transform.position);
    }
}
