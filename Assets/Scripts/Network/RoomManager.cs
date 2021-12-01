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
    public bool isReady;
    public Text roomNameText;
    public Text playerInfoText;
    Button gameStartBtn;
    Button backButton;
    [SerializeField]
    GameObject readyData;
    BoardGameBase GameManager;
    [SerializeField]
    SpawnManager spawnManager;
    
    private void Awake() {
        Debug.LogError("Retry?");
        SetManager();
        InitGame(); // 아 retry때문에 이래야 됬구나;;
        roomNameText.text = $"Room Name : {room.Name}";
        photonView.RPC("RpcPlayerEnter",RpcTarget.AllViaServer);
    }
    void SetManager()
    {
        isReady = false;

        roomNameText = GameObject.Find("RoomNameText").GetComponent<Text>();
        playerInfoText = GameObject.Find("PlayerInfoText").GetComponent<Text>();
        playerPath = "Network/Player";

        room = PhotonNetwork.CurrentRoom;

        gameStartBtn = GameObject.Find("Canvas").transform.Find("GameMenu").Find("GameStartBtn").GetComponent<Button>();
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
        InitGame();
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
        InitGame();
        roomNameText.text = $"Room Name : {room.Name}";
        photonView.RPC("RpcPlayerEnter",RpcTarget.AllViaServer);
    }
    #endregion

    #region InitGame
    public void InitGame()
    {
        photonView.RPC("RpcInitGame",RpcTarget.MasterClient); // master에게만 줘도 되는구나~ 어차피 master기준이라
    }
    [PunRPC]
    public void RpcInitGame()
    {
        gameStartBtn.gameObject.SetActive(true);
        gameStartBtn.interactable = PhotonNetwork.PlayerList.Length >= room.MaxPlayers;
    }    
    [PunRPC]
    void RpcPlayerEnter() // 현재 room에 존재하는 모든 player들의 이름을 가져와야 되는데ㅋㅋㅋ 내가 이걸 왜 rpc로 햇지라고 일단 생각 하엿음
    {
        var readys = FindObjectsOfType<ReadyData>().ToList();
        foreach(var player in PhotonNetwork.PlayerList)
        {
            var count = readys.Count(r => r.player.NickName == player.NickName);
            if(count > 0) continue;

            ReadyData _readyUI = Instantiate(readyData,GameObject.Find("ReadyUI").transform.Find("Panel").transform.Find("ReadyObject")).GetComponent<ReadyData>();
            _readyUI.SetPlayerText(player.NickName);
            _readyUI.player = player;
        }                
    }
    [PunRPC]
    void RpcPlayerLeft(string name)
    {
        var readys = FindObjectsOfType<ReadyData>();
        
        foreach(var ready in readys)
        {
            if(ready.player.NickName == name)
            {
                Destroy(ready.gameObject);
                break;
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
        if(PhotonNetwork.IsMasterClient) // master client는 초기화작업
        {
            gameStartBtn.interactable = false;
            gameStartBtn.gameObject.SetActive(false);
        }

        GameManager.GameStart();
        backButton.interactable = GameManager.isGameOver; // 나가지 못하게 만들기
    }
    #endregion
    #region GameStop .. GameScene Reload
    public void GameSceneLoad() // 이게 해결 방법이 없는데..
    {
        if(!GameManager.isGameOver) return; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //photonView.RPC("RpcGameSceneLoad",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcGameSceneLoad()
    {
        Debug.LogError("what");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion


}
