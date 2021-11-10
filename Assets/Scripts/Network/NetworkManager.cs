using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

// 이걸 싱글톤으로 바꿔야 되는건가..?
public class NetworkManager : MonoBehaviourPunCallbacks
{
    // RoomInfo : photon realtime의 방정보 기능
    private readonly string gameVersion = "1";
    GameObject canvas;

    [Header("DisconnetPanel")]
    public GameObject disconnetPanel;
    public Text disconnectTitleText;
    public InputField input;
    Button disconnectionEnterButton;

    [Header("LobbyPanel")]
    public GameObject lobbyPanel;
    public Text nickNameText;
    Text currentRoomInfoText;
    Text currentConnectionStatusText;
    public List<Button> lobbyGameEnterButton = new List<Button>();
    public List<Button> lobbyOptionKindButton = new List<Button>();
    public Button exitButton;

    
    [Header("TempPanel")]
    public GameObject tempPanel;
    public Button tempExitBtn;

    [Header("RoomList")]
    public List<RoomInfo> m_roomList = new List<RoomInfo>();   

    private void Start() {
        SetGame();
        PhotonNetwork.AutomaticallySyncScene = true; 
        // 룸 안의 모든 접속한 클라이언트에 대해 이 레벨 로드를 유니티가 직접 하는 것이 아닌 Photon 이 하도록 하였습니다.
        // 즉, 룸에 접속시 자동으로 해당 씬으로 이동함
    }
    void SetGame()
    {
        canvas = GameObject.Find("Canvas");
        input = disconnetPanel.transform.Find("NickNameInput").GetComponent<InputField>();
        disconnectionEnterButton = disconnetPanel.transform.Find("EnterBtn").GetComponent<Button>();

        nickNameText = lobbyPanel.transform.Find("NickNameText").GetComponent<Text>();
        currentConnectionStatusText = lobbyPanel.transform.Find("ConnectionStatusText").GetComponent<Text>();        
        currentRoomInfoText = lobbyPanel.transform.Find("CurrentRoomInfo").GetComponent<Text>();

        // lobby panel find child btn object
        for(int i = 0; i < lobbyPanel.transform.Find("RoomEnterPanel").childCount;i++)
        {
            lobbyGameEnterButton.Add(lobbyPanel.transform.Find("RoomEnterPanel").GetChild(i).GetComponent<Button>());
        }
        for(int i = 0; i < lobbyPanel.transform.Find("SelectOptionPanel").childCount;i++)
        {
            lobbyOptionKindButton.Add(lobbyPanel.transform.Find("SelectOptionPanel").GetChild(i).GetComponent<Button>());
        }
        exitButton = lobbyPanel.transform.Find("ExitBtn").GetComponent<Button>();
    }

    #region DisconnectPanel status
    public void EnterConnectPanel() // connect button event 함수
    {
        if(string.IsNullOrEmpty(input.text)) 
        {
            disconnectTitleText.text = "Please input nickname...";
            return;
        }
        EnterPlayer();
    }

