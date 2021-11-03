using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class knight : ChessStone
{
    // {TLL,BLL,TRR,BRR,TRR,TTL,TTR,BBL,BBR};
    //int[,] direction = new int[,] {{-1,-2}, {1,-2}, {-1,2}, {1,2}, {-2,-1}, {-2,1}, {2,-1}, {2,1}};
    ChessManager GameManager;
    Animator _anim;    
    private void Start() {
        SetGame();
    }    
    void SetGame()
    {
        gameObject.name = "knight";
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
    void DefaultMove() // 이거 반복문으로 가능
    {        
        // left-top 
        if (GameManager.IsPossibleMove(m_row - 1, m_col - 2, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row - 1, m_col - 2));
        // left-bottom        
        if (GameManager.IsPossibleMove(m_row + 1, m_col - 2, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row + 1, m_col - 2));
        // right-top        
        if ( GameManager.IsPossibleMove(m_row - 1, m_col + 2, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row - 1, m_col + 2));
        // right-bottom        
        if ( GameManager.IsPossibleMove(m_row + 1, m_col + 2, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row + 1, m_col + 2));
        // top-left        
        if (GameManager.IsPossibleMove(m_row - 2, m_col - 1, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row - 2, m_col - 1));
        // top-right        
        if (GameManager.IsPossibleMove(m_row - 2, m_col + 1, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row - 2, m_col + 1));
        // bottom-left        
        if ( GameManager.IsPossibleMove(m_row + 2, m_col - 1, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row + 2, m_col - 1));
        // bottom-right
        if ( GameManager.IsPossibleMove(m_row + 2, m_col + 1, turn) >= 2) 
            GameManager.list.Add(new KeyValuePair<int, int>(m_row + 2, m_col + 1)); // -1,-2 안되는것들        
    }   
}
