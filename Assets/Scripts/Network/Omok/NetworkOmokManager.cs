using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkOmokManager : MonoBehaviourPunCallbacks
{
    Room room;
    string playerPath;
    public Text roomNameText;
    public Text playerInfoText;
    private void Awake() {
        SetManager();

        PhotonNetwork.Instantiate(playerPath,new Vector3(0,0,0),Quaternion.identity);        
        Debug.Log($"NickName = {PhotonNetwork.LocalPlayer.NickName}");
        Debug.Log($"ActtorNumber = {PhotonNetwork.LocalPlayer.ActorNumber}");
        UpdateText();
    }

    #region LeaveRoom
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    public override void OnLeftRoom() // 이미 접속했던 플레이어가 나가는 순간 실행되는 콜백 메소드(나 자신이 떠나는 경우에만 실행됨)
    {
        SceneManager.LoadScene("Lobby"); // 이 경우 포톤네트워크를 사용하게 된다면 모든 사람이 나가게 될 수 있음으로 SceneManager를 사용한다
    }
    #endregion

    void SetManager()
    {
        roomNameText = GameObject.Find("RoomNameText").GetComponent<Text>();
        playerInfoText = GameObject.Find("PlayerInfoText").GetComponent<Text>();
        playerPath = "Network/Player";
        room = PhotonNetwork.CurrentRoom;
    }
    public void UpdateText()
    {
        //if(PhotonNetwork.PlayerList.Length < 2) return;
        Debug.Log("PlayerList Length  " + PhotonNetwork.PlayerList.Length);
        // if(!PhotonNetwork.IsMasterClient) return; // 이건 왜??

        photonView.RPC("RpcUpdateText",RpcTarget.All);
    }
    [PunRPC]
    void RpcUpdateText() // 현재 room에 존재하는 모든 player들의 이름을 가져와야 되는데ㅋㅋㅋ
    {
        roomNameText.text = room.Name;
        string playersStr = "";
        foreach(var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"너의 이름은 : {player.NickName}");
            playersStr += $"{player.NickName}\n";
        }
        playerInfoText.text = playersStr;
    }
}
