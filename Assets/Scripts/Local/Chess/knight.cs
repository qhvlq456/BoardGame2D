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
        SetKnight();
    }    
    void SetKnight()
    {
        gameObject.name = "knight";
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        _anim = GetComponent<Animator>();
        isSequence = false;
        SetDirection('/',"top/left/left","bottom/left/left","bottom/right/right",
        "top/right/right","top/top/left","top/top/right","bottom/bottom/left","bottom/bottom/right");
    }
    public override void IsCheck()
    {
        base.IsCheck();
        _anim.SetBool("isCheck",isCheck);
    }      
}
