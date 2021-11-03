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
    enum Dir {T,TL,TR,B,BL,BR};
    int[,] direction = new int[,] {{-1,0},{-1,-1},{-1,1},{1,0},{1,-1},{1,1}};
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
        _anim = GetComponent<Animator>();
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }
    public override void CheckMove()
    {
        ChangeInitPawn();
        White();
        Black();        
    }    
    void White()
    {
        if(turn != (int)StoneType.white) return;
        else if(initPawn)
        {
            InitMove(); // 밑에 공격 루틴도 가야되는데..
        }
        else
        {
            // default
            DefaultMove();
        }
        // 공격
        AttackMove();
    }
    void Black()
    {
        if(turn != (int)StoneType.black) return;
        else if(initPawn)
        {
            InitMove();
        }
        else
        {
            DefaultMove();
        }
        AttackMove();
    }
    // protected enum MoveKind {none,move,enemy,same}
    void AttackMove()
    {                    
        if(turn == (int)StoneType.white)
        {
            // white
            if(GameManager.IsPossibleMove(m_row + direction[(int)Dir.TL,0], m_col + direction[(int)Dir.TL,1],turn) == 3)
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + direction[(int)Dir.TL,0], m_col + direction[(int)Dir.TL,1]));

            if(GameManager.IsPossibleMove(m_row + direction[(int)Dir.TR,0], m_col + direction[(int)Dir.TR,1],turn) == 3)
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + direction[(int)Dir.TR,0], m_col + direction[(int)Dir.TR,1]));
        }
        else
        {
            // black 
            if(GameManager.IsPossibleMove(m_row + direction[(int)Dir.BL,0], m_col + direction[(int)Dir.BL,1],turn) == 3)
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + direction[(int)Dir.BL,0], m_col + direction[(int)Dir.BL,1]));

            if(GameManager.IsPossibleMove(m_row + direction[(int)Dir.BR,0], m_col + direction[(int)Dir.BR,1],turn) == 3)
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + direction[(int)Dir.BR,0], m_col + direction[(int)Dir.BR,1]));
        }
    }
    void DefaultMove()
    {        
        if(turn == (int)StoneType.white)
        {
            // white
            if(GameManager.IsPossibleMove(m_row + direction[(int)Dir.T,0], m_col,turn) == 2)
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + direction[(int)Dir.T,0], m_col));
        }
        else
        {
            // black
            if(GameManager.IsPossibleMove(m_row + direction[(int)Dir.B,0], m_col,turn) == 2)
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + direction[(int)Dir.B,0], m_col));
        }
    }
    // protected enum MoveKind {none,same,move,enemy}
    void ChangeInitPawn()
    {
        if(turn == (int)StoneType.white)
        {
            if(m_row != 6) initPawn = false;
        }
        else
        {
            if(m_row != 1) initPawn = false;
        }
    }
    void InitMove()
    {        
        if(turn == (int)StoneType.white)
        {
            // white
            for(int i = 1; i <= 2; i++)
            {                
                if(GameManager.IsPossibleMove(m_row - i, m_col,turn) != 2) break;
                else if (GameManager.IsPossibleMove(m_row - i, m_col,turn) == 2) 
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row - i, m_col));
            }
        }
        else
        {        
            // black
            for(int i = 1; i <= 2; i++)
            {
                if(GameManager.IsPossibleMove(m_row + i, m_col,turn) != 2) break;
                else if (GameManager.IsPossibleMove(m_row + i, m_col,turn) == 2) 
                GameManager.list.Add(new KeyValuePair<int, int>
                (m_row + i, m_col));
            }
        }
    }
}
