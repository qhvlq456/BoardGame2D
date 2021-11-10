using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SettingsUI : MonoBehaviourPunCallbacks
{
    [Header("GameKind Button")]
    [SerializeField]
    Button[] buttons;

    [Header("InputField")]
    [SerializeField]
    InputField roomNameInput;

    [Header("Room option")]
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
    public void SettingsClose()
    {
        StartCoroutine(SettingsCloseDelay());
    }
    IEnumerator SettingsCloseDelay()
    {
        Animator anim = GetComponent<Animator>();

        anim.SetTrigger("close");

        yield return new WaitForSeconds(0.5f); // close 시간 보장이구나
        gameObject.SetActive(false); 
        anim.ResetTrigger("close");
    }
    #endregion

    #region Settings option create Room
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInput.text)) return;
        else if(type <= -1) return;

        PhotonNetwork.CreateRoom(roomNameInput.text,new RoomOptions{MaxPlayers = maxPlayer}, null);
    }
    public override void OnCreatedRoom()
    {
        if(PhotonNetwork.IsMasterClient) // master client에서만 load scene 사용가능
        {
            Debug.LogError($"Load Scene   gamekind = {type + 1}");
            PhotonNetwork.LoadLevel(type + 1);
        }   
    }
    #endregion
}
