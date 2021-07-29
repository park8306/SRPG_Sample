using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType
{
    NotInit,                // 아직 초기화 되지 않음
    SelectPlayer,           // 조정할 아군 선택,
    SelectedPlayerMoveOrAct,  // 선택된 플레이어가 이동하거나 행동을 할 차례
    IngPlayerMode,          // 플레이어 이동 중
    SelectToAttackTarget,   // 이동 후에 공격할 타겟을 선택.
    MonsterTurn,
}

public class StageManager : SingletonMonoBehavior<StageManager>
{
    [SerializeField] GameStateType m_gameState;
    static public GameStateType GameState
    {
        get => Instance.m_gameState;
        set {
            Debug.Log($"{Instance.m_gameState} => {value}");
            NotifyUI.Instance.Show(value.ToString(), 10);
            Instance.m_gameState = value;
        }
    }
    void Start()
    {
        OnStartTurn();
    }

    private void OnStartTurn()
    {
        // 카메라의 위치를 Players 첫 번째의 위치로 따라가자
        FollowTarget.Instance.SetTarget(Player.Players[0].transform);

        // 몇 번째 턴인지 보여주자.
        ShowCurrentTurn();

        // 게임상태를 SelectPlayer
        GameState = GameStateType.SelectPlayer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))   // 마우스 우클릭 시
        {// 게임 상태가 움직일 대상을 선택하고 움직이거나 행동해야할 턴일 경우
            if(GameState == GameStateType.SelectedPlayerMoveOrAct)
            {
                Player.SelectedPlayer = null;   // 선택된 대상을 풀어주고
                BlockInfo.ClearMoveableArea();  // 이동 가능 영역을 클리어해주고
                GameState = GameStateType.SelectPlayer; // 대상을 선택할 턴으로 게임상태를 바꿔줌
            }
            else // 나머지 상황에서는 ContextMenuUI를 보여줌
            {
                ContextMenuUI.Instance.Show(Input.mousePosition);
            }
        }
    }

    internal void EndTurnPlayer()
    {
        GameState = GameStateType.MonsterTurn;
        StartCoroutine(MonsterTurnCo());
    }

    private IEnumerator MonsterTurnCo()
    {
        foreach(var monster in Monster.Monsters)
        {
            FollowTarget.Instance.SetTarget(monster.transform);
            yield return monster.AutoAttackCo();
        }

        ProgressNextTurn();
    }
    int turn = 1;
    private void ProgressNextTurn()
    {
        ClearTurnInfo();

        turn++;
        // 몇 번째 턴인지 보여주자.
        OnStartTurn();
    }

    private void ClearTurnInfo()
    {
        Player.Players.ForEach(x => { x.completeMove = false; x.completeAct = false; });

        Monster.Monsters.ForEach(x => { x.completeMove = false; x.completeAct = false; });
    }

    private void ShowCurrentTurn()
    {
        CenterNotifyUI.Instance.Show($"{turn}턴이 시작되었습니다");
    }
}
