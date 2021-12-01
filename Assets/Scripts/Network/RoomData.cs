using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomData : MonoBehaviour
{    
    [SerializeField]
    Text titleText;
    [SerializeField]
    Button button;
    public RoomInfo roomInfo {private get; set;}
    private void Awake() {
        OnClickEnterRoom();
    }   
    private void Update()
    {
        titleText.text = 
        $"{roomInfo.Name} \n GameKind = {roomInfo.CustomProperties["k"]} \n {roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }
    public void OnClickEnterRoom()
    {
        button.onClick.AddListener(() => {
            PhotonNetwork.JoinRoom(roomInfo.Name);
        });
    }
}
