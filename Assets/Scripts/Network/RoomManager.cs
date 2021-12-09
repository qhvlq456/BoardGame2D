using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon;
using utility = Photon.Pun.UtilityScripts;

public class RoomManager : MonoBehaviourPunCallbacks
{
    protected Room room;
    [SerializeField]
    Text roomNameText;
    [SerializeField]    
    GameObject readyData;
    [SerializeField]
    Transform readyDataParent;    

    public List<ReadyData> readyList = new List<ReadyData>();
    public virtual void Awake() {
        room = PhotonNetwork.CurrentRoom;
        SetManager();
    }
    public virtual void SetManager()
    {
        PlayerEnter();
        roomNameText.text = $"Room Name : {room.Name}";
    }
    public void SyncScene()
    {
        photonView.RPC("RpcSyncScene",RpcTarget.All);
    }
    [PunRPC]
    public void RpcSyncScene()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.LogError("What!!");
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // 나자신은 포함 안되는구나 ㅎ
    {
        PlayerEnter();
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        PlayerLeft(otherPlayer.NickName);
    }
    public override void OnLeftRoom() // 이미 접속했던 플레이어가 나가는 순간 실행되는 콜백 메소드(나 자신이 떠나는 경우에만 실행됨)
    {
        SceneManager.LoadScene("Lobby"); // 이 경우 포톤네트워크를 사용하게 된다면 모든 사람이 나가게 될 수 있음으로 SceneManager를 사용한다
    }
    #region Enter and Left Player
    public virtual void PlayerEnter() // 현재 room에 존재하는 모든 player들의 이름을 가져와야 되는데ㅋㅋㅋ 내가 이걸 왜 rpc로 햇지라고 일단 생각 하엿음
    {
        Debug.LogError($"player length = {PhotonNetwork.PlayerList.Length}"); // 요게 ㅈㄴ 문제네

        foreach(var player in PhotonNetwork.PlayerList)
        {
            var count = readyList.Count(r => r.player.NickName == player.NickName);
            if(count > 0) continue;

            ReadyData _readyUI = 
            Instantiate(readyData,readyDataParent).GetComponent<ReadyData>();
            readyList.Add(_readyUI);
            _readyUI.player = player;
        }
    }
    public virtual void PlayerLeft(string name)
    {
        foreach(var ready in readyList)
        {
            if(ready.player.NickName == name)
            {
                readyList.Remove(ready);
                Destroy(ready.gameObject);
                break;
            }
        }
    }
    #endregion
    #region Player Ready
    public void ReadyPlayer(string name,bool value)
    {
        photonView.RPC("RpcReadyPlayer",RpcTarget.AllViaServer,name,value);
    }
    [PunRPC]
    void RpcReadyPlayer(string name, bool value)
    {
        foreach(var ready in readyList)
        {
            if(ready.player.NickName == name)
            {
                ready.isReady = value;
            }
        }
    }
    #endregion
    
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }    
    
    public virtual void GameSceneLoad()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel((string)room.CustomProperties["k"]);
        }
    }
    
}
