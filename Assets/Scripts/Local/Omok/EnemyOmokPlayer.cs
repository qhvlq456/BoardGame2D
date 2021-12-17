using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyOmokPlayer : OmokPlayer
{
    public int direction;
    public int playType;
    // checkDir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
    // SequenceDir { T, B, L, R, TL, TR, BL, BR }
    //               0  1  2  3  4    5  6    7
    public List<KeyValuePair<string,string>> priorityList = new List<KeyValuePair<string, string>>()
    { // 1 = i
        new KeyValuePair<string, string>("arrNum","i"), // top,,0
        new KeyValuePair<string, string>("0","i"), // bottom,,1
        new KeyValuePair<string, string>("i","arrNum"), // left,,2
        new KeyValuePair<string, string>("i","0"), // right,,3
        new KeyValuePair<string, string>("arrNum","arrNum - i"), // top-left,,4 .. 4
        new KeyValuePair<string, string>("arrNum - i","arrNum"), // top-left,,5
        new KeyValuePair<string, string>("arrNum - i","0"), // top-right,,6 .. 5
        new KeyValuePair<string, string>("arrNum","arrNum - i"), // top-right,,7 
        new KeyValuePair<string, string>("0","i"), // bottom-left,,8..6
        new KeyValuePair<string, string>("arrNum - i","arrNum"), // bottom-left,,9
        new KeyValuePair<string, string>("0","arrNum - i"), // bottom-right,,10.. 7
        new KeyValuePair<string, string>("i","0"), // bottom-right,,11
    };

    public List<ConcaveStone> stones = new List<ConcaveStone>();
    public override void Awake()
    {
        base.Awake();
    }
    private void Start() {
        StartCoroutine(EnemyRoutine());
    }
    public override void SetPlayer()
    {
        base.SetPlayer();
        playerType = (EPlayerType)GameManager.enemyStoneNum;
        m_turn = 2;
        direction = -1;
    }
    private void Update() {
    }
    IEnumerator EnemyRoutine()
    {
        if(GameManager.isGameOver)
        {
            yield break;
        }

        while(GameManager.turn != m_turn)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        
        if(!PrioritySearchStone())
            EnemyStart();

        putPosition = new Vector3(xPostion[c],yPostion[r],0);

        SelectStone(CreateStone());

        GameManager.SetBoardValue(r,c,m_turn);
        GameManager.CheckDirection(r,c,m_turn);
        
        if(GameManager.AnalyzeBoard(r,c,5))
        {
            Debug.Log($"analyze r = {r}, c = {c}, turn = {m_turn}");
            GameManager.GameOver();
        }
        else
        {
            GameManager.ResetLength();
        }

        if(!GameManager.isGameOver)
        {
            GameManager.NextTurn();
            StartCoroutine(EnemyRoutine());
        }
    }

    public void EnemyStart()
    {
        playType = Random.Range(1,10); // max = 10;

        if(r <= -1 || c <= -1)
        {
            playType = 0;
        }

        CheckWarningType();
    }
    void CheckWarningType()
    {
        List<int> list = new List<int>();

        if(playType <= 0)
        {
            Debug.Log($"Enemy IsInit");
            InitPutStone();
        }
        else if(playType <= 4) // 50퍼
        {
            Debug.Log($"Enemy IsAttack");
            SelectHaviour(r,c,list);
        }
        else
        {
            Debug.Log($"Enemy IsDefend");
            var player = GameObject.Find("OmokPlayer(Clone)").GetComponent<OmokPlayer>();
            SelectHaviour(player.r,player.c,list,player);
        }
    }
    
    void InitPutStone() // player 방향중 하나 찾아서 두기
    {
        // init direction
        direction = Random.Range(0,GameManager.checkDir.GetLength(0));
        // player nearly stone
        var stone = FindObjectOfType<ConcaveStone>();

        r = stone.m_row + GameManager.checkDir[direction,0];
        c = stone.m_col + GameManager.checkDir[direction,1];

        while ((r < 0 || r >= StaticVariable.omokBoardNum) || (c < 0 || c >= StaticVariable.omokBoardNum))
        {
            direction = (direction + 1) % GameManager.checkDir.GetLength(0);

            r = stone.m_row + GameManager.checkDir[direction,0];
            c = stone.m_col + GameManager.checkDir[direction,1];
            Debug.Log($"init r = {r}, c = {c}");
        }
    }   
    // checkDir = new int[,] { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
    // SequenceDir { T, B, L, R, TL, TR, BL, BR }
    //               0  1  2  3  4    5  6    7
    bool PrioritySearchStone()
    {
        int arrNum = StaticVariable.omokBoardNum - 1;        
        int kind = 2; // 2==player   
       
        while(kind > 0)
        {
            int _turn = kind == 2 ? 3- m_turn : m_turn;
            int index = 0, dir = 0;
            int _r = 0,_c = 0;
            foreach(var list in priorityList)
            {
                for(int i = 0; i <= arrNum; i++)
                {
                    switch(list.Key)
                    {
                        case "0" : _r = 0; break;
                        case "i" : _r = i; break;
                        case "arrNum" : _r = arrNum; break;
                        case "arrNum - i" : _r = arrNum - i; break;
                        default : _r = -1; break;
                    }
                    switch(list.Value)
                    {
                        case "0" : _c = 0; break;
                        case "i" : _c = i; break;
                        case "arrNum" : _c = arrNum; break;
                        case "arrNum - i" : _c = arrNum - i; break;
                        default : _c = -1; break;
                    }
                    string s = dir == 0 ? "top" : dir == 1 ? "bottom" : dir == 2 ? "left" : dir == 3 ? "right" : dir == 4 ? "top-right"
                    : dir == 5 ? "top-left" : 
                    dir == 6 ? "bottom-left" : 
                    dir == 7 ? "bottom-right" : "Fail";

                    if(SequenceDangerousStone(_r,_c,_turn,dir))//if(DirectionSearch(_r,_c,_turn,dir,kind))
                    {
                        Debug.Log($"5 stone search {(_turn != m_turn ? "Player turn " : "enemy turn")}  {s}");
                        return true;
                    }
                    else if(SequenceWarningStone(_r,_c,_turn,dir))
                    {
                        Debug.Log($"3 stone search {(_turn != m_turn ? "Player turn " : "enemy turn")}  {s}");
                        return true;
                    }
                }
                index++;
                dir++;
                if(index >= 4 && index <= 5) dir = 4;
                else if(index >= 6 && index <= 7) dir = 5;
                else if(index >= 8 && index <= 9) dir = 6;
                else if(index >= 10 && index <= 11) dir = 7;
            }
            kind--;
        }
        return false;
    }
    bool SequenceDangerousStone(int _sr, int _sc, int turn,int direction)
    {
        // 사방에 적 돌이 없고 비어있다면 막아야 하는 로직을 새로 생성해야 된다 우선순위는 제일 높다
        // 4칸도 막아진다 고로 3칸만 찾음된다
        int saveR = -1, saveC = -1;
        int startR = 0, startC = 0;        
        List<KeyValuePair<int,int>> saveList = new List<KeyValuePair<int, int>>();

        bool isFind = false;
        int count = 0, sequence = 0;
        for(int sr = _sr, sc = _sc; GameManager.CheckOverValue(sr,sc); 
        sr += GameManager.checkDir[direction,0], sc += GameManager.checkDir[direction,1])
        {
            if(GameManager.GetBoardValue(sr,sc) == 0)
                saveList.Add(new KeyValuePair<int, int>(sr,sc));
            if(count == 0)
            {
                startR = sr; startC = sc;
            }
            if(count >= 5)
            {
                foreach(var save in saveList)
                {
                    GameManager.SetBoardValue(save.Key,save.Value,turn);

                    for(int i = startR, j = startC; GameManager.CheckOverValue(i,j);
                    i += GameManager.checkDir[direction,0], j += GameManager.checkDir[direction,1])
                    {
                        if(GameManager.GetBoardValue(i,j) == turn)
                        {
                            sequence++;
                            if(sequence >= 5)
                            {
                                Debug.Log("True");
                                Debug.Log($"true saveR = {save.Key}, saveC = {save.Value}, turn = {turn}");
                                saveR = save.Key;
                                saveC = save.Value;
                                isFind = true;
                            }
                        }
                        else sequence = 0;
                    }
                    GameManager.SetBoardValue(save.Key,save.Value,0);
                    if(isFind)  // 이것도 나중에 바꾸자
                    {
                        break;
                    }
                }
                count = 0;
            }            
            count++;
        }
        if(isFind)
        {
            r = saveR;
            c = saveC;
            return true;
        }
        else return false;
    }
    bool SequenceWarningStone(int _sr, int _sc, int turn,int direction)
    {
        int sequence = 0;
        int backR = _sr ,backC = _sc, frontR = 0, frontC = 0;
        int saveR, saveC;

        for(int sr = _sr, sc = _sc; 
        GameManager.CheckOverValue(sr,sc);
        sr += GameManager.checkDir[direction,0], sc += GameManager.checkDir[direction,1])
        {
            if(GameManager.GetBoardValue(sr,sc) == turn)
            {
                sequence++; // 내가 포함이 되어지지 않아서 이렇구나!!
                if(sequence >= 3) // 앞뒤중 하나라도 막혀있음 false로 전환되어야 함
                {
                    backR = sr - (GameManager.checkDir[direction,0] * sequence);
                    backC = sc - (GameManager.checkDir[direction,1] * sequence);
                    frontR = sr + GameManager.checkDir[direction,0];
                    frontC = sc + GameManager.checkDir[direction,1];
                    
                    if(GameManager.CheckOverValue(backR ,backC) && GameManager.CheckOverValue(frontR,frontC))
                    {
                        if(GameManager.IsEmpty(backR,backC) && GameManager.IsEmpty(frontR,frontC))
                        {
                            Debug.Log($"true backR = {backR}, backC = {backC}, turn = {turn}");
                            Debug.Log($"true frontR = {frontR}, frontC = {frontC}, turn = {turn}");
                            saveR = frontR;
                            saveC = frontC;
                            Debug.Log("2.True");
                            Debug.Log($"true saveR = {saveR}, saveC = {saveC}, turn = {turn}");
                            r = saveR;
                            c = saveC;
                            return true;
                        }
                    }
                }
            }
            else sequence = 0;
        }
        return false;
    }
    void SelectHaviour(int _r, int _c, List<int> storageDirections, OmokPlayer player = null)
    {
        stones = FindObjectsOfType<ConcaveStone>().ToList();
                
        int defendDirection = 0;
        if(player != null) defendDirection = Random.Range(0,GameManager.checkDir.GetLength(0)); //defend

        for(int i = 0; i < GameManager.checkDir.GetLength(0);i++) //방향 엄선
        {
            int tempR = _r; int tempC = _c;

            tempR += GameManager.checkDir[i,0];
            tempC += GameManager.checkDir[i,1];

            // 음수와 오목 max값을 걸러낸다
            if(GameManager.CheckOverValue(tempR,tempC)) 
                storageDirections.Add(i);
        }

        Queue<int> removeQueue = new Queue<int>();

        for(int i = 0; i < storageDirections.Count; i++) // 동일한 값 제외
        {
            int index = storageDirections[i];
            
            foreach(var stone in stones)
            {
                if(stone.m_row  == _r + GameManager.checkDir[index,0]  // 이게 문제인데;;
                && stone.m_col == _c + GameManager.checkDir[index,1])
                {
                    removeQueue.Enqueue(index); // 값을 너야 되는데 index를 넣어서 생긴 이슈같음..
                }
            }
        }
        while (removeQueue.Count > 0)
        {
            int index = removeQueue.Dequeue();
            storageDirections.Remove(index);
        }

        if(storageDirections.Count > 0)
        {
            if(player != null) // defend
            {
                if(!storageDirections.Contains(defendDirection)) // 방향을 엄선 // defend
                {
                    defendDirection = storageDirections[Random.Range(0,storageDirections.Count)];
                }
                r = _r + GameManager.checkDir[defendDirection,0];
                c = _c + GameManager.checkDir[defendDirection,1];
            }
            else
            {
                if(!storageDirections.Contains(direction)) // 방향을 엄선 // attack
                {
                    direction = storageDirections[Random.Range(0,storageDirections.Count)];
                }
                r = _r + GameManager.checkDir[direction,0];
                c = _c + GameManager.checkDir[direction,1];
            }
        }
        else 
        {
            Debug.Log("ReStart!!");
            EnemyStart();
        }
    }
}
