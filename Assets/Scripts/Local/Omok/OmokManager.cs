using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class OmokManager : SequenceBoardGame
{
    GameObject _result;
    Transform _transform;
    int m_Length = 0;

    void Awake()
    {
        OnGameStart();
    }
    public override bool AnalyzeBoard(int r, int c ,int length)
    {
        if (sequenceQ.Count <= 0) return false;
        
        while (sequenceQ.Count > 0)
        {
            int dir = sequenceQ.Dequeue();
            int _sr,_sc;
            _sr = _sc = 0;

            for (int sr = r, sc = c;
            CheckOverValue(sr, sc);
            sr += checkDir[dir, 0], sc += checkDir[dir, 1])
            {
                _sr = sr; _sc = sc;
                if(GetBoardValue(sr,sc) != turn)
                {
                    break;
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작

            // 왔던길 되돌아 가기
            int changeR,changeC;
            CheckChangeDirection(dir,out changeR,out changeC);

            if(changeR == 0 && changeC == 0) return false;

            for(int row = _sr, col = _sc; CheckOverValue(row, col); row += changeR, col += changeC)
            {
                if(GetBoardValue(row,col) != turn) 
                {
                    if(m_Length == length) break;
                    ResetLength();
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작
        
        }
        if (m_Length >= length) return true;
        else return false;
    }
    public override bool AnalyzeBoard(int r, int c ,int _turn, int length)
    {
        if (sequenceQ.Count <= 0) return false;
        
        while (sequenceQ.Count > 0)
        {
            int dir = sequenceQ.Dequeue();
            int _sr,_sc;
            _sr = _sc = 0;

            for (int sr = r, sc = c;
            CheckOverValue(sr, sc);
            sr += checkDir[dir, 0], sc += checkDir[dir, 1])
            {
                _sr = sr; _sc = sc;
                if(GetBoardValue(sr,sc) != _turn)
                {
                    break;
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작

            // 왔던길 되돌아 가기
            int changeR,changeC;
            CheckChangeDirection(dir,out changeR,out changeC);

            if(changeR == 0 && changeC == 0) return false;

            for(int row = _sr, col = _sc; CheckOverValue(row, col); row += changeR, col += changeC)
            {
                if(GetBoardValue(row,col) != _turn) 
                {
                    if(m_Length == length) break;
                    ResetLength();
                }
                else // 나와 같은 위치에 있는 돌
                {
                    m_Length++;
                }
            }
            if (m_Length >= length) break; // 오목!!
            else ResetLength(); // 재시작
        
        }
        if (m_Length >= length) return true;
        else return false;
    }
    public void CheckChangeDirection(int dir, out int changeR, out int changeC)
    {
        switch(dir)
        {
            case (int)SequenceDir.T : 
            changeR = checkDir[(int)SequenceDir.B,0];
            changeC = checkDir[(int)SequenceDir.B,1];
                break;
            case (int)SequenceDir.B : 
            changeR = checkDir[(int)SequenceDir.T,0];
            changeC = checkDir[(int)SequenceDir.T,1];
            break;
            case (int)SequenceDir.L : 
            changeR = checkDir[(int)SequenceDir.R,0];
            changeC = checkDir[(int)SequenceDir.R,1];
            break;
            case (int)SequenceDir.R : 
            changeR = checkDir[(int)SequenceDir.L,0];
            changeC = checkDir[(int)SequenceDir.L,1];
            break;
            case (int)SequenceDir.TL : 
            changeR = checkDir[(int)SequenceDir.BR,0];
            changeC = checkDir[(int)SequenceDir.BR,1];
            break;
            case (int)SequenceDir.TR : 
            changeR = checkDir[(int)SequenceDir.BL,0];
            changeC = checkDir[(int)SequenceDir.BL,1];
            break;
            case (int)SequenceDir.BL : 
            changeR = checkDir[(int)SequenceDir.TR,0];
            changeC = checkDir[(int)SequenceDir.TR,1];
            break;
            case (int)SequenceDir.BR : 
            changeR = checkDir[(int)SequenceDir.TL,0];
            changeC = checkDir[(int)SequenceDir.TL,1];
            break;
            default : 
            changeR = 0;
            changeC = 0;
            break;
        }
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
        m_Length = 0;
    }
}
