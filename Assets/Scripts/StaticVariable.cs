using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticVariable : MonoBehaviour
{
    public enum Gamekind{omok,othello,chess}
    public static Gamekind gameKind;
    // omok
    public static int omokBoardNum= 17;
    public static int sequneceNum = 0;
    // common
    public static float startPos;
    public static float lastPos;
    public static float interval;
    // othello
    public static int othelloBoardNum = 8;
    // chess
    public static int chessBoardNum = 8;
    // practice    
    public static int practiceSceneLoadNum = 0;

    // 나중에 이것도 바꿔야겠다 모든 pos값 하나의 함수로 다 되게
    // omok
    public static int OmokGetStoneRowPosition(Vector2 _mousePos, ref Vector2 _vector)
    {
        float margin = 0.25f; // 순수 계산한거 수치스럽다
        float lastPos = 4f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.5f;

        float currentPos = lastPos;
        
        for (int i = 0; i < omokBoardNum; i++) // 아니 이것만 왜 되는거지?
        {
            if (currentPos - margin <= _mousePos.y && _mousePos.y <= currentPos + margin)
            {
                _vector.y = currentPos;
                return i;
            }
            currentPos -= interval;
        }
        return -1;
    }
    public static int OmokGetStoneColPosition(Vector2 _mousePos, ref Vector2 _vector)
    {
        float margin = 0.25f; // 순수 계산한거 수치스럽다
        float startPos = -4f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.5f;

        float currentPos = startPos;

        for (int i = 0; i < omokBoardNum; i++)
        {
            if (currentPos - margin <= _mousePos.x && _mousePos.x <= currentPos + margin)
            {
                _vector.x = currentPos;
                return i;
            }
            currentPos += interval;
        }
        return -1;
    }   
    
    // chess
    public static void InitYMidPos(float[] yMidpos)
    {        
        float lastPos = 3.6f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.9f;

        float currentPos = lastPos;

        for (int i = 0; i < chessBoardNum; i++) // 아니 이것만 왜 되는거지?
        {            
            yMidpos[i] = currentPos - (interval / 2);
            currentPos -= interval;
        }        
    }
    public static void InitXMidPos(float[] xMidpos)
    {        
        float startPos = -3.6f; // 앞 뒤 제외하고 [0].x2 ~ [18].x1 까지 가야함        
        float interval = 0.9f;

        float currentPos = startPos;

        for (int i = 0; i < chessBoardNum; i++)
        {
            xMidpos[i] = currentPos + (interval/2);
            currentPos += interval;
        }
    }  
    public static int GetStoneRowPosition(Vector2 _mousePos)
    {
        float currentPos = lastPos;

        for (int i = 0; i < chessBoardNum; i++) // 아니 이것만 왜 되는거지?
        {
            if (currentPos > _mousePos.y && _mousePos.y >= currentPos - interval)
            {
                return i;
            }
            currentPos -= interval;
        }
        return -1;
    }
    public static int GetStoneColPosition(Vector2 _mousePos)
    {
        float currentPos = startPos;

        for (int i = 0; i < chessBoardNum; i++)
        {
            if (currentPos <= _mousePos.x && _mousePos.x < currentPos + interval)
            {
                return i;
            }
            currentPos += interval;
        }
        return -1;
    }
}
