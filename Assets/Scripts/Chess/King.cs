using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class King : ChessStone
{
    // {T,B,L,R,TL,TR,BL,BR};
    int[,] direction = new int[,] {{-1,0},{1,0},{0,-1},{0,1},{-1,-1},{-1,1},{1,-1},{1,1}};
    int dirIdx = 0;
    ChessManager GameManager;
    Animator _anim;    

    private void Start() {
        SetGame();
    }    
    void SetGame()
    {
        gameObject.name = "king";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        _anim = GetComponent<Animator>();        
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }
    public override void CheckMove()
    {
        DefaultMove();
    }    
    //protected enum MoveKind {none,same,move,enemy}
    //                         0     1   2     3
    void DefaultMove()
    {        
        int sr = m_row; int sc = m_col;
        while(dirIdx < direction.GetLength(0))
        {
            sr += direction[dirIdx,0];
            sc += direction[dirIdx,1];            
            
            if(GameManager.IsPossibleMove(sr,sc,turn) < 2)
            {
                sr = m_row; sc = m_col;
            }            
            else
            {
                GameManager.list.Add(new KeyValuePair<int, int>(sr,sc));
                if(GameManager.IsPossibleMove(sr,sc,turn) != 2)
                {
                    sr = m_row; sc = m_col;
                }
            }
            dirIdx++;
        }
        // init
        dirIdx = 0;
    }   
}
