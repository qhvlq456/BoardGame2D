using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

[RequireComponent(typeof(BoxCollider2D))]
// 1. King,  2. Queen, 3. Rook, 4.Bishop, 5.Knight, 6.Pawn
// left, right, top, bottom, top-left, top-right, bottom-left, bottom-right
public class Pawn : ChessStone
{
    // 1.pawn은 적이 있는 경우 대각선으로만 공격 가능
    // 2.pawn은 앞으로만 전진 가능
    // 3.pawn은 처음에 2칸 전진 가능
    // 4.pawn은 상대 진영 도착시 나의 죽은 말을 pawn의 목숨대신 살릴수 있음
    // top, top-left, top-right 끝
    ChessManager GameManager;
    SpawnChessStone spawnChessStone;
    Animator _anim;
    bool initPawn = true;

    private void Start() {
        SetPawn();
    }
    void SetPawn()
    {
        gameObject.name = "pawn";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        spawnChessStone = GameObject.Find("GameManager").GetComponent<SpawnChessStone>();
        isSequence = false;
        _anim = GetComponent<Animator>();
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }
    public override void DefaultMove(possibleMove action, List<KeyValuePair<int,int>> possibleMove)
    {
        ChangeInitPawn();
        PawnLogic();
    }
    void PawnLogic()
    {
        if(initPawn)
        {
            FirstMove(); // 밑에 공격 루틴도 가야되는데..
        }
        else
        {
            // default
            Move();
        }
        // 공격
        AttackMove();
    }
    void AttackMove()
    {                    
        int dir = turn == 1 ? -1 : 1;
        int diagonal = -1;
        for(int i = 0; i < 2; i++)
        {
            if(GameManager.IsPossibleMove(m_row + dir, m_col + diagonal,turn) == (int)MoveKind.enemy)
                GameManager.moveList.Add(new KeyValuePair<int, int>
                (m_row + dir, m_col + diagonal));
            diagonal = 1;
        }
    }
    void Move()
    {
        int dir = turn == 1 ? -1 : 1; 
        // white
        if(GameManager.IsPossibleMove(m_row + dir, m_col,turn) == (int)MoveKind.move)
            GameManager.moveList.Add(new KeyValuePair<int, int>
            (m_row + dir, m_col));
    }
    void ChangeInitPawn()
    {        
        if(!initPawn) return;
    
        if(m_row != 6 && turn == 1) 
        {
            initPawn = false;
        }
        if(m_row != 1 && turn == 2)
        {
            initPawn = false;
        }
        
    }
    void FirstMove()
    {
        int dir = turn == 1 ? -1 : 1;
        
        for(int i = 1; i <= 2; i++)
        {                
            if(GameManager.IsPossibleMove(m_row + (i * dir), m_col,turn) != (int)MoveKind.move) break; // 연속성이 깨짐
            else if (GameManager.IsPossibleMove(m_row + (i * dir), m_col,turn) == (int)MoveKind.move) 
            GameManager.moveList.Add(new KeyValuePair<int, int>
            (m_row + (i * dir), m_col));
        }
    }
}
