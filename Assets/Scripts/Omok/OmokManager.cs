using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class OmokManager : SequenceBoardGame
{
    GameObject _result;
    Transform _transform;
    int m_Length = 4;

    void Awake()
    {
        OnGameStart();
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
        _result = Resources.Load("Result Panel") as GameObject;
        _transform = GameObject.Find("Canvas").transform;
        InitBoard(StaticVariable.omokBoardNum);
    }
    public override void OnGameStop()
    {
        Instantiate(_result, _transform);
    }
    public void ResetLength()
    {
        m_Length = 4;
    }
}
