using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Res_2D_BoardGame;
public class ReadyData : MonoBehaviour
{
    // 이걸 재활용 가능하게 만들어야 됨
    public Photon.Realtime.Player player;
    protected RoomManager roomManager;
    public bool isReady;

    [SerializeField]
    Text playerNameText;

    public virtual void Awake() {
        roomManager = FindObjectOfType<RoomManager>();
    }
    private void Update() {
        SetPlayerText();
    }
    public virtual void SetPlayerText()
    {
        if(player.IsMasterClient)
        {
            playerNameText.text = string.Format("{0} ({1})",player.NickName,"master");
        }
        else 
           playerNameText.text = string.Format("{0}",player.NickName);
        
        if(player.IsLocal)
            playerNameText.color = Color.yellow;
    }
}
