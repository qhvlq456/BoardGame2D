using UnityEngine;
using UnityEngine.UI;
using Res_2D_BoardGame;

public class ConcaveStone : Stone
{    
    public Text _text;    
    public virtual void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
    }
    void Start() {
        ++StaticVariable.sequneceNum;
        _text.text = StaticVariable.sequneceNum.ToString();
    }
    public virtual void Update() {
        SetTextColor();
    }
    public void SetTextColor()
    {        
        // 1. white = 1, 2. black = 2
        if(stoneType == EPlayerType.white)
        {
            _text.color = Color.black;
        }
        else _text.color = Color.white;
    }
}
