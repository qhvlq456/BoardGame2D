using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Res_2D_BoardGame;

// 여기서 직접 알아서 해야되넹
public class NetworkConcaveStone : Stone
{    
    protected PhotonView pv;
    protected BoardGameBase GameManager;
    public Text _text;
    public virtual void Awake() {
        SetStone();
        UpdateText();
        SetTextColor();
        SetImageType();
    }
    private void Update() {
        //if(GameManager.isGameOver) Destroy(gameObject);
    }
    public virtual void SetStone()
    {
        pv = GetComponent<PhotonView>();
        _renderer = GetComponent<SpriteRenderer>();
        GameManager = GameObject.Find("GameManager").GetComponent<BoardGameBase>();
    }
    #region Update Text  
    void UpdateText()
    {
        ++StaticVariable.sequneceNum;
        _text.text = StaticVariable.sequneceNum.ToString();
    }
    public virtual void SetTextColor()
    {
        // 1. white = 1, 2. black = 2
        //Debug.LogError($"text color turn = {GameManager.turn}");
        if(GameManager.turn == 1)
        {
            _text.color = Color.black;
        }
        else _text.color = Color.white;
    }
    #endregion
    
    #region Update Image
    public override void SetImageType()
    {
        // 1. white, 2. black
        //Debug.LogError($"Stone Turn = {GameManager.turn}");
        _renderer.sprite = _sprite[GameManager.turn - 1];
    }
    #endregion
}    
