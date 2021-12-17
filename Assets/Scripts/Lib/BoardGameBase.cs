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
        int arrNum;
        //public int r,c;
        public int turn; //{get; private set; }
        public bool isGameOver; //{get; private set; }
        public BoardGameBase()
        {
            isGameOver = false;
            turn = 1;
        }
        public void InitBoard(int boardNum)
        {
            arrNum = boardNum;
            board = new int[arrNum, arrNum];
        }
        public virtual void InitGame()
        {
            Array.Clear(board,0,board.Length);
        }
        // public void SetRowCol(int r, int c)
        // {
        //     this.r = r; this.c = c;
        // }
        public int GetBoardValue(int r, int c) // board pos value값을 리턴하여 logic의 구성하게 돕는다
        {
            return board[r, c];
        }
        public void SetBoardValue(int r,int c, int value)
        {
            board[r, c] = value;
        }
        protected int GetBoardLength()
        {
            return board.GetLength(0);
        }
        public void GameStart()
        {
            //isGameOver = false;
            OnGameStart();
        }
        public void GameOver()
        {
            isGameOver = true;
            OnGameStop();
        }
        public virtual void OnGameStart(){}
        public virtual void OnGameStop(){}        
        public void NextTurn() // 1인용인 경우 사용하지 않음 그만이라 가상함수로 정의하지 않음!
        {            
            turn = 3 - turn; // 2 > 1 > 2 > 1
        }
        public bool CheckOverValue(int r, int c) // row, col의 범위 값을 넘으면 false를 리턴함 .. length값이기 때문에 r >= row 하여야 함
        {
            if (r >= arrNum || r < 0) return false;
            if (c >= arrNum || c < 0) return false;
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
        // turn == 1 : white , 2 == black
        // top, bottom, left, right, top-left, top-right, bottom-left, bottom-right
        protected enum SequenceDir { T, B, L, R, TL, TR, BL, BR }
        protected Queue<int> sequenceQ;
        public int[,] checkDir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
        public SequenceBoardGame()
        {
            sequenceQ = new Queue<int>();
        }
        public bool IsEmpty(int r, int c)
        {
            if (GetBoardValue(r, c) != 0 || GetBoardValue(r,c) == turn) return false;
            else 
            {
                return true;
            }            
        }
        public void CheckDirection(int r, int c, int _turn)
        {
            // top, bottom, left, right, top-left, top-right, bottom-left, bottom-right

            for (int i = 0; i < checkDir.GetLength(0); i++) // 두개가 있을 경우의 수를 생각 못했네...
            {
                if (!CheckOverValue(r, c) || !CheckOverValue(r + checkDir[i, 0], c + checkDir[i, 1])) continue; // out of index range check
                if (GetBoardValue(r + checkDir[i, 0], c + checkDir[i, 1]) == _turn)
                    sequenceQ.Enqueue(i);
            }
        }
        public abstract bool AnalyzeBoard(int r, int c, int length);
        public abstract bool AnalyzeBoard(int r, int c,int turn, int length);
    }
    // chess, 장기, 마작 // one card, poker, 라스베가스 이거랑은 다른 분류인데 이건 card분류 
    public abstract class StrategyBoardGame : BoardGameBase 
    {
        protected enum MoveKind {none,same,move,enemy}
        public List<KeyValuePair<int,int>> moveList = new List<KeyValuePair<int, int>>(); // 체크한 돌 또는 오브젝트들이 갈 수 있는 방향
        public int[,] deathStone {get;}
        public StrategyBoardGame()
        {
            deathStone = new int[2,7]; // 임시
        }
        public void DebugList()
        {
            foreach(var i in moveList)
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
        public void Move(int prevR, int prevC, int r, int c)
        {
            SetBoardValue(prevR,prevC,0); // 전의 값 초기화
            SetBoardValue(r,c,turn); // 이동할 현재의 값 set
        }
        public void Attack(int r, int c, int stone)
        {
            // check enemy destroy stone
            if(IsPossibleMove(r,c,turn) == (int)MoveKind.enemy)
            {
                int enemyTurn = 3 - turn;
                deathStone[enemyTurn - 1 ,stone]++;
            }
        }
        public int IsPossibleMove(int r, int c, int _turn)
        {
            if (!CheckOverValue(r,c)) return (int)MoveKind.none; // out of range

            if (GetBoardValue(r,c) == 0) return (int)MoveKind.move; // 0은 이동가능 적아님
            else if (GetBoardValue(r,c) != _turn) return (int) MoveKind.enemy; // 적과 조우함
            else return (int) MoveKind.same;
        }        
        public bool AnalyzeBoard(int r , int c) // 이 분석이 각 stone마다 달라가지고..
        {
            foreach(var _list in moveList)
            {
                if(_list.Key == r && _list.Value == c) return true;
            }
            return false;
        }
        public void ResetList()
        {
            moveList.Clear();
        }
        public override void OnGameStop() // 이 시점에서 isGameOver = false 와 turn의 값을 판단하여 누가 이겼는지 리턴        
        {
            Debug.Log($"First turn = {turn}");
            int enemy = (3 - turn) - 1; // 나중에 n인용 게임을 만들게 되면 3을 매개변수 또는 고정 값으로 만들어 변경,, deathstone배열도 변경
            if(deathStone[enemy,1] > 0)
            {
                GameOver();
                enemy += 1;
                Debug.Log($"GameStop turn = {turn}");
                Debug.Log($"{(enemy == 1 ? "black" : "white")} is victory!!"); // 임시
            }
        }
    }

    public class Stone : MonoBehaviour
    {
        // turn == 1 : white , 2 == black
        public Sprite[] _sprite;
        public SpriteRenderer _renderer;
        public EPlayerType playerType;
        public bool isCheck {get; private set;}
        public int m_row;
        public int m_col;
        public int turn;
        public int count;
        public Stone()
        {
            isCheck = false;
        }
        public virtual void IsCheck()
        {
            isCheck = !isCheck;
        }
        public virtual void SetImageStone()
        {
            // 1. white, 2. black
            _renderer.sprite = _sprite[turn - 1]; // 아 이것부터 해결해야 되네..
        }
        public void SetImageType()
        {
            _renderer.sprite = _sprite[(int)playerType];
        }
        

    }
    // 그냥 여기에 정의를 다 해버렸네.. 재활용 안될것 같은데 .. 이걸 나눠야 될듯 StrategyStone -> sequenceStone, UnsequenceStone
    // 그다음 이걸 상속받고 뭐 해야 하냨ㅋㅋㅋ 이제 로직이 없는데 ㅋㅋㅋ 다른 효과들을 넣어야 되나 아 모르겠다 ~~ 
    public class ChessStone : Stone
    {
        protected enum MoveKind {none,same,move,enemy}
        public int m_num; // 고유넘버
        public bool isSequence;
        protected int dirIdx;
        public int[,] searchdirection;
        public delegate int possibleMove(int r, int c, int turn);

        public ChessStone()
        {
            dirIdx = 0;
        }
        public void SetDirection(char subChar, params string[] dirStr)
        {
            searchdirection = new int[dirStr.Length,2];
            foreach(var subStr in dirStr)
            {
                string[] subDir = subStr.Split(subChar);
                SearchDirectionArray(subDir);
            }
            dirIdx = 0;
        }
        void SearchDirectionArray(string[] str)
        {
            foreach(var _str in str)
            {
                SetTempDir(_str);
            }
            dirIdx++;
        }
        void SetTempDir(string str)
        {            
            switch(str)
            {
                case "top" : 
                searchdirection[dirIdx,0] += -1;
                searchdirection[dirIdx,1] += 0;
                    break;
                case "bottom" :
                searchdirection[dirIdx,0] += 1;
                searchdirection[dirIdx,1] += 0;
                    break;
                case "left" :
                searchdirection[dirIdx,0] += 0;
                searchdirection[dirIdx,1] += -1;
                    break;
                case "right" :
                searchdirection[dirIdx,0] += 0;
                searchdirection[dirIdx,1] += 1;
                    break;
                default : break;
            }
        }
        // 하 상수가 모호하네.. 통일을 하든가 리스트 복사하던가 생각해야됨
        // 이건 연속성 있는 함수야..
        public virtual void DefaultMove(possibleMove action, List<KeyValuePair<int,int>> possiblePos)
        {
            int sr = m_row; int sc = m_col;

            while(dirIdx < searchdirection.GetLength(0))
            {
                sr += searchdirection[dirIdx,0];
                sc += searchdirection[dirIdx,1];
                
                if(isSequence) SequenceLogic(ref sr, ref sc, action, possiblePos);
                else UnSequenceLogic(ref sr, ref sc, action, possiblePos);
            }        
            // init
            dirIdx = 0;
        }
        // 델리게이트 빈거 하나 만들어서 이 부분을 정의하여 새로운 스크립트에 박아둬야 되나;;
        void SequenceLogic(ref int sr, ref int sc, possibleMove action, List<KeyValuePair<int,int>> possiblePos)
        {
            if(action(sr,sc,turn) < (int)MoveKind.move)
            {
                sr = m_row; sc = m_col;
                dirIdx++;
            }
            else
            {
                possiblePos.Add(new KeyValuePair<int, int>(sr,sc));
                if(action(sr,sc,turn) != (int)MoveKind.move)
                {
                    sr = m_row; sc = m_col;
                    dirIdx++;
                }
            }
        }
        void UnSequenceLogic(ref int sr, ref int sc, possibleMove action, List<KeyValuePair<int,int>> possiblePos)
        {
            if(action(sr,sc,turn) >= (int)MoveKind.move)
            {
                possiblePos.Add(new KeyValuePair<int, int>(sr,sc));
            }

            sr = m_row; sc = m_col;
            dirIdx++;
        }
    }
    
}
