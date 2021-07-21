using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Flags]
// 비트 마스크?
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
        // clickDistance보다 작으면 Player의 OnTouch함수를 실행하자
        Player.SelectPlayer.OnTouch(transform.position);
    }

    string debugTextPrefab = "DebugTextPrefab"; // 리소스에서 생성할 DebugTextPrefab의 이름 저장
    GameObject debugTextGos;    // DebugTextPrefab를 생성해서 게임오브젝트로 저장할 변수
    // 디버그 텍스트를 생성시켜보자
    internal void UpdateDebugINfo()
    {
        // 생성된 DebugTextPrefab 오브젝트가 없다면
        if (debugTextGos == null)
        {
            // DebugTextPrefab를 생성시켜주자
            GameObject textMeshGo = Instantiate((GameObject)Resources.Load(debugTextPrefab), transform);
            debugTextGos = textMeshGo;
            // 이건 왜 0으로 만들었지... 위치를 맞춰주기 위함인가...
            textMeshGo.transform.localPosition = Vector3.zero;
        }

        // 블록의 정보를 저장하자
        StringBuilder debugText = new StringBuilder();
        //ContainingText(debugText, item, BlockType.Walkable);
        ContainingText(debugText, BlockType.Water);
        ContainingText(debugText, BlockType.Player);
        ContainingText(debugText, BlockType.Monster);

        // block 오브젝트의 자식 중 text 컴포넌트를 찾아 debugText의 정보를 String형으로 반환 시켜 넣어 준다.(자신의 타입이 들어 갈 것임)
        GetComponentInChildren<TextMesh>().text = debugText.ToString();
    }
    // 3d 텍스트에 자신의 blockType 정보를 넣어주는 함수
    private void ContainingText(StringBuilder sb, BlockType walkable)
    {
        // 만약 block의 BlockType과 walkable이 같은 타입이라면
        if (blockType.HasFlag(walkable))
        {
            // debugText에 값을 넣어준다.
            sb.AppendLine(walkable.ToString());
        }
    }

    Renderer m_Renderer;
    private Color m_MouseOverColor = Color.red;
    private Color m_OriginalColor;
    internal Actor actor;

    private void Awake()
    {
        m_Renderer = GetComponentInChildren<Renderer>();
        m_OriginalColor = m_Renderer.material.color;
    }
    void OnMouseOver()
    {
        m_Renderer.material.color = m_MouseOverColor;
        if (actor)
        {
            ActorStausUI.Instance.Show(actor);
        }
    }

    void OnMouseExit()
    {
        m_Renderer.material.color = m_OriginalColor;
    }
}
