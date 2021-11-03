using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Res_2D_BoardGame;

public class Result : MonoBehaviour
{
    SequenceBoardGame GameManager;
    Text titleText, bodyText;
    Button retryBtn, mainBtn;
    void Start()
    {
        SetInit();
        SetButtonEvent();
        SetText();
    }        
    void SetInit()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<SequenceBoardGame>();
        titleText = GameObject.Find("Title Text").GetComponent<Text>();
        bodyText = GameObject.Find("Body Text").GetComponent<Text>();
        // find button
        retryBtn = GameObject.Find("Retry Button").GetComponent<Button>();
        mainBtn = GameObject.Find("Main Button").GetComponent<Button>();
    }
    void SetButtonEvent()
    {
        retryBtn.onClick.AddListener(OnClickRetry);
        mainBtn.onClick.AddListener(OnClickMain);
    }
    void SetText()
    {
        titleText.text = "Result";
        bodyText.text = $"Victory {(GameManager.turn == 1 ? "White" : "Black")}";
    }
    void OnClickRetry()
    {
        Debug.Log("Retry");
    }
    void OnClickMain()
    {
        Debug.Log("Main");
    }
}
