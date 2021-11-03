using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class OmokManager : SequenceBoardGame
{
    GameObject _stone;
    GameObject _result;
    Transform _transform;
    Vector2 _vector;
    int m_Length = 4;

    void Start()
    {
        OnGameStart();
    }
    
    void Update()
    {
        if (isGameOver) return;

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos= Camera.main.ScreenToWorldPoint(Input.mousePosition);
            r = StaticVariable.OmokGetStoneColPosition(mousePos,ref _vector);
            c = StaticVariable.OmokGetStoneRowPosition(mousePos,ref _vector);

            if (r < 0 || c < 0) return;
            // r,c 입력했다고 가정
            if (!IsEmpty()) return;
            CreateStone();
            SetBoardValue(r, c, turn);
            CheckDirection();
            if (AnalyzeBoard()) OnGameStop();
            else
            {                
                ResetLength();
            }
            if (!isGameOver) NextTurn();            
        }
    }
      
    void CreateStone()
    {
        GameObject stone = Instantiate(_stone, _vector, Quaternion.identity);
        stone.GetComponent<ConcaveStone>().turn = turn;        
        stone.GetComponent<ConcaveStone>().m_col = c;
        stone.GetComponent<ConcaveStone>().m_row = r;
        stone.GetComponent<Stone>().SetImageStone();
    }
    public override bool AnalyzeBoard()
    {
        if (sequenceQ.Count <= 0) return false;

        while (sequenceQ.Count > 0)
        {
            int dir = sequenceQ.Dequeue();
            for (int sr = r + checkDir[dir, 0], sc = c + checkDir[dir, 1];
                CheckOverValue(sr, sc);
                sr += checkDir[dir, 0], sc += checkDir[dir, 1])
            {
                if (GetBoardValue(sr, sc) != turn) break;
                else m_Length--;
            }
            if (m_Length <= 0) break; // 오목!!
            else ResetLength(); // 재시작
        }

        if (m_Length <= 0) return true;
        else return false;
    }    
    public override void OnGameStart()
    {
        _stone = Resources.Load("Stone") as GameObject;
        _result = Resources.Load("Result Panel") as GameObject;
        _transform = GameObject.Find("Canvas").transform;
        InitGame(StaticVariable.omokBoardNum);
    }
    public override void OnGameStop()
    {
        IsGameOver();
        Instantiate(_result, _transform);
    }
    public void ResetLength()
    {
        m_Length = 4;
    }
}
