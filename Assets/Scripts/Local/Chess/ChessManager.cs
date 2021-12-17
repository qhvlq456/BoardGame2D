using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Res_2D_BoardGame;

public class ChessManager : StrategyBoardGame
{
    [Header("Position")]
    float[] xMidpos = new float[StaticVariable.chessBoardNum];
    float[] yMidpos = new float[StaticVariable.chessBoardNum];
    public List<GameObject> dotList = new List<GameObject>();
    public List<ChessStone> findList = new List<ChessStone>();
    GameObject dotObject;
    GameObject spawnPanel;
    SpawnChessStone spawnChessStone;
    Transform parentTransform;
    int r,c;

    [Header("Object")]
    public RaycastHit2D hit;
    public GameObject checkObject;
    float lastPos = 3.6f;
    float startPos = -3.6f;
    float interval = 0.9f;
    void Start()
    {
        OnGameStart();
        DebugBoard();
    }
    
    void Update()
    {
        if(isGameOver) return;

        if (EventSystem.current.IsPointerOverGameObject()) // 어차피 흐름끝나고 여기로 올거임      ui위이고 alertbox가 솬되어 있다면 ui이벤트만 받겠다는거네
            return;
        
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePos,Vector3.forward);
            r = StaticVariable.GetStoneRowPosition(mousePos);
            c = StaticVariable.GetStoneColPosition(mousePos);

            // Life Cycle

            if(checkObject == null) //  not select object
            {
                SelectCycle();
            }
            else // select object
            {
                MoveCycle();
            }
            DebugDeathStone();
        }
    }
    void SelectCycle()
    {
        if(hit.collider == null) // hit 잡힌게 없다 리턴
        {
            return;
        }
        // 잡힌게 있다.. 하지만 같은 stone이 잡힐수도 있음으로 stone초기화후 다시 select
        UnCheckObject();
        CheckObject();
        FindDisableObject();
    }
    void MoveCycle()
    {
        if(hit.collider == null) // hit 잡힌게 없다 .. 로직 검사하여 movement 실행 후 초기화(common)
        {
            if(!AnalyzeBoard(r,c)) return;
            // success analyze
            FindAttackObject(); // 1. find search enemy
            MoveObject(); // 2. move select object
            OnGameStop(); // 3. Next turn previous Check Game Stop
            PawnToAnyStone(); // etc.. Check pawn end line arrive
            NextTurn(); // 4. if don't destroy enemy king nexturn else gameover
        }
        
        // init
        FindEnableObject();
        UnCheckObject();
    }
    
    void CheckObject()
    {
        if(turn != hit.collider.GetComponent<ChessStone>().turn) return;

        checkObject = hit.collider.gameObject;
        
        checkObject.GetComponent<ChessStone>().IsCheck();
        // 결과 모호함
        checkObject.GetComponent<ChessStone>().DefaultMove(IsPossibleMove,moveList);

        CreateDot();
    }
    void UnCheckObject()
    {
        if(checkObject == null) return;

        checkObject.GetComponent<ChessStone>().IsCheck();
        checkObject = null;
        ResetList();
        ClearDot();
    }
    void MoveObject()
    {
        ChessStone stone = checkObject.GetComponent<ChessStone>();
        Move(stone.m_row, stone.m_col,r,c); // board 로직 이동
        stone.m_row = r; stone.m_col = c; // stone의 r,c 변경
        stone.transform.position = new Vector2(xMidpos[c],yMidpos[r]); // world space 위치 변경
    }
    void PawnToAnyStone()
    {
        if(isGameOver) return;

        if(checkObject.gameObject.name == "pawn")
        {
            if(r == 0 || r == 7)
                CreateSpawnPanel();
        }
    }
    void CreateSpawnPanel()
    {
        GameObject objSpawnPanel = Instantiate(spawnPanel,parentTransform);
        spawnChessStone._vector = checkObject.transform.position; // 1. spawnmanager vector값 set
        objSpawnPanel.GetComponent<SpawnBoard>().arrivePawn = checkObject.GetComponent<Pawn>();
        // 3. spawnPanel의 arrivePawn을 적진에 도착한 pawn과 연결
    }
    void FindDisableObject() // Attack 클릭시 빈 오브젝트로 만들기 위해 콜라이더 false 이것도 나누어 만들어야 겠다
    {
        //Debug.Log(IsPossibleMove(r,c,turn) == (int)MoveKind.enemy);
        if(dotList.Count <= 0) return;

        ChessStone[] stones = FindObjectsOfType<ChessStone>();
        foreach(var _stone in stones)
        {
            foreach(var dot in dotList)
            {
                if(_stone.m_row == dot.GetComponent<Dot>().m_row && 
                _stone.m_col == dot.GetComponent<Dot>().m_col)
                {
                    _stone.GetComponent<Collider2D>().enabled = false;
                    findList.Add(_stone);
                }
            }
        }
    }    
    void FindEnableObject() // 다시 원상복귀 시키기
    {
        if(findList.Count <= 0) return;

        foreach(var _list in findList)
        {
            _list.GetComponent<Collider2D>().enabled = true;
        }
        findList.Clear();
    }

    void FindAttackObject()
    {
        foreach(var _list in findList)
        {
            if(_list.m_row == r && _list.m_col == c) 
            {
                //Debug.Log($"{_list.name} is Destroy");
                Attack(r,c,_list.m_num); // 이 리스트 때문에 그렇구나;;
                Destroy(_list.gameObject);
            }
        }
    }
    void InitBoard()
    {
        // black stone start pos = r : 0, c = 0 ~ r : 1, c = 7
        // white stone start pos = r : 6, c = 0 ~ r : 7, c = 7
        // row difference = 4
        int _turn = turn;
        for (int i = 0; i < StaticVariable.chessBoardNum; i++)
        {                                    
            if(i == 2) // 솔직히 이것도 맘에 안드넹ㅋ
            {
                i += 4;
                _turn = 3 - _turn;
            }            
            for(int j = 0; j < StaticVariable.chessBoardNum; j++)
            {                                
                SetBoardValue(i, j,_turn);
            }
        }
    }
    void CreateDot()
    {
        //DebugList();
        foreach(var _list in moveList)
        {
            GameObject dot = Instantiate(dotObject,new Vector2(xMidpos[_list.Value],yMidpos[_list.Key]),Quaternion.identity);
            dot.GetComponent<Dot>().m_row = _list.Key;
            dot.GetComponent<Dot>().m_col = _list.Value;
            dotList.Add(dot);
        }
    }
    void ClearDot()
    {
        foreach(var _dot in dotList)
        {
            Destroy(_dot);
        }
        dotList.Clear();
    }
    public override void OnGameStart()
    {
        checkObject = null;
        StaticVariable.startPos = startPos;
        StaticVariable.lastPos = lastPos;
        StaticVariable.interval = interval;
        InitBoard(StaticVariable.chessBoardNum);
        
        StaticVariable.InitXMidPos(xMidpos);
        StaticVariable.InitYMidPos(yMidpos);

        parentTransform = GameObject.Find("Canvas").transform;
        spawnChessStone = GetComponent<SpawnChessStone>();
        dotObject = Resources.Load("Chess/Dot") as GameObject;
        spawnPanel = Resources.Load("Chess/Spawn_Panel") as GameObject;
    }    
}
