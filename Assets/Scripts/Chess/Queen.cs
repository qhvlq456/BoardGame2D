using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class Queen : ChessStone
{    
    // {T,B,L,R,TL,TR,BL,BR};
    ChessManager GameManager;
    Animator _anim;    

    private void Start() {
        SetQueen();
    }    
    void SetQueen()
    {
        gameObject.name = "queen";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        _anim = GetComponent<Animator>();
        isSequence = true;
        SetDirection('/',"top","bottom","left","right","top/left","top/right","bottom/left","bottom/right");
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }
    // protected enum MoveKind {none,same,move,enemy}
    //                          0     1   2     3
    // sr, sc 원래 값으로 초기화 해줘야 함!
    
}
