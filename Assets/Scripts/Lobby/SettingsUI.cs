using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SettingsUI : MonoBehaviourPunCallbacks
{
    string playerPath;
    [Header("GameKind Button")]
    [SerializeField]
    Button[] buttons;

    [Header("InputField")]
    [SerializeField]
    InputField roomNameInput;
    
    int type = -1;
    public byte maxPlayer = 2;
    public override void OnEnable() {
        base.OnEnable();
        SetButtonColor();
    }
    public void SetGameKind(int _type)
    {
        type = _type;
        SetButtonColor();
    }
    void SetButtonColor()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            if(type == i) buttons[type].GetComponent<Image>().color = Color.green;
            else buttons[i].GetComponent<Image>().color = Color.white;
        }
    }

    #region Settings open n close
    public void OnClickSettingsClose(bool value)
    {
        StartCoroutine(SettingsCloseDelay(value));
    }
    IEnumerator SettingsCloseDelay(bool value)
    {
        Animator anim = GetComponent<Animator>();

        anim.SetTrigger("close");
        
        yield return new WaitForSeconds(0.5f); // close 시간 보장이구나
        gameObject.SetActive(value);
        anim.ResetTrigger("close");
    }
    #endregion

    #region Settings option create Room
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInput.text)) return;
        else if(type <= -1) return;        
        string gameKind = type == 0 ? "Omok" : type == 1 ? "Othello" : type == 2 ? "Chess" : "";
       
        PhotonNetwork.CreateRoom(roomNameInput.text,
        new RoomOptions 
        {
            MaxPlayers = maxPlayer,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() {{"k",gameKind}},
            CustomRoomPropertiesForLobby = new string[1] {"k"} // 엄청난 삽질이다 로비에 룸 커스텀 받으려면 스트링으로 키값 셋팅해놔야함
        }
        , null);
        
    }
    public override void OnCreatedRoom()
    {
        if(PhotonNetwork.IsMasterClient) // master client에서만 load scene 사용가능
        {
            Debug.LogError($"Load Scene   gamekind = {type + 1}");
            //PhotonNetwork.LoadLevel(type + 1);
            PhotonNetwork.LoadLevel("Room");
        }
    }
    #endregion
}
