﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class OthelloPlayer : BasePlayer
{
    protected LocalOthelloManager GameManager;
    protected float[] xPosition = new float[StaticVariable.othelloBoardNum];
    protected float[] yPosition = new float[StaticVariable.othelloBoardNum];
    public virtual void Awake()
    {
        boardNum = StaticVariable.othelloBoardNum;
        SetStonePosition();
        SetPlayer();
        InitStone();
        GameManager.ChangeStoneNumber();
    }    
    public virtual void SetPlayer()
    {
        r = c = -1;
        m_turn = 1;
        GameManager = GameObject.Find("GameManager").GetComponent<LocalOthelloManager>();
        parent = GameObject.Find("Canvas").transform;
        playerType = (EPlayerType)GameManager.playerStoneNum;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isGameOver) return;      
        if(GameManager.turn != m_turn) return;

        if (Input.GetMouseButtonDown(0))
        {
            IsMouseButtonDown();

            if (r < 0 || c < 0) return;
            // r,c 입력했다고 가정
            if (!GameManager.IsEmpty(r,c))
            {
                GameObject alert = Instantiate(alertUI,parent);
                alert.GetComponent<AlertUI>().alert = EAlertKind.Fail;
                alert.GetComponent<AlertUI>().StartAnimation();
                return;
            }
            else
            {
                // (int)EPlayerType.white + 1
                if (!GameManager.CheckTransferTurn(GameManager.GetTurn())) // input밖으로 꺼내야됨ㅋ
                {
                    GameObject alert = Instantiate(alertUI,parent);
                    alert.GetComponent<AlertUI>().alert = EAlertKind.NextTurn;
                    alert.GetComponent<AlertUI>().StartAnimation();
                    Debug.Log("Don't put Stone therefore change turn");
                }
                else
                {
                    GameManager.CheckDirection(r,c,m_turn);
                    if (!GameManager.AnalyzeBoard(r,c)) return;
                    else
                    {
                        SelectStone(CreateStone(),r,c);
                        GameManager.ChangeStone(playerType,m_turn);
                        GameManager.SetBoardValue(r, c, m_turn);
                        GameManager.ResetList();
                        GameManager.ChangeStoneNumber();
                    }
                }
                
            }
            if(GameManager.ConditionGameOver())
                GameManager.GameOver();
            else GameManager.NextTurn();
        }
    }    
    public virtual void SelectStone(GameObject stone,int row, int col)
    {
        ConcaveStone _stone = stone.GetComponent<ConcaveStone>();
        _stone.stoneType = playerType;
        _stone.GetComponent<ConcaveStone>().m_turn = m_turn;
        _stone.GetComponent<ConcaveStone>().m_row = row;
        _stone.GetComponent<ConcaveStone>().m_col = col;
    }  
    public virtual void InitStone()
    {
        int[,] initStone = new int[,] { { 3, 4 }, { 4, 3 } }; // row & col -> mousePos // 흑, 백, 흑, 백

        for (int i = 0; i < 2; i++)
        {
            putPosition = new Vector2(xPosition[initStone[i,1]],yPosition[initStone[i,0]]);
            SelectStone(CreateStone(),initStone[i,0],initStone[i,1]);
            GameManager.SetBoardValue(initStone[i, 0], initStone[i, 1], m_turn);
        }
    }
    public override void GetStonePosition(Vector3 mousePosition)
    {
        float lastPos = 3.7f;
        float startPos = -3.7f;
        float interval = 0.925f;
        float currentPos = lastPos; // row

        for (int i = 0; i < boardNum; i++) // 아니 이것만 왜 되는거지?
        {
            if (currentPos > mousePosition.y && mousePosition.y >= currentPos - interval)
            {
                r = i;
                putPosition.y = yPosition[r];
            }
            currentPos -= interval;
        }
        r = r <= -1 ? -1 : r;

        currentPos = startPos; // col

        for (int i = 0; i < boardNum; i++)
        {
            if (currentPos <= mousePosition.x && mousePosition.x < currentPos + interval)
            {
                c = i;
                putPosition.x = xPosition[c];
            }
            currentPos += interval;
        }
        c = c <= -1 ? -1 : c;
    }

    public override void SetStonePosition()
    {
        // ypos 
        float lastPos = 3.6f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.9f;

        float currentPos = lastPos;

        for (int i = 0; i < boardNum; i++) // 아니 이것만 왜 되는거지?
        {            
            yPosition[i] = currentPos - (interval / 2);
            currentPos -= interval;
        } 
        // xpos
        float startPos = -3.6f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함

        currentPos = startPos;

        for (int i = 0; i < boardNum; i++)
        {
            xPosition[i] = currentPos + (interval/2);
            currentPos += interval;
        }
    }
}
