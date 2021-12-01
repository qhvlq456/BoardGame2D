using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChattingManager : MonoBehaviour
{
    PhotonView pv;
    [SerializeField]
    GameObject chatting;
    [SerializeField]
    GameObject chattingText;
    [SerializeField]
    InputField input;
    [SerializeField]    
    Button enterButton;
    [SerializeField]
    Button chattingButton;
    Queue<string> contentsQueue = new Queue<string>();
    private void Awake() {
        pv = GetComponent<PhotonView>();
    }
    public void CreateChattingText()
    {
        if(string.IsNullOrEmpty(input.text)) return;
        pv.RPC("RcpEnqueue",RpcTarget.AllViaServer,string.Format("{0} : {1}",PhotonNetwork.NickName,input.text));

        input.text = "";
    }
    [PunRPC]
    void RcpEnqueue(string msg)
    {
        contentsQueue.Enqueue(msg); // 아 이두개를 동기화 시켰어야 됬네
        StartCoroutine(CheckMessage());
    }
    void RpcCreateChattingText(string msg)
    {        
        Text chatting = Instantiate(chattingText,GameObject.Find("ChattingObject").transform).GetComponent<Text>();
        
        chatting.text = msg;
    }
    IEnumerator CheckMessage()
    {
        if(contentsQueue.Count <= 0) 
        {
            yield break;
        }        

        while(chattingButton.gameObject.activeSelf)
        {
            chattingButton.image.color = Color.Lerp(Color.white,Color.green,Mathf.PingPong(Time.time * 2,1));
            yield return null;
        }

        while(contentsQueue.Count > 0)
        {
            string content = contentsQueue.Dequeue(); // 난 비어져 있는데 상대방은 안비어져잇는 경우를 생각 못했구나 ㅋ
            RpcCreateChattingText(content);
            yield return null;
        }

        chattingButton.image.color = Color.white;
    }
    public void OnClickEnterButton()
    {
        CreateChattingText();
    }

    public void OnClickCloseChatting()
    {
        StartCoroutine(WaitForCloseDelay());
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
