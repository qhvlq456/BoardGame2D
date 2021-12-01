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
    public Button exitButton;

    [Header("RoomList")]
    public Dictionary<string,GameObject> m_Dic = new Dictionary<string, GameObject>();
    public bool isConnection;
    int maxRoomCount = 10;
    [SerializeField]
    GameObject roomPrefebs;
    [SerializeField]
    CanvasGroup canvasGroup;

    private void Awake() {
        SetGame();
        canvasGroup.interactable = true;
        isConnection = PhotonNetwork.IsConnected;
        PhotonNetwork.AutomaticallySyncScene = true;
        // 룸 안의 모든 접속한 클라이언트에 대해 이 레벨 로드를 유니티가 직접 하는 것이 아닌 Photon 이 하도록 하였습니다.
        // 즉, 룸에 접속시 자동으로 해당 씬으로 이동함
    }
    private void Start() {
    }
    private void Update() {
        currentConnectionStatusText.text = PhotonNetwork.NetworkClientState.ToString();
        currentRoomInfoText.text = $"{m_Dic.Count} / {maxRoomCount}";
    }
    void SetGame()
    {
        canvas = GameObject.Find("Canvas");
        input = disconnetPanel.transform.Find("NickNameInput").GetComponent<InputField>();
        disconnectionEnterButton = disconnetPanel.transform.Find("EnterBtn").GetComponent<Button>();

        nickNameText = lobbyPanel.transform.Find("NickNameText").GetComponent<Text>();
        currentConnectionStatusText = lobbyPanel.transform.Find("ConnectionStatusText").GetComponent<Text>();        
        currentRoomInfoText = lobbyPanel.transform.Find("CurrentRoomInfo").GetComponent<Text>();

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
        PhotonNetwork.LocalPlayer.NickName = input.text;
        EnterPlayer();
    }

    public void EnterPlayer()
    {
        nickNameText.text = $"Your name : {PhotonNetwork.LocalPlayer.NickName}";

        disconnetPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        if(isConnection) return;
        
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); // master server 연결
        canvasGroup.interactable = false;
    }

    public override void OnConnectedToMaster() // master sever 접속시 호출되는 callback함수
    {
        canvasGroup.interactable = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if(!isConnection) return;
        EnterPlayer();
    }
    #endregion            

    #region Join Button
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

        // for(int i = 0; i < listCount; i++)
        // {
        //     // 삭제 되었다면
        //     if(roomList[i].RemovedFromList)
        //     {
        //         m_roomList.RemoveAt(m_roomList.IndexOf(roomList[i])); // delete room // 아 이이슈인가? 찾았다 시발 ㅠㅠㅠㅠㅠㅠㅠ 하ㅏ아아..
        //     }
        //     else // update, add
        //     {
        //         if(!m_roomList.Contains(roomList[i])) m_roomList.Add(roomList[i]); // add
        //         else
        //         {
        //             m_roomList[m_roomList.IndexOf(roomList[i])] = roomList[i]; // update
        //             // 아 roomlist가 포함되어 있다면 roomList가 포함된 m_roomList인덱스를 찾아서 해당 roomList를 집어넣는구나
        //         }
        //     }
        // }
        for(int i = 0; i < listCount; i++)
        {
            GameObject tempRoom = null;

            if(roomList[i].RemovedFromList)
            {
                m_Dic.TryGetValue(roomList[i].Name,out tempRoom);
                Destroy(tempRoom);
                m_Dic.Remove(roomList[i].Name);
            }
            else
            {
                if(m_Dic.ContainsKey(roomList[i].Name)) // update
                {
                    m_Dic.TryGetValue(roomList[i].Name,out tempRoom);
                    m_Dic[roomList[i].Name].GetComponent<RoomData>().roomInfo = roomList[i];
                }
                else // add
                {
                    GameObject createRoom = Instantiate(roomPrefebs,lobbyPanel.transform.Find("RoomEnterPanel").transform);
                    createRoom.GetComponent<RoomData>().roomInfo = roomList[i];
                    m_Dic.Add(roomList[i].Name,createRoom);
                }
            }
        }
    }    
    #endregion

    #region Disconnect status
    public void Disconnect() => PhotonNetwork.Disconnect(); // 아에 master server까지 disconnected 하는거구나
    public override void OnDisconnected(DisconnectCause cause) // master server 연결 실패시 호출되는 callback함수
    {
        canvasGroup.interactable = false;
        lobbyPanel.SetActive(false);
        disconnetPanel.SetActive(true);

        currentConnectionStatusText.text = $"Offline : Connection Disabled {cause.ToString()} - Try reconnectind...";
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion    
    
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);
        currentConnectionStatusText.text = "There is no empty room, Please Create Room...";
    }
    
}