    public void EnterPlayer()
    {
        PhotonNetwork.LocalPlayer.NickName = input.text;
        nickNameText.text = $"Your name : {PhotonNetwork.LocalPlayer.NickName}";

        disconnetPanel.SetActive(false);
        lobbyPanel.SetActive(true);

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); // master server 연결
        OnSelectButton(false);
    }

    public override void OnConnectedToMaster() // master sever 접속시 호출되는 callback함수
    {
        OnSelectButton(true);
        PhotonNetwork.JoinLobby();
        currentConnectionStatusText.text = "Online : Connected To Master Server";
    }

    public override void OnJoinedLobby()
    {
        print("lobby 입장");
    }
    #endregion

    // ConnectPanel status
    public void OnSelectButton(bool isTrue)
    {        
        var buttons = new List<Button>();
        buttons.AddRange(lobbyGameEnterButton);
        buttons.AddRange(lobbyOptionKindButton);
        buttons.Add(exitButton);

        foreach(var button in buttons)
        {
            button.interactable = isTrue;
        }
    }        

    #region Join Button
    public void JoinRoomButton(int idx)
    {
        PhotonNetwork.JoinRoom(m_roomList[idx].Name);
        Debug.LogError($"{m_roomList[idx].Name} Enter");
        RoomListUpdate();
    }
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();
    #endregion

    #region Update List Room  / Photon override All server in update information

    // OnRoomListUpdate
    // -room의 변동상황들만 매개변수로 들어옴
    // -로비에 접속 시 # join lobby 
    // -새로운 룸이 만들어질 경우 // create room
    // -룸이 삭제되는 경우 // delete room
    // -룸의 IsOpen 값이 변화할 경우(아예 RoomInfo 내 데이터가 바뀌는 경우 전체일 수도 있습니다) // full room count
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // m_roomList를 컨트롤하여 button의 index말고 새롭게 text를 업데이트
        // 1. join state, 2. create state, 3. delete state 
        // 위 상태일 때만 m_Roomlist또한 동기화 해주어 RoomListUpdate()를 호출함
        
        int listCount = roomList.Count;

        for(int i = 0; i < listCount; i++)
        {
            // 삭제 되었다면
            if(roomList[i].RemovedFromList)
            {
                m_roomList.RemoveAt(m_roomList.IndexOf(roomList[i])); // delete room
            }
            else // update, add
            {
                if(!m_roomList.Contains(roomList[i])) m_roomList.Add(roomList[i]); // add
                else
                {
                    m_roomList[m_roomList.IndexOf(roomList[i])] = roomList[i]; // update
                    // 아 roomlist가 포함되어 있다면 roomList가 포함된 m_roomList인덱스를 찾아서 해당 roomList를 집어넣는구나
                }
            }
        }
        RoomListUpdate();
    }
    
    void RoomListUpdate()
    {
        int btnMaxNum = lobbyPanel.transform.Find("RoomEnterPanel").childCount;
         Debug.Log($"listCount. {m_roomList.Count}");
        for(int i = 0; i < m_roomList.Count; i++)
        {
            lobbyGameEnterButton[i].interactable = true;
            Debug.LogError($"{m_roomList[i].Name} \n {m_roomList[i].PlayerCount} / {m_roomList[i].MaxPlayers}");

            lobbyGameEnterButton[i].transform.GetChild(0).GetComponent<Text>().text = 
            $"{m_roomList[i].Name} \n {m_roomList[i].PlayerCount} / {m_roomList[i].MaxPlayers}";
        }

        for(int i =  m_roomList.Count; i < btnMaxNum; i++)
        {
            lobbyGameEnterButton[i].interactable = false;
                
            lobbyGameEnterButton[i].transform.GetChild(0).GetComponent<Text>().text = "";
        }
    }
    #endregion

    #region Disconnect status
    public void Disconnect() => PhotonNetwork.Disconnect(); // 아에 master server까지 disconnected 하는거구나
    public override void OnDisconnected(DisconnectCause cause) // master server 연결 실패시 호출되는 callback함수
    {
        OnSelectButton(false);
        lobbyPanel.SetActive(false);
        disconnetPanel.SetActive(true);

        Debug.LogError($"Disconnected");
        currentConnectionStatusText.text = $"Offline : Connection Disabled {cause.ToString()} - Try reconnectind...";
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom(); 
    public override void OnLeftRoom() // 방을 떠났을때 호출되는 callback 함수
    {
        SceneManager.LoadScene("Lobby");
    }
    // 이 메소드는 명시적으로 플레이어를 Photon Network 룸에서 나가도록 하며 추상화를 위해서 public 메소드로 wrap 하였습니다. 
    #endregion    
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);
        currentConnectionStatusText.text = "There is no empty room, Please Create Room...";
        //PhotonNetwork.CreateRoom(gameKind,new RoomOptions{MaxPlayers = maxPlayer}, null); // 방 참가에 실패하였으면 새로운 방을 만듬
    }
    public override void OnJoinedRoom() // room에 참여 하였을때 호출되는 callback함수
    {
        Debug.Log("Enter Room!!");
        currentConnectionStatusText.text = "Connected new room";
        //PhotonNetwork.LoadLevel(gameKind);
        // scenemanger.LoadScene쓰면 동기화가 되지않는다 하지만 PhotonNetwork.LoadLevel("Main")를 사용하면 Host를 통하여 동기화가 된다
    }
    
}
