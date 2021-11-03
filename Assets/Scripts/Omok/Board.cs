using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    GameObject _line;
    GameObject line;
    Vector2 startPos;
    float x = -4f;
    float y = 4f; 
    float interval = 0.5f;

    void Start()
    {
        SetInit();
        SetLinePosition();
    }
    
    void SetInit()
    {
        line = Resources.Load("Line") as GameObject;
        startPos = new Vector2(x, y);

    }
    void SetLinePosition()
    {
        for(int i = 0; i < StaticVariable.omokBoardNum; i++)
        {
            CreateLine();            
            _line.GetComponent<LineRenderer>().SetPosition(0, startPos);
            _line.GetComponent<LineRenderer>().SetPosition(1, new Vector2(-startPos.x,startPos.y));
            startPos.y -= interval;
        }
        startPos.y = y;
        for(int i = 0; i < StaticVariable.omokBoardNum; i++)
        {
            CreateLine();            
            _line.GetComponent<LineRenderer>().SetPosition(0, startPos);
            _line.GetComponent<LineRenderer>().SetPosition(1, new Vector2(startPos.x, -startPos.y));
            startPos.x += interval;
        }
    }
    void CreateLine()
    {
        _line = Instantiate(line, startPos,Quaternion.identity);
    }
}
