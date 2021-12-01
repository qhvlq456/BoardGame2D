using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Res_2D_BoardGame;
using Photon.Pun;


public class Result : MonoBehaviourPun
{
    SequenceBoardGame GameManager;
    RoomManager roomManager;
    Text titleText, bodyText;
    Button confirmButton;
    public float maxTime;
    void Awake()
    {
        SetResult();
        SetButtonEvent();
        SetText();
    }    
    private void Start() {
    }
    void SetResult()
    { 

        GameManager = GameObject.Find("GameManager").GetComponent<SequenceBoardGame>();
        titleText = GameObject.Find("Title Text").GetComponent<Text>();
        bodyText = GameObject.Find("Body Text").GetComponent<Text>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        // find button
        confirmButton = GameObject.Find("Confirm Button").GetComponent<Button>();
    }
    void SetButtonEvent()
    {
        confirmButton.onClick.AddListener(() => 
        {
            Debug.Log("OK");
            roomManager.SceneLoadValue();
            Destroy(gameObject);
        });
    }
    void SetText()
    {
        titleText.text = "Result";
        bodyText.text = $"Victory {(GameManager.turn == 1 ? "White" : "Black")}";
    }
    // 승패를 턴으로 알수 있다 raise event나 써볼가? 나쁘지 않은거 같은뎅
    
}

