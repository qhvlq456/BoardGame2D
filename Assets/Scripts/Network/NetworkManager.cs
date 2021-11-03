using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "1";
    GameObject canvas;

    [Header("DisconnetPanel")]
    public GameObject disconnetPanel;
    public Text disconnectTitleText;
    public InputField input;
    Button enterButton;

    [Header("ConnetPanel")]
    public GameObject connetPanel;
    public Text nickNameText;
    Text currentInfoText;
    public string roomName;

    List<Button> gameButton = new List<Button>();

    [Header("Common")]
    public byte maxPlayer = 2;

    private void Start() {
        SetGame();
    }
    void SetGame()
    {
        canvas = GameObject.Find("Canvas");
        input = GameObject.Find("NickNameInput").GetComponent<InputField>();
        enterButton = GameObject.Find("EnterBtn").GetComponent<Button>();

        currentInfoText = connetPanel.transform.Find("CurrentConnectionText").GetComponent<Text>();
        
        for(int i = 0; i < connetPanel.transform.Find("GameBtnPanel").childCount; i++)
        {
            gameButton.Add(connetPanel.transform.Find("GameBtnPanel").GetChild(i).GetComponent<Button>());
        }
    }

    // DisconnectPanel status
    public void EnterConnectPanel() // connect button event 함수
    {
        if(input.text == "") 
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
        connetPanel.SetActive(true);

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(); // master server 연결
        OnSelectButton(false);
    }

    // ConnectPanel status
    public void OnSelectButton(bool isTrue)
    {        
        foreach(var button in gameButton)
        {
            button.interactable = isTrue;
        }
    }
    public void OnClick() // mount button event
    {
        OnSelectButton(false);
        Connect();
    }
    // public void SelectGame(int idx) // mount button event // 이게 이유야..? 버튼에 이벤트 두개 너으면 안되는 거엿어??
    // {        
    //     if(idx == (int)Games.OMOK) roomName = "Omok";
    //     else if (idx == (int)Games.OTHELLO) roomName = "Othello";
    //     else roomName = "Chess";
    // }
    public void SelectGame(string gameName) // mount button event // 이게 이유야..? 버튼에 이벤트 두개 너으면 안되는 거엿어??
    {        
        roomName = gameName;
    }
    public void Connect() // mount button event
    {
        if(PhotonNetwork.IsConnected)
        {
            currentInfoText.text = "Connecting to random room...";
            PhotonNetwork.JoinRandomRoom(); // 현재 존재하고있는 렌덤 룸에 진입
        }
        else
        {
            currentInfoText.text = "Offline : Connection Disabled - Try reconnectind...";
            PhotonNetwork.ConnectUsingSettings(); // 재연결 시도
        }
    }
    public override void OnConnectedToMaster() // master sever 접속시 호출되는 callback함수
    {
        OnSelectButton(true);
        currentInfoText.text = "Online : Connected To Master Server";
    }
    public override void OnDisconnected(DisconnectCause cause) // master server 연결 실패시 호출되는 callback함수
    {
        OnSelectButton(false);
        currentInfoText.text = $"Offline : Connection Disabled {cause.ToString()} - Try reconnectind...";
        PhotonNetwork.ConnectUsingSettings(); // master server 재접속 시도
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print(message);
        currentInfoText.text = "There is no empty room, Creating new room...";
        PhotonNetwork.CreateRoom(roomName,new RoomOptions{MaxPlayers = maxPlayer}, null); // 방 참가에 실패하였으면 새로운 방을 만듬
    }
    public override void OnJoinedRoom() // room에 참여 하였을때 호출되는 callback함수
    {
        currentInfoText.text = "Connected new room";
        PhotonNetwork.LoadLevel(roomName);
        // scenemanger.LoadScene쓰면 동기화가 되지않는다 하지만 PhotonNetwork.LoadLevel("Main")를 사용하면 Host를 통하여 동기화가 된다
    }
    
}
