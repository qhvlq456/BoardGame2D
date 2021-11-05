using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class Bishop : ChessStone
{
    // {TL,TR,BL,BR};
    //int[,] direction = new int[,] {{-1,-1},{-1,1},{1,-1},{1,1}};
    //int dirIdx = 0;
    ChessManager GameManager;
    Animator _anim;        

    private void Start() {
        SetBishop();
    }    
    void SetBishop()
    {
        gameObject.name = "bishop";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        _anim = GetComponent<Animator>();
        isSequence = true;
        SetDirection('/',"top/left","top/right","bottom/left","bottom/right");
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }
    //protected enum MoveKind {none,same,move,enemy}
    //                          0     1   2     3  
}
