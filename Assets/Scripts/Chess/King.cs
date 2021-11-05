using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class King : ChessStone
{
    // {T,B,L,R,TL,TR,BL,BR};
    ChessManager GameManager;
    Animator _anim;    

    private void Start() {
        SetKing();
    }    
    void SetKing()
    {
        gameObject.name = "king";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        _anim = GetComponent<Animator>();
        isSequence = false;
        SetDirection('/',"top","bottom","left","right","top/left","top/right","bottom/left","bottom/right");
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }    
}
