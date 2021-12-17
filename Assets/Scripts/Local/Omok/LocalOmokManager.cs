using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Res_2D_BoardGame;

public class LocalOmokManager : OmokManager
{
    [SerializeField]
    Button gameStartButton;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject enemy;
    GameObject gameMenu;
    public int playerStoneNum;
    public int enemyStoneNum;
    
    private void Awake() {
        OnGameStart();
    }
    public void OnClickGameStartButton()
    {

        gameMenu.SetActive(false);
        enemyStoneNum = playerStoneNum == 0 ? 1 : 0;

        Instantiate(player,new Vector3(0,0,0),Quaternion.identity);
        Instantiate(enemy,new Vector3(0,0,0),Quaternion.identity);
    }
    public void OnClickBackButton()
    {
        SceneManager.LoadScene("Lobby"); // 임시 lobby
    }
    public override void OnGameStart()
    {
        base.OnGameStart();
        gameMenu = GameObject.Find("GameMenu");
    }
    public override void OnGameStop()
    {
        Debug.Log("GameOver");
        //base.OnGameStop();
        // 여기선 result창 나옴 바로 다시 scene load하면된다
    }
}
