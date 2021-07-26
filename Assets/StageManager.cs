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
    AttackToTarget,         // 모든 플레이어 선택했다면 MonsterTurn을 진행 시킨다.
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
        GameState = GameStateType.SelectPlayer;

        ShowCurrentTurn();
        CenterNotifyUI.Instance.Show("게임이 시작되었습니다", 1.5f);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ContextMenuUI.Instance.Show(Input.mousePosition);
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
            yield return monster.AutoAttackCo();
        }

        ProgressNextTurn();
    }
    int turn = 1;
    private void ProgressNextTurn()
    {
        turn++;
        // 몇 번째 턴인지 보여주자.
        ShowCurrentTurn();
        // 게임 상태를 SelectPlayer로 바꾸고
        GameState = GameStateType.SelectPlayer;
        // 턴 정보 초기화 안된것들 초기화 해주자.
    }
    private void ShowCurrentTurn()
    {
        CenterNotifyUI.Instance.Show($"{turn}이 시작되었습니다");
    }
}
