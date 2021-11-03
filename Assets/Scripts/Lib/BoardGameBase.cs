using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Res_2D_BoardGame
{
    // 정사각형 : square
    // 직사각형 : Rectangle
    public class BoardGameBase : MonoBehaviour
    {
        int[,] board;
        int row, col;
        public int r { protected get; set; }
        public int c { protected get; set; }
        public int turn {get; private set; }
        protected bool isGameOver { get; private set; }
        public BoardGameBase()
        {
            turn = 2; // 나중에 매개변수로 people로 받아서 n인용 까지 만들어야겠다...
            isGameOver = false;
        }
        public void InitGame(int boardNum)
        {
            row = col = boardNum;
            board = new int[row, col];
        }

        protected int GetBoardValue(int r, int c) // board pos value값을 리턴하여 logic을 구성하게 돕는다
        {
            return board[r, c];
        }
        protected void SetBoardValue(int r,int c, int value)
        {
            board[r, c] = value;
        }
        protected int GetBoardLength()
        {
            return board.GetLength(0);
        }
        protected void IsGameOver()
        {
            isGameOver = true;
        }
        public void NextTurn() // 1인용인 경우 사용하지 않음 그만이라 가상함수로 정의하지 않음!
        {            
            turn = 3 - turn; // 2 > 1 > 2 > 1
        }
        public bool CheckOverValue(int r, int c) // row, col의 범위 값을 넘으면 false를 리턴함 .. length값이기 때문에 r >= row 하여야 함
        {
            if (r >= row || r < 0) return false;
            if (c >= col || c < 0) return false;
            return true;
        }
        public void DebugBoard()
        {
            string s = "";
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(0); j++) // 아 반대로 포스값도 받아야되는구나 ,,, 
                {
                    s += board[i, j].ToString() + " ";
                }
                s += "\n";
            }            
            Debug.Log(s);
        }
    }
    /*
        1. 내가 둔 stone위치값이 empty인걸 확인하여야 함
        2. top, bottom, left, right, top-left, top-right, bottom-left, bottom-right
        3. 내가 둔 stone위치에서 위의 pos값에서 omok인 경우 같은 값이 존재하여야 하며, 
        othello인 경우 처음엔 omok과 같지만 탐색 마지막 값이 다른 값이 존재 하여야만 함!!
    */
    public abstract class SequenceBoardGame : BoardGameBase
    {
        // top, bottom, left, right, top-left, top-right, bottom-left, bottom-right
        protected enum SequenceDir { T, B, L, R, TL, TR, BL, BR }        
        protected Queue<int> sequenceQ;
        protected int[,] checkDir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
        public SequenceBoardGame()
        {
            sequenceQ = new Queue<int>();
        }        
        public abstract void OnGameStart();
        public abstract void OnGameStop(); // 이 시점에서 isGameOver = false 와 turn의 값을 판단하여 누가 이겼는지 리턴        
        public bool IsEmpty()
        {
            if (GetBoardValue(r, c) != 0 || GetBoardValue(r,c) == turn) return false;
            else return true;
        }
        public void CheckDirection()
        {
            // top, bottom, left, right, top-left, top-right, bottom-left, bottom-right

            for (int i = 0; i < checkDir.GetLength(0); i++) // 두개가 있을 경우의 수를 생각 못했네...
            {
                if (!CheckOverValue(r, c) || !CheckOverValue(r + checkDir[i, 0], c + checkDir[i, 1])) continue; // out of index range check
                if (GetBoardValue(r + checkDir[i, 0], c + checkDir[i, 1]) == turn)
                    sequenceQ.Enqueue(i);
            }
        }
        public abstract bool AnalyzeBoard();
    }
    // chess, 장기, 마작 // one card, poker, 라스베가스 이거랑은 다른 분류인데 이건 card분류 
    public abstract class StrategyBoardGame : BoardGameBase 
    {
        protected enum MoveKind {none,same,move,enemy}
        public List<KeyValuePair<int,int>> list = new List<KeyValuePair<int, int>>(); // 체크한 돌 또는 오브젝트들이 갈 수 있는 방향
        public int[,] deathStone {get;}
        public StrategyBoardGame()
        {
            deathStone = new int[2,7]; // 임시
        }
        public void DebugList()
        {
            foreach(var i in list)
            {
                Debug.Log($"Key(row) = {i.Key}, Value(col) = {i.Value}");
            }
        }
        public void DebugDeathStone()
        {
            string s = "";
            for(int i = 0; i < deathStone.GetLength(0); i++)
            {
                if(i == 0) s += "White : ";
                else s+= "Black : ";
                for(int j = 1; j < deathStone.GetLength(1); j++)
                {
                    s+= $"{j} = {deathStone[i,j]}, ";
                }
                s+= "\n";
            }
            Debug.Log(s);
        }
        /*
        1. none = out of range 절대 안됨
        2. move = 이동가능 // Add 
        3. enemy = 이동가능 하지만 이 순간 반복문을 멈춰야 함 // Add
        4. same = 이동 불가능 반복문 멈춤 
        */        

        // 나중에 _turn 그냥 turn으로 변경
        public void Move(int prevR, int prevC, int _turn)
        {
            SetBoardValue(prevR,prevC,0); // 전의 값 초기화
            SetBoardValue(r,c,_turn); // 이동할 현재의 값 set
        }
        public void Attack(int stone)
        {
            if(IsPossibleMove(r,c,turn) == (int)MoveKind.enemy)
            {
                deathStone[turn - 1 ,stone]++;
            }
        }
        public int IsPossibleMove(int r, int c, int _turn)
        {
            if (!CheckOverValue(r,c)) return (int)MoveKind.none; // out of range

            if (GetBoardValue(r,c) == 0) return (int)MoveKind.move; // 0은 이동가능 적아님
            else if (GetBoardValue(r,c) != _turn) return (int) MoveKind.enemy; // 적과 조우함
            else return (int) MoveKind.same;
        }        
        public bool AnalyzeBoard() // 이 분석이 각 stone마다 달라가지고..
        {
            foreach(var _list in list)
            {
                if(_list.Key == r && _list.Value == c) return true;
            }
            return false;
        }
        public void ResetList()
        {
            list.Clear();
        }
        public abstract void OnGameStart(); // 이건 내 생각엔 virtual로 만들어도 될 것 같은뎅
        public virtual void OnGameStop() // 이 시점에서 isGameOver = false 와 turn의 값을 판단하여 누가 이겼는지 리턴        
        {
            Debug.Log($"First turn = {turn}");
            int enemy = (3 - turn) - 1; // 나중에 n인용 게임을 만들게 되면 3을 매개변수 또는 고정 값으로 만들어 변경,, deathstone배열도 변경
            if(deathStone[enemy,1] > 0)
            {
                IsGameOver();
                enemy += 1;
                Debug.Log($"GameStop turn = {turn}");
                Debug.Log($"{(enemy == 1 ? "black" : "white")} is victory!!"); // 임시
            }
        }
    }

    public class Stone : MonoBehaviour
    {
        public enum StoneType { none, white, black }
        public Sprite[] _sprite;
        public SpriteRenderer _renderer;
        public int m_row { get; set; }
        public int m_col { get; set; }
        public int turn { get; set; }
        public int count { get; set; }
        public virtual void SetImageStone()
        {
            // 1. white, 2. black
            _renderer.sprite = _sprite[turn - 1];
        }

    }    
    public abstract class ChessStone : Stone
    {
        public int m_num; // 고유넘버
        public GameObject dotObject;
        public bool isCheck {get; private set;}
        public ChessStone()
        {
            isCheck = false;
        }
        public virtual void IsCheck()
        {
            isCheck = !isCheck;
        }
        public abstract void CheckMove();
    }
    
}
