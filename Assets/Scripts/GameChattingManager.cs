using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameChattingManager : ChattingManager
{
    [SerializeField]
    GameObject chatting;
    [SerializeField]
    Button enterButton;
    [SerializeField]
    Button chattingButton;
    public override void Awake() {
        base.Awake();
    }
    public void OnClickCloseChatting()
    {
        StartCoroutine(WaitForCloseDelay());
    }
    public override void OnClickEnterButton()
    {
        base.OnClickEnterButton();
        pv.RPC("SetOthersChatting",RpcTarget.Others);
    }
    [PunRPC]
    public void SetOthersChatting()
    {
        StartCoroutine(CheckMessage());
    }
    IEnumerator CheckMessage()
    {
        while(chattingButton.gameObject.activeSelf)
        {
            chattingButton.image.color = Color.Lerp(Color.white,Color.green,Mathf.PingPong(Time.time * 2,1));
            yield return null;
        }

        while(contentsObject.Count > 0)
        {
            yield return null;
        }

        chattingButton.image.color = Color.white;
    }
    IEnumerator WaitForCloseDelay()
    {
        Animator anim = chatting.GetComponent<Animator>();
        anim.SetTrigger("close");

        yield return new WaitForSeconds(0.5f);
        chatting.SetActive(false);
        anim.ResetTrigger("close");
    }  
}
