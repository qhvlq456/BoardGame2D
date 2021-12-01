using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Res_2D_BoardGame;
using System.Linq;
using ExitGames.Client.Photon;
using utility = Photon.Pun.UtilityScripts;
// 그래 이건 ui or network room manager 역할만 해야되는거야
public class RoomManager : MonoBehaviourPunCallbacks
{
    Room room;
    string playerPath;
    public Text roomNameText;
    public Text playerInfoText;
    Button backButton;
    [SerializeField]
    GameObject readyData;
    BoardGameBase GameManager;
    [SerializeField]
    SpawnManager spawnManager;
    public List<ReadyData> readyList = new List<ReadyData>();

    
    private void Awake() {
        Debug.LogError("Retry?");
        SetManager();
        roomNameText.text = $"Room Name : {room.Name}";
        photonView.RPC("RpcPlayerEnter",RpcTarget.AllViaServer);
    }
    void SetManager()
    {
        roomNameText = GameObject.Find("RoomNameText").GetComponent<Text>();
        playerInfoText = GameObject.Find("PlayerInfoText").GetComponent<Text>();
        playerPath = "Network/Player";

        room = PhotonNetwork.CurrentRoom;
        
        backButton = GameObject.Find("Canvas").transform.Find("BackBtn").GetComponent<Button>();
        GameManager = GameObject.Find("GameManager").GetComponent<BoardGameBase>(); // omok, othello, chess 를 사용하기위해 클래스 다향성 사용

        PhotonNetwork.Instantiate(playerPath,new Vector3(0,0,0),Quaternion.identity);
    }
    #region LeaveRoom
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
        
    
    public override void OnLeftRoom() // 이미 접속했던 플레이어가 나가는 순간 실행되는 콜백 메소드(나 자신이 떠나는 경우에만 실행됨)
    {
        SceneManager.LoadScene("Lobby"); // 이 경우 포톤네트워크를 사용하게 된다면 모든 사람이 나가게 될 수 있음으로 SceneManager를 사용한다
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        Debug.LogError("Change Owner = " + newMasterClient.NickName);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        photonView.RPC("RpcPlayerLeft",RpcTarget.AllViaServer,otherPlayer.NickName);
        StartCoroutine(LeftPlayerRoutine(otherPlayer));
    }
    IEnumerator LeftPlayerRoutine(Photon.Realtime.Player other) // 여기도 scene load 다시해야함;;
    {
        while(!GameManager.isGameOver)
        {
            var players = FindObjectsOfType<Player>();

            foreach(var player in players)
            {
                if(player.GetComponent<PhotonView>().Owner == other)
                    PhotonNetwork.DestroyPlayerObjects(other);
                else
                {
                    if(player.m_turn != GameManager.turn) GameManager.NextTurn();
                    player.DestroyStones();
                }
            }
            GameManager.GameOver(); // 여기서 한번
            yield return null;
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // 나자신은 포함 안되는구나 ㅎ
    {
        roomNameText.text = $"Room Name : {room.Name}";
        photonView.RPC("RpcPlayerEnter",RpcTarget.AllViaServer);
    }
    #endregion
    #region Enter and Left Player
    [PunRPC]
    void RpcPlayerEnter() // 현재 room에 존재하는 모든 player들의 이름을 가져와야 되는데ㅋㅋㅋ 내가 이걸 왜 rpc로 햇지라고 일단 생각 하엿음
    {
        Debug.LogError($"player length = {PhotonNetwork.PlayerList.Length}");

        foreach(var player in PhotonNetwork.PlayerList)
        {
            var count = readyList.Count(r => r.player.NickName == player.NickName);
            if(count > 0) continue;

            ReadyData _readyUI = 
            Instantiate(readyData,GameObject.Find("ReadyUI").transform.Find("Panel").transform.Find("ReadyObject")).GetComponent<ReadyData>();
            readyList.Add(_readyUI);

            _readyUI.player = player;
        }                
    }
    [PunRPC]
    void RpcPlayerLeft(string name)
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
    #region GameStartButtonEvent
    public void GameStart()
    {
        photonView.RPC("RpcGameStart",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcGameStart()
    {
        GameManager.GameStart();
        backButton.interactable = GameManager.isGameOver; // 나가지 못하게 만들기
    }
    #endregion

    #region GameStop .. GameScene Reload
    public void SceneLoadValue()
    {
        StartCoroutine(SceneLoad());

        photonView.RPC("RpcSceneLoadValue",RpcTarget.All);
    }
    [PunRPC]
    void RpcSceneLoadValue()
    {
        ++StaticVariable.practiceSceneLoadNum;
    }
    IEnumerator SceneLoad() // rpc도 아닌데 잘 되네?
    {
        while(StaticVariable.practiceSceneLoadNum < room.PlayerCount)
        {
            yield return null;
        }
        StaticVariable.practiceSceneLoadNum = 0;

        //PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

}
