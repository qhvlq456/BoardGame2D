using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class OthelloManager : SequenceBoardGame
{
    float[] xMidpos = new float[StaticVariable.othelloBoardNum];
    float[] yMidpos = new float[StaticVariable.othelloBoardNum];
    float lastPos = 3.7f;
    float startPos = -3.7f;
    float interval = 0.925f;
    int r,c;
    GameObject _stone, _result;    
    Transform _transform;    
    List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();    

    void Start()
    {
        OnGameStart();
    }
    
    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            r = StaticVariable.GetStoneRowPosition(mousePos);
            c = StaticVariable.GetStoneColPosition(mousePos);
            if (r < 0 || c < 0) return;
            // r,c 입력했다고 가정
            if (!IsEmpty(r,c))
            {
                return;
            }
            else
            {
                CheckDirection(r,c,turn);
                if (!AnalyzeBoard(r,c,0)) return;
                else
                {
                    CreateStone(r,c);
                    OnChangeStone();
                    SetBoardValue(r, c, turn);
                    ResetList();
                }
            }

            if (!isGameOver) NextTurn();
        }
    }    
    void CreateStone(int row, int col)
    {
        GameObject stone = Instantiate(_stone, new Vector2(xMidpos[col],yMidpos[row]), Quaternion.identity);
        stone.GetComponent<ConcaveStone>().turn = turn;
        stone.GetComponent<ConcaveStone>().m_row = row;
        stone.GetComponent<ConcaveStone>().m_col = col;
        stone.GetComponent<ConcaveStone>().SetImageStone();
    }     
    public override bool AnalyzeBoard(int r, int c, int length)
    {
        // 내가 둔 돌에 사방에 같은 돌이 한개라도 존재한다면 들어감
        // 고로 오델로는 이 방향을 피하면 됨.. 총 8방향이 존재한다

        int[] diraction = new int[checkDir.GetLength(0)];
        List<KeyValuePair<int,int>> value = new List<KeyValuePair<int, int>>();

        while(sequenceQ.Count > 0)
        {
            diraction[sequenceQ.Dequeue()] = 1;
        }
        for(int i = 0; i < diraction.Length; i++)
        {
            if(diraction[i] != 1)
            {
                for (int sr = r + checkDir[i, 0], sc = c + checkDir[i, 1];
                CheckOverValue(sr, sc);
                sr += checkDir[i, 0], sc += checkDir[i, 1])
                {
                    if (GetBoardValue (sr, sc) == 3 - turn) // 1. 나의 돌과 다른 경우
                    {
                        value.Add(new KeyValuePair<int, int>(sr, sc));
                    }
                    else if(GetBoardValue(sr,sc) == turn) // 나의 돌과 같은 경우 .. 연속성이 깨졌다
                    {
                        CheckDirectionPosition(value);
                    }
                    else // 돌이 없는 경우
                    {
                        value.Clear();
                        break;
                    }
                }
            }
        }
        if (list.Count <= 0) return false;
        else return true;
    }
    void CheckDirectionPosition(List<KeyValuePair<int,int>> _value)
    {
        list.AddRange(_value);
        _value.Clear();
    }
    public override void OnGameStart()
    {
        StaticVariable.startPos = startPos;
        StaticVariable.lastPos = lastPos;
        StaticVariable.interval = interval;
        StaticVariable.InitXMidPos(xMidpos);
        StaticVariable.InitYMidPos(yMidpos);

        _stone = Resources.Load("Stone") as GameObject;
        _result = Resources.Load("Result Panel") as GameObject;
        _transform = GameObject.Find("Canvas").transform;
        InitBoard(StaticVariable.othelloBoardNum);        
        InitStone();
    }

    public override void OnGameStop()
    {
        GameOver();
        Instantiate(_result, _transform);
    }
    void InitStone()
    {
        int[,] initStone = new int[,] { { 3, 3 }, { 3, 4 }, { 4, 4 }, { 4, 3 } }; // row & col -> mousePos // 흑, 백, 흑, 백
        float xPos, yPos;
        xPos = startPos; yPos = lastPos;
        Vector2 _vector;

        for (int i = 0; i < 4; i++)
        {
            float x, y;
            y = yPos - initStone[i, 0] * interval - (interval / 2);
            x = xPos + initStone[i, 1] * interval + (interval / 2);
            _vector = new Vector2(x, y);
            SetBoardValue(initStone[i, 0], initStone[i, 1], turn);
            CreateStone(initStone[i, 0], initStone[i, 1]);
            NextTurn();
        }
    }
    public void OnChangeStone()
    {
        if (list.Count <= 0) return;
        /*
         * internal(내부의) <> external(외부의)
         * 1. board change internal(내부의)
         * 2. stone change external(외부의)
         */

        ConcaveStone[] enemy_Stone = FindObjectsOfType<ConcaveStone>(); // 이거 list로 addstone으로 변경하자
                
        foreach (var _list in list) // 1.이제 내부의 list걸린 모든 배열 값을 변경해 준다 
        {
            SetBoardValue(_list.Key, _list.Value, turn);
        }
        for (int i = 0; i < enemy_Stone.Length; i++) // 2. 외부의 오브젝트를 찾아서 색상 밑 턴의 값을 변경해 준다
        {
            int row = enemy_Stone[i].m_row;
            int col = enemy_Stone[i].m_col;
            
            if (GetBoardValue(row, col) == turn)
            {
                enemy_Stone[i].turn = turn;
                enemy_Stone[i].SetImageStone();
            }
        }        
    }
    public void ResetList()
    {
        list.Clear();        
    }

    public override bool AnalyzeBoard(int r, int c, int turn, int length)
    {
        throw new NotImplementedException();
    }
}
