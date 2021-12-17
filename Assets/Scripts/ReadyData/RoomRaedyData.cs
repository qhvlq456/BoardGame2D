using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomRaedyData : ReadyData
{
    [SerializeField]
    Button readyButton;
    bool isMasterValue;

    public override void Awake() {
        base.Awake();
        isReady = false;
        isMasterValue = false;        
    }
    
    public void OnClickReady()
    {
        if(!player.IsLocal) return;

        IsMasterGameStart();

        if(player.IsMasterClient && isMasterValue && PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {            
            roomManager.SyncScene();
            roomManager.GameSceneLoad();
        }
        else
        {
            isReady = !isReady;
            roomManager.ReadyPlayer(player.NickName,isReady);
        }
    }
    void IsMasterGameStart()
    {
        if(!player.IsMasterClient) return;

        isMasterValue = true;
        // 여기 세번호출됨 왜일까?
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
    public override void SetPlayerText()
    {
        if(player.IsMasterClient) readyButton.transform.GetChild(0).GetComponent<Text>().text = "Start";

        base.SetPlayerText();
        IsMasterGameStart();

        SetBlinkButton();
    }
    public void SetBlinkButton()
    {
        if(player.IsLocal)
        {
            if(player.IsMasterClient)
            {
                if(roomManager.readyList.Count >= PhotonNetwork.CurrentRoom.MaxPlayers)
                {
                    if(isMasterValue) 
                    {
                        readyButton.image.color = Color.Lerp(Color.clear,new Color(0.95f,0.75f,0.45f,1),Mathf.PingPong(Time.time, 1));
                    }
                    else readyButton.image.color = Color.white;
                }
                else readyButton.image.color = Color.white;
            }
            else
            {
                if(!isReady) readyButton.image.color = Color.Lerp(Color.clear,new Color(0.95f,0.95f,0.95f,1),Mathf.PingPong(Time.time, 1));
                else readyButton.image.color = Color.green;
            }
        }
        else        
        {
            if(player.IsMasterClient)
            {
                readyButton.image.color = Color.white;
            }
            else
            {
                if(!isReady) readyButton.image.color = Color.white;
                else 
                {
                    readyButton.image.color = Color.green;
                }
            }
        }
        //Debug.LogError($"PhotonNetwork.AutomaticallySyncScene = {PhotonNetwork.AutomaticallySyncScene}");
    }
    public void SetActiveFalseButton()
    {
        readyButton.gameObject.SetActive(false);
    }
}
