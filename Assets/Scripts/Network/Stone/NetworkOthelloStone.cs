using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkOthelloStone : NetworkConcaveStone,IPunObservable
{
    public override void Awake() {
        base.Awake();
        var manager = GameObject.Find("GameManager").GetComponent<OthelloManager>();
        manager.saveStones.Add(this);
    }
    private void Start() {
        //Debug.LogError($"r = {m_row}, c = {m_col}, turn = {m_turn}, type = {stoneType}");
    }
    private void Update() {
        SetTextColor();
        SetImageType();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsReading)
        {
            m_row = (int)stream.ReceiveNext();
            m_col = (int)stream.ReceiveNext();
            m_turn = (int)stream.ReceiveNext();
            stoneType = (EPlayerType)stream.ReceiveNext();
            
            // 여기서 stoneType 변경
        }
        else if(stream.IsWriting)
        {
            stream.SendNext(m_row);
            stream.SendNext(m_col);
            stream.SendNext(m_turn);
            stream.SendNext(stoneType);
            
            // 여기서 stoneType변경
        }
    }
    public override void SetTextColor()
    {   
        if(stoneType == EPlayerType.white)
        {
            _text.color = Color.black;
        }
        else _text.color = Color.white;
    }
    public override void SetImageType()
    {
        _renderer.sprite = _sprite[(int)stoneType];
    }
    // # region Initialization Stone    
    // IPunInstantiateMagicCallback
    // public void OnPhotonInstantiate(PhotonMessageInfo info) // 받은게 없어서 그렇구나 client 202에러나 확인하자'' 아 client측에서도 이 stone은 생성되는데 player custom property가 없어서 생긴 이슈임
    // {
    //     Debug.Log($"enter info name = {info.Sender.NickName}");
    //     m_row = (int)info.Sender.CustomProperties["r"];
    //     m_col = (int)info.Sender.CustomProperties["c"];
    //     m_turn = (int)info.Sender.CustomProperties["turn"];
    //     stoneType = (EPlayerType)info.Sender.CustomProperties["type"];
    //     //Debug.Log($"row = {m_row}\n col = {m_col}\n turn = {m_turn}\n Type = {stoneType}");
    // }
    // #endregion
}