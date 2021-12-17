using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

// 여기서 직접 알아서 해야되넹
public class NetworkConcaveStone : MonoBehaviourPun
{    
    PhotonView pv;
    SpriteRenderer _renderer;
    OmokManager GameManager;
    public Sprite[] _sprite;
    public Text _text;
    void Awake() {
        SetConcaveStone();
        UpdateText();
        SetTextColor();
        SetImageStone();
    }
    private void Update() {
        //if(GameManager.isGameOver) Destroy(gameObject);
    }
    void SetConcaveStone()
    {
        pv = GetComponent<PhotonView>();
        _renderer = GetComponent<SpriteRenderer>();
        GameManager = GameObject.Find("GameManager").GetComponent<OmokManager>();
    }
    #region Update Text  
    void UpdateText()
    {
        ++StaticVariable.sequneceNum;
        _text.text = StaticVariable.sequneceNum.ToString();
    }
    public void SetTextColor()
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
    public void SetImageStone()
    {
        // 1. white, 2. black
        //Debug.LogError($"Stone Turn = {GameManager.turn}");
        _renderer.sprite = _sprite[GameManager.turn - 1];
    }
    #endregion
}    
