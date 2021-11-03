using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class SpawnChessStone : MonoBehaviour
{
    ChessManager chessManager;
    GameObject pawn,knight,bishop,rook,queen,king;
    public Vector2 _vector {private get; set;}
    void Start()
    {
        SetSpawn();
        InitStone();
    }        
    void SetSpawn()
    {
        chessManager = GetComponent<ChessManager>();

        pawn = Resources.Load("Chess/Pawn") as GameObject;
        knight = Resources.Load("Chess/Knight") as GameObject;
        bishop = Resources.Load("Chess/Bishop") as GameObject;
        rook = Resources.Load("Chess/Rook") as GameObject;
        queen = Resources.Load("Chess/Queen") as GameObject;
        king = Resources.Load("Chess/King") as GameObject;
    }
    public GameObject CreateStone(int type)
    {
        GameObject _stone = null;
        switch(type)
        {
            case 1: _stone = Instantiate(king, _vector, Quaternion.identity);
            break;
            case 2: _stone = Instantiate(queen, _vector, Quaternion.identity);
            break;
            case 3: _stone = Instantiate(rook, _vector, Quaternion.identity);
            break;
            case 4: _stone = Instantiate(bishop, _vector, Quaternion.identity);
            break;
            case 5: _stone = Instantiate(knight, _vector, Quaternion.identity);
            break;
            case 6: _stone = Instantiate(pawn, _vector, Quaternion.identity);
            break;
        }
        return _stone;
    }
    public void InitCreateStone(GameObject _stone, params int[] stoneInfo)
    {        
        ChessStone chessStone = _stone.GetComponent<ChessStone>();
        chessStone.m_row = stoneInfo[0];
        chessStone.m_col = stoneInfo[1];
        chessStone.turn = stoneInfo[2];
        chessStone.m_num = stoneInfo[3];
        chessStone.SetImageStone();
    }
    void InitStone()
    {        
        int[,] initStone = new int[,] 
        {
            { 3,  5 , 4 , 1 , 2 , 4 , 5 , 3 },
            { 6 , 6 , 6 , 6 , 6 , 6 , 6 , 6 },
            { 6 , 6 , 6 , 6 , 6 , 6 , 6 , 6 },
            { 3 , 5 , 4 , 1 , 2 , 4 , 5 , 3 }
        };

        float xPos, yPos, interval;
        xPos = -3.6f; yPos = 3.6f; interval = 0.9f;
        int difference = 4;
        int _difference = 0;
        int _turn = chessManager.turn;        
        // black stone start pos = r : 0, c = 0 ~ r : 1, c = 7
        // white stone start pos = r : 6, c = 0 ~ r : 7, c = 7
        // row difference = 4
        for (int i = 0; i < initStone.GetLength(0); i++)
        {
            float x, y;            
            if(i == initStone.GetLength(0)/2) // 솔직히 이것도 맘에 안드넹ㅋ
            {
                _difference = difference;
                _turn = 3 - _turn;
            }            
            for(int j = 0; j < initStone.GetLength(1); j++)
            {
                y = yPos - (i + _difference) * interval - (interval / 2);
                x = xPos + j * interval + (interval / 2);
                _vector = new Vector2(x, y);                
                InitCreateStone(CreateStone(initStone[i,j]), (i + _difference) ,j ,_turn,initStone[i,j]);
            }
        }
        
    }        
}
