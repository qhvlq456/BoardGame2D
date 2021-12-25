using Res_2D_BoardGame;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using Photon.Pun;
using System.Collections;

public class GameRoomManager : RoomManager
{
    string[] playerPath = new string[]{
        "Network/OmokPlayer",
        "Network/OthelloPlayer"
    };
    BoardGameBase GameManager;

    public override void Awake() {
        base.Awake();
        SetManager();
        PlayerEnter();
    }    
    private void Start() {
        SpawnGamePlayer();
    }
    public override void SetManager()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<BoardGameBase>();
    }
    public void GameOver() 
    {
        PhotonNetwork.AutomaticallySyncScene = false; // client의 Scene의 자유를 준다
        SceneManager.LoadScene("Room");
    } // 아 player sequence를 맞춰야되 슈바
    void SpawnGamePlayer()
    {
        int spawnIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["gameKind"];
        PhotonNetwork.Instantiate(playerPath[spawnIndex],new Vector3(0,0,0),Quaternion.identity);
    }
    // [PunRPC]
    // public void RpcPeopleCount()
    // {
    //     ++StaticVariable.twoPeople;
    // }
    // IEnumerator RoomSceneLoad()
    // {
    //     AsyncOperation op =  SceneManager.LoadSceneAsync("Room");
    //     op.allowSceneActivation = false;

    //     float timer = 0f;
            
    //     while (!op.isDone)
    //     {
    //         yield return null;
    //         timer += Time.deltaTime;
    //         if(timer >= 0.9f)
    //         {
    //             op.allowSceneActivation = true;
    //             yield break;
    //         }
    //     }
    // }
}
