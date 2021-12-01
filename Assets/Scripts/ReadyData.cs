using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class ReadyData : MonoBehaviour
{
    public Photon.Realtime.Player player;
    public bool isReady;
    RoomManager roomManager;

    [SerializeField]
    Text playerNameText;
    [SerializeField]
    Button readyButton;
    bool isMasterValue;

    private void Awake() {
        isReady = false;
        isMasterValue = false;
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }    
    private void Start() {
        StartCoroutine(SetPlayerVisibleButton());
    }
    void SetPlayerText(string name)
    {
        playerNameText.text = name;
    }
    
    void OnClickReady()
    {
        if(!player.IsLocal) return;
        
        IsMasterGameStart();

        if(player.IsMasterClient && isMasterValue && roomManager.readyList.Count >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            roomManager.GameStart();
        }
        else
        {
            if(player.IsMasterClient) return;
        }
        isReady = !isReady;
        roomManager.ReadyPlayer(player.NickName,isReady);
    }
    void IsMasterGameStart()
    {
        if(!player.IsMasterClient) return;

        isMasterValue = true;
        
        foreach(var ready in roomManager.readyList)
        {
            if(!ready.player.IsMasterClient) 
            {
                if(!ready.isReady)
                {
                    isMasterValue = ready.isReady;
                    break;
                }        
            }
        }
    }
    IEnumerator SetPlayerVisibleButton()
    {
        while(true)
        {
            if(player.IsMasterClient)
            {
                SetPlayerText(string.Format("{0} ({1})",player.NickName,"master"));
                readyButton.transform.GetChild(0).GetComponent<Text>().text = "Start";
            }
            else 
                SetPlayerText(string.Format("{0}",player.NickName));

            IsMasterGameStart();

            if(player.IsLocal && !player.IsMasterClient)
            {
                readyButton.image.color = Color.Lerp(Color.clear,new Color(0.95f,0.95f,0.95f,1),Mathf.PingPong(Time.time, 1));
            }
            else if(player.IsLocal && player.IsMasterClient && isMasterValue && 
            roomManager.readyList.Count >= PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                readyButton.image.color = Color.Lerp(Color.clear,new Color(0.95f,0.75f,0.45f,1),Mathf.PingPong(Time.time, 1));
            }

            if(isReady)
                readyButton.image.color = Color.green;
            
            yield return null;
        }
    }
}
