using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class ReadyData : MonoBehaviour
{
    public Photon.Realtime.Player player;
    public bool isReady;

    [SerializeField]
    Text playerNameText;
    [SerializeField]
    Button readyButton;

    private void Awake() {
        isReady = false;
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

        var manager = FindObjectOfType<RoomManager>();
        bool value = true;

        foreach(var ready in manager.readyList)
        {
            if(!ready.player.IsMasterClient) 
            {
                if(!ready.isReady)
                {
                    value = ready.isReady;
                    break;
                }        
            }
        }
        if(player.IsMasterClient && value && manager.readyList.Count >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            manager.GameStart();
        }
        else
        {
            if(player.IsMasterClient) return;
        }
        isReady = !isReady;
        manager.ReadyPlayer(player.NickName,isReady);
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

            if(player.IsLocal)
                readyButton.image.color = Color.Lerp(Color.clear,new Color(0.75f,0.75f,0.75f,1),Mathf.PingPong(Time.time, 1));

            if(isReady)
                readyButton.image.color = Color.green;
            yield return null;
        }
    }
}
