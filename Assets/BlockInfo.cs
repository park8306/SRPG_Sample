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
    public Actor actor;

    Vector3 downMousePosition;
    public float clickDistance = 1;
    private void OnMouseDown()
    {
        //ClearMoveableArea();
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

        // 처음 StageManager.GameState 상태는 SelectPlayer이다.
        // 조건에 맞는 함수들을 실행 시켜 주자
        switch (StageManager.GameState)
        {
            case GameStateType.SelectPlayer:    // Player를 선택할 상태라면
                SelectPlayer();
                break;
            case GameStateType.SelectBlockToMoveOrAttackTarget:
                SelectBlockToMoveOrAttackTarget();
                break;
            case GameStateType.SelectToAttackTarget:
                SelectToAttackTarget();
                break;
            case GameStateType.AttackToTarget:
                AttackToTarget();
                break;
            case GameStateType.NotInit:
            case GameStateType.IngPlayerMode:
            case GameStateType.MonsterTurn:
                break;
        }
    }

    private void AttackToTarget()
    {
        throw new NotImplementedException();
    }

    private void SelectToAttackTarget()
    {
        throw new NotImplementedException();
    }

    private void SelectBlockToMoveOrAttackTarget()
    {
        // 공격 대상이 있다면 공격 하자.(액터가 몬스터라면)
        if (actor)
        {
            // 공격 가능한지 판단
            if (Player.SelectedPlayer.CanAttackTarget(actor))
            {
                // 실제로 공격
                Player.SelectedPlayer.AttackToTarget(actor);
            }
            
        }
        else
        {
            if (highLightedMoveableArea.Contains(this))// 이동 가능한 영역에 포한되어있는 블록이면 이동 시키자
            {
                Player.SelectedPlayer.MoveToPosition(transform.position);// 플레이어를 이동시키고
                ClearMoveableArea();    // 이동가능영역 표시를 지워주자
                StageManager.GameState = GameStateType.IngPlayerMode;   // 플레이어가 움직이는 중이라고 상태를 바꿔주자
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void SelectPlayer()
    {
        // 선택한 actor가 플레이어라면
        if (actor.GetType() == typeof(Player))
        {
            // 플레이어의 정보를 넣어준다(캐릭터가 여러개라면 클릭한 캐릭터의 정보를 넣어준다)
            Player.SelectedPlayer = (Player)actor;

            // 선택한 캐릭터의 이동 가능한 영역 표시
            ShowMoveDistance(Player.SelectedPlayer.moveDistance);

            // 선택한 캐릭터의 현재 위치에서 공격 가능한 영역 표시.
            Player.SelectedPlayer.ShowAttackableArea();
            StageManager.GameState = GameStateType.SelectBlockToMoveOrAttackTarget;
        }
    }
    // 이동가능한 거리를 표시했던걸 원래 색으로 바꿔주자
    private void ClearMoveableArea()
    {
        // 색을 바꾸고
        highLightedMoveableArea.ForEach(x => x.ToChangeOriginalColor());
        // 리스트를 비워주자
        highLightedMoveableArea.Clear();
    }

    

    static List<BlockInfo> highLightedMoveableArea = new List<BlockInfo>();
    private void ShowMoveDistance(int moveDistance)
    {

        //Vector2Int currentPos = transform.position.ToVector2Int();
        // 쓸모 없음
        Quaternion rotate = Quaternion.Euler(0, 45, 0);
        // 블록의 위치로부터 플레이어가 이동가능한 영역의 충돌체들을 가져옴
        // halfExtents를 구하자, 0.99f는 살짝 작은 거리를 주기 위함이다
        Vector3 halfExtents = (moveDistance / Mathf.Sqrt(2)) * 0.99f * Vector3.one;

        // blocks는 플레이어에서부터 이동 가능한 거리까지의 블록들의 정보를 모아놨다
        var blocks = Physics.OverlapBox(transform.position, halfExtents, rotate);

        // 이제 이 블록들을 가지고 이동 가능한 거리 표시를 해보자
        foreach (var item in blocks)
        {
            // 이동 거리 조건이 맞는 블록들의 색을 변하게 하자
            if (Player.SelectedPlayer.OnMoveable(item.transform.position, moveDistance))
            {
                var block = item.GetComponent<BlockInfo>();
                if (block)
                {
                    // 블록의 색을 바꿔주고, 바꼈던 블록을 관리하기 위해 리스트에 넣어주자
                    block.ToChangeBlueColor();
                    highLightedMoveableArea.Add(block);
                }
            }
        }
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

        // 좌표값으로 오브젝트의 이름이 바뀜
        var intPos = transform.position.ToVector2Int();
        name = $"{name}{intPos.x}:{intPos.y}";

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
        // 만약 block의 BlockType이 텍스트에 추가할 walkable을 가지고 있다면
        // (플레이어가 있는 블록은 처음에는 Walkable만 가지고 있지만 AddBlockInfo함수를 통해서 blockType에 Player를 넣어줬다)
        if (blockType.HasFlag(walkable))
        {
            // debugText에 값을 넣어준다.
            sb.AppendLine(walkable.ToString());
        }
    }

    Renderer m_Renderer;
    private Color moveableColor = Color.blue;
    private Color m_OriginalColor;

    private void Awake()
    {
        // 블록의 렌더 정보를 블러옴
        m_Renderer = GetComponentInChildren<Renderer>();
        // 블록의 오리지널 칼라를 저장
        m_OriginalColor = m_Renderer.material.color;
    }

    // 마우스가 블록에 들어 오면 실행
    void OnMouseOver()
    {
        // 블록에 다른 배우(캐릭터나 몬스터)가 있으면 상태UI를 보여준다.
        if (actor)
        {
            ActorStatusUI.Instance.Show(actor);
        }
    }

    // 렌더러가 가지고 있는 메테리얼의 color값이 바뀜
    public void ToChangeBlueColor()
    {
        m_Renderer.material.color = moveableColor;
    }
    public void ToChangeOriginalColor()
    {
        m_Renderer.material.color = m_OriginalColor;
    }
    internal void ToChangeColor(Color color)
    {
        m_Renderer.material.color = color;
    }

    // 마우스가 블록을 빠져 나가면 실행
    void OnMouseExit()
    {
        if (actor)
        {
            ActorStatusUI.Instance.Close();
        }
    }
}
