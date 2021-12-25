using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviourPun
{    
    [SerializeField]
    Text titleText;
    [SerializeField]
    Button button;
    public RoomInfo roomInfo {private get; set;}
    string roomKind;
    private void Awake() {
        OnClickEnterRoom();
    }   
    private void Start() {
        SetRoomKind();
    }
    private void Update()
    {
        titleText.text = 
        $"{roomInfo.Name} \n GameKind = {roomKind} \n {roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }
    void SetRoomKind()
    {
        int kind = (int)roomInfo.CustomProperties["gameKind"];
        switch(kind)
        {
            case 0 : roomKind = "Omok"; break;
            case 1 : roomKind = "Othello"; break;
        }
    }
    public void OnClickEnterRoom()
    {
        button.onClick.AddListener(() => {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        });
    }
}
