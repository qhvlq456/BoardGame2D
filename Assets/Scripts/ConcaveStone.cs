using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Res_2D_BoardGame;

public class ConcaveStone : Stone
{    
    public Text _text;    

    void Start() {
        ++StaticVariable.sequneceNum;
        _text.text = StaticVariable.sequneceNum.ToString();
    }
    void Update() {
        SetTextColor2();
    }
    public void SetTextColor()
    {        
        // 1. white = 1, 2. black = 2
        if(turn == 1)
        {
            _text.color = Color.black;
        }
        else _text.color = Color.white;
    }
    public void SetTextColor2()
    {        
        // 1. white = 1, 2. black = 2
        if(playerType == EPlayerType.white)
        {
            _text.color = Color.black;
        }
        else _text.color = Color.white;
    }
}
