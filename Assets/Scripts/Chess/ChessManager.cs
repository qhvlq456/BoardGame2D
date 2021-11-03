using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Res_2D_BoardGame;

public class ChessManager : StrategyBoardGame
{
    float[] xMidpos = new float[StaticVariable.chessBoardNum];
    float[] yMidpos = new float[StaticVariable.chessBoardNum];
    public List<GameObject> dotList = new List<GameObject>();
    public List<ChessStone> find_List = new List<ChessStone>();    
    GameObject dotObject;
    GameObject spawnPanel;
    SpawnChessStone spawnChessStone;
    Transform _transform;
    public GameObject checkObject;
    float lastPos = 3.6f;
    float startPos = -3.6f;
    float interval = 0.9f;
    void Start()
    {
        OnGameStart();
    }
    
    void Update()
    {        
        if(isGameOver) return;
        if (EventSystem.current.IsPointerOverGameObject()) // 어차피 흐름끝나고 여기로 올거임      ui위이고 alertbox가 솬되어 있다면 ui이벤트만 받겠다는거네
            return;
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos,Vector3.forward);
            r = StaticVariable.GetStoneRowPosition(mousePos);
            c = StaticVariable.GetStoneColPosition(mousePos);

            if(hit.collider != null)
            {
                if(checkObject == hit.collider.gameObject) // 같은 오브젝트 클릭
                {
                    UnCheckObject();
                    // 만약 같은 오브젝트 클릭시 초기화를 원한다면 이 자리에 UnCheckObject(); 넣으면 됨
                    FindEnableObject();
                }
                else // 다른 오브젝트 클릭
                {
                    UnCheckObject();
                    CheckObject(hit);
                    FindDisableObject(); // dot 경로에 있는 오브젝트의 collider를 false로 바꾸어 클릭 가능하게 함
                }
            }
            else // dot 또는 빈 오브젝트 클릭
            {
                if(AnalyzeBoard())
                {
                    MoveObject();
                    FindEnableObject();  // init
                    NextTurn();
                }
                UnCheckObject();
            }
            DebugBoard();
        }
    }    
    void CheckObject(RaycastHit2D hit)
    {        
        if(turn != hit.collider.GetComponent<ChessStone>().turn) return;

        checkObject = hit.collider.gameObject;
        
        checkObject.GetComponent<ChessStone>().IsCheck();
        checkObject.GetComponent<ChessStone>().CheckMove();
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
        Move(stone.m_row,stone.m_col,turn); //stone.turn); // board 로직 이동 // 요게 stone의 매개변수로 통해서 그렇구나..        
        stone.m_row = r; stone.m_col = c; // stone의 r,c 변경
        stone.transform.position = new Vector2(xMidpos[c],yMidpos[r]); // world space 위치 변경
        PawnToAnyStone();
    }
    void PawnToAnyStone()
    {
        if(checkObject.gameObject.name == "pawn")
        {
            if(r == 0 || r == 7)
                CreateSpawnPanel();
        }
    }
    void CreateSpawnPanel()
    {
        GameObject objSpawnPanel = Instantiate(spawnPanel,_transform);
        spawnChessStone._vector = checkObject.transform.position; // 1. spawnmanager vector값 set
        objSpawnPanel.GetComponent<SpawnBoard>().arrivePawn = checkObject.GetComponent<Pawn>();
        // 3. spawnPanel의 arrivePawn을 적진에 도착한 pawn과 연결
    }
    void FindDisableObject() // attack 클릭시 빈 오브젝트로 만들기 위해 콜라이더 false
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
                    find_List.Add(_stone);
                }
            }
        }
    }    
    void FindEnableObject() // 다시 원상복귀 시키기
    {
        if(find_List.Count <= 0) return;

        foreach(var _list in find_List)
        {
            if(_list.m_row == r && _list.m_col == c) 
            {
                Debug.Log("Destroy");
                Attack(_list.m_num, _list.turn); // 이 리스트 때문에 그렇구나;;
                Destroy(_list.gameObject);
            }            
            _list.GetComponent<Collider2D>().enabled = true;
        }
        find_List.Clear();
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
        foreach(var _list in list)
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
        InitGame(StaticVariable.chessBoardNum);

        InitBoard();
        StaticVariable.InitXMidPos(xMidpos);
        StaticVariable.InitYMidPos(yMidpos);

        _transform = GameObject.Find("Canvas").transform;
        spawnChessStone = GetComponent<SpawnChessStone>();
        dotObject = Resources.Load("Chess/Dot") as GameObject;
        spawnPanel = Resources.Load("Chess/Spawn_Panel") as GameObject;
    }    
}
