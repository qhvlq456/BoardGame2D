using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnBoard : MonoBehaviour
{    
    public Sprite[] w_Sprite; // array.Length = 4
    public Sprite[] b_Sprite; // array.Length = 4
    public Pawn arrivePawn {private get; set;}
    ChessManager GameManager;
    SpawnChessStone spawnChessStone;    
    Button[] buttons;
    Text[] texts;
    // Start is called before the first frame update
    void Start()
    {        
        InitSpawnBoard();
        SetSpawnBoard();
        SetText();
        SetButtonEvent();
    }    
    void InitSpawnBoard()
    {        
        GameManager = GameObject.Find("GameManager").GetComponent<ChessManager>();
        spawnChessStone = GameObject.Find("GameManager").GetComponent<SpawnChessStone>();
        buttons = gameObject.GetComponentsInChildren<Button>();
        texts = gameObject.GetComponentsInChildren<Text>();
    }
    void SetSpawnBoard()
    {
        int _turn = arrivePawn.m_turn; // 아
        int spriteIdx = 0;
        foreach(var button in buttons)
        {
            if(button.name == "Spawn_Btn_Exit") continue;
            button.gameObject.GetComponent<Image>().sprite = 
            _turn == 1 ? w_Sprite[spriteIdx] : b_Sprite[spriteIdx];
            spriteIdx++;
        }
    }    
    void SetButtonEvent()
    {        
        // 해결해야 할 문제
        // 1. 코드 꼬임
        // 2. 불필요한 코드가 많음
        // 3. pawn 다시짜야됨
        // 4. turn의 통일성
        // 5. 이 코드에서 2 ~ 5 인덱스의 값이 0일 경우 예외처리가 없음;;
        // 근데 create panel을 하지 않으면 되는데 createpanel 자체가 pawn오브젝트의 CreateSpawnPanel() 이리 존제함...
        // 조건문 걸면 또 코드 꼬임 꼬이기 보단 덮어쓰기가 심함 이것도 덮어쓰기 심한듯..
                
        
        // 1. King,  2. Queen, 3. Rook, 4.Bishop, 5.Knight, 6.Pawn
        buttons[0].onClick.AddListener(() => SetButton(2)); // 지역변수가 이리 남아있을 줄은 몰랐다..
        buttons[1].onClick.AddListener(() => SetButton(3)); // 왜 생성되는 거지?
        buttons[2].onClick.AddListener(() => SetButton(4));
        buttons[3].onClick.AddListener(() => SetButton(5));
        buttons[4].onClick.AddListener(() => ExitButton());
    }
    void SetButton(int num)
    {
        if(GameManager.deathStone[arrivePawn.m_turn - 1, num] > 0)
        {
            spawnChessStone.InitCreateStone(spawnChessStone.CreateStone(num),
            arrivePawn.m_row,
            arrivePawn.m_col,
            arrivePawn.m_turn,
            num);
            GameManager.deathStone[arrivePawn.m_turn - 1, num]--; // // death stone--
            Destroy(arrivePawn.gameObject); arrivePawn = null;
            Destroy(gameObject);
        }
    }
    void ExitButton()
    {
        Destroy(gameObject);
    }
    void SetText()
    {    
        int textIdx = 2;
        foreach(var text in texts)
        {
            if(text.name == "Title_Text" || text.name == "Exit_Text") continue;
            text.text = GameManager.deathStone[arrivePawn.m_turn - 1,textIdx].ToString();
            textIdx++;
        }
    }
}
