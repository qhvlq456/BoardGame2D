using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Res_2D_BoardGame;

public class EnemyOthelloPlayer : OthelloPlayer
{
    int playType;
    public override void Awake()
    {
        base.Awake();
    }
    private void Start() {
        StartCoroutine(EnemyRoutine());
    }
    private void Update() {
        
    }
    public override void SetPlayer()
    {
        base.SetPlayer();
        playerType = (EPlayerType)GameManager.enemyStoneNum;
        m_turn = 2;
    }    
    public override void InitStone()
    {
        int[,] initStone = new int[,] { { 3, 3 }, { 4, 4 } }; // row & col -> mousePos // 흑, 백, 흑, 백

        for (int i = 0; i < 2; i++)
        {
            putPosition = new Vector2(xPosition[initStone[i,1]],yPosition[initStone[i,0]]);
            SelectStone(CreateStone(),initStone[i,0],initStone[i,1]);
            GameManager.SetBoardValue(initStone[i, 0], initStone[i, 1], m_turn);
        }
    }
    IEnumerator EnemyRoutine()
    {
        if(GameManager.isGameOver) yield break;

        while (GameManager.turn != m_turn)
        {
            yield return null;     
        }
        yield return new WaitForSeconds(1f);


        if (!GameManager.CheckTransferTurn(GameManager.GetTurn()))
        {
            //GameManager.NextTurn();
            Debug.Log("Don't put Stone therefore change turn");
        }
        else
        {
            KeyValuePair<int,int> location = GameManager.MuchChangeStoneLocation();
            r = location.Key;
            c = location.Value;
            
            putPosition = new Vector3(xPosition[c],yPosition[r],0);

            SelectStone(CreateStone(),r,c);
            GameManager.OnChangeStone(playerType,m_turn);
            GameManager.SetBoardValue(r, c, m_turn);
            GameManager.ResetList();
            GameManager.ChangeStoneNumber();
        }
        
        if(GameManager.ConditionGameOver())
            GameManager.GameOver();
        else 
        {
            GameManager.NextTurn();
            StartCoroutine(EnemyRoutine());
        }
    }
}
