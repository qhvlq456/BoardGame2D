using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Res_2D_BoardGame;

public class OthelloManager : SequenceBoardGame
{
    [SerializeField]
    Text whiteNumber;
    [SerializeField]
    Text blackNumber;
    [SerializeField]
    GameObject numbering;

    float[] xMidpos = new float[StaticVariable.othelloBoardNum];
    float[] yMidpos = new float[StaticVariable.othelloBoardNum];
    float lastPos = 3.7f;
    float startPos = -3.7f;
    float interval = 0.925f;
    int r,c;
    GameObject _stone, _result;
    Transform _transform;    
    List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
    public List<ConcaveStone> whiteList = new List<ConcaveStone>(); // 임시 public 
    public List<ConcaveStone> blackList = new List<ConcaveStone>(); // 임시 public 

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
                // (int)EPlayerType.white + 1
                if (!CheckTransferTurn(GetTurn()))
                {
                    Debug.Log("Don't put Stone therefore change turn");
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
                        ChangeStoneNumber();
                    }
                }
                
            }
            if(!CheckTransferTurn(GetTurn()) && !CheckTransferTurn(GetNextTurn())) GameOver();
            else NextTurn();
        }
    }    
    void ChangeStoneNumber()
    {
        whiteNumber.text = whiteList.Count.ToString();
        blackNumber.text = blackList.Count.ToString();
    }
    void CreateStone(int row, int col)
    {
        ConcaveStone stone = Instantiate(_stone, new Vector2(xMidpos[col],yMidpos[row]), Quaternion.identity).GetComponent<ConcaveStone>();
        stone.playerType = turn == 1 ? EPlayerType.white : EPlayerType.black;
        stone.GetComponent<ConcaveStone>().turn = turn;
        stone.GetComponent<ConcaveStone>().m_row = row;
        stone.GetComponent<ConcaveStone>().m_col = col;
        stone.GetComponent<ConcaveStone>().SetImageStone();

        if(turn - 1 == (int)EPlayerType.white) whiteList.Add(stone);
        else blackList.Add(stone);
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
    public bool CheckTransferTurn(int _turn) // 이건 그냥 보조수단임 // 여기서 만들어야 하는구나~ㅋㅋㅋㅋㅋ 시발
    {
        // overvalue여선 안되며, 돌이 존재하는 위치에서 확인하면 안된다 그리고 row, col의 최대, 최솟값을 구하여 그 둘레부분을 탐색한다
        // 최소,최댓값을 구하기엔 너무 과분하여 그냥 전부 탐색으로 만들었다
        bool isTrue = false;
        for(int i = 0; i < StaticVariable.othelloBoardNum; i++)
        {
            for(int j = 0; j < StaticVariable.othelloBoardNum; j++)
            {
                if(IsEmpty(i,j))
                {
                    CheckDirection(i,j,_turn);
                    if(AnalyzeBoard(i,j,0)) 
                    {
                        ResetList();
                        isTrue = true;
                        break;
                    }
                }
            }
            if(isTrue) break;
        }
        return isTrue;
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
        CreateNumbering();
    }

    public override void OnGameStop()
    {
        string s = string.Format("Victory : {0}",whiteList.Count > blackList.Count ? "White" : whiteList.Count < blackList.Count ? "Black" : "Same");
        Debug.Log(s);
        //Instantiate(_result, _transform);
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

        List<ConcaveStone> enemyList = (turn -1 == (int)EPlayerType.white) ? blackList : whiteList; // 서로 반대로 해야 됬는데;;
        Queue<ConcaveStone> enemyQueue = new Queue<ConcaveStone>();

        foreach (var _list in list) // 1.이제 내부의 list걸린 모든 배열 값을 변경해 준다 
        {
            SetBoardValue(_list.Key, _list.Value, turn);
        }
        
        var enemys = enemyList.Where(e => GetBoardValue(e.m_row,e.m_col) == turn);

        foreach(var enemy in enemys)
        {
            enemy.turn = turn;
            enemy.playerType = turn == 1 ? EPlayerType.white : EPlayerType.black;
            enemy.SetImageType();
            enemyQueue.Enqueue(enemy);
        }
        while(enemyQueue.Count > 0)
        {
            ConcaveStone enemy = enemyQueue.Dequeue();

            if(enemyList == whiteList)
            {
                whiteList.Remove(enemy); // white가 바뀔 돌 들이니 whitelist에서 삭제해야 된다
                blackList.Add(enemy);
            }
            else
            {
                blackList.Remove(enemy);
                whiteList.Add(enemy);
            }
        }
    }
    public void CreateNumbering()
    {
        float interval = 0.85f;

        for(int i = 0; i < StaticVariable.othelloBoardNum; i++)
        {
            GameObject text = Instantiate(numbering,new Vector3(xMidpos[0] - interval,yMidpos[i],0),Quaternion.identity); 
            text.GetComponent<Canvas>().sortingLayerName = "Text";
            Text _text = text.transform.GetChild(0).GetComponent<Text>();
            _text.color = Color.white;
            _text.text = i.ToString();
        }
        for(int j = 0; j < StaticVariable.othelloBoardNum; j++)
        {
            GameObject text = Instantiate(numbering,new Vector3(xMidpos[j],yMidpos[0] + interval,0),Quaternion.identity);
            text.GetComponent<Canvas>().sortingLayerName = "Text";
            Text _text = text.transform.GetChild(0).GetComponent<Text>();
            _text.color = Color.white;
            _text.text = j.ToString();
        }
    }
    public void ResetList()
    {
        if(list.Count <= 0) return;
        list.Clear();        
    }

    public override bool AnalyzeBoard(int r, int c, int turn, int length)
    {
        throw new NotImplementedException();
    }
}
