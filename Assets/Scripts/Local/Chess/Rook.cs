using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;
public class Rook : ChessStone
{
    // {T,B,L,R};
    ChessManager GameManager;
    Animator _anim;        

    private void Start() {
        SetRook();
    }    
    void SetRook()
    {
        gameObject.name = "rook";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        _anim = GetComponent<Animator>();
        isSequence = true;
        SetDirection('/',"top","bottom","left","right");
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }
    //protected enum MoveKind {none,same,move,enemy}
    //                          0     1   2     3
    
}
