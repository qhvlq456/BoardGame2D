using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using Photon.Realtime;
using utility = Photon.Pun.UtilityScripts;

// enum으로 다시 로직 짜기,, 오목 로직 수정
public class Player : MonoBehaviourPun
{
    public PhotonView pv;
    OmokManager GameManager;
    string stonePath;
    public int m_turn; //{set; private get;}
    int r,c;
    Vector2 _vector;
    [SerializeField]
    GameObject alertUI;
    

    void Awake()
    {
        SetPlayer();
        StartCoroutine(WaitPlayerNumbering());
        gameObject.name = "Player " + photonView.OwnerActorNr;
    }
    private void Start() {
    }
    IEnumerator WaitPlayerNumbering()
    {
        while(utility.PlayerNumberingExtensions.GetPlayerNumber(pv.Owner) <= -1)
        {
            yield return null;
        }
        int playerNumber = utility.PlayerNumberingExtensions.GetPlayerNumber(pv.Owner);
        m_turn = playerNumber % 2 == 0 ? 1 : 2; // white and black
    }
    void Update()
    {
        if(!pv.IsMine) return;
        if(GameManager.isGameOver) return;
        if(GameManager.turn != m_turn) return;

        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            r = StaticVariable.OmokGetStoneRowPosition(mousePos,ref _vector);
            c = StaticVariable.OmokGetStoneColPosition(mousePos,ref _vector);

            if (r < 0 || c < 0) return;
            if(!GameManager.IsEmpty(r,c)) 
            {
                GameObject alert = Instantiate(alertUI,GameObject.Find("Canvas").transform);
                alert.GetComponent<AlertUI>().alert = EAlertKind.Fail;
                alert.GetComponent<AlertUI>().StartAnimation();
                return;
            }

            CreateStone();
            SyncPlayer(r,c);

            GameManager.CheckDirection(r,c,m_turn);

            if(GameManager.AnalyzeBoard(r,c,5)) GameOver();
            else
            {
                GameManager.ResetLength();
            }

            if(!GameManager.isGameOver)
            {
                NextTurn();
            }
        }
    }
    void SetPlayer()
    {
        pv = GetComponent<PhotonView>();
        GameManager = GameObject.Find("GameManager").GetComponent<OmokManager>();
        stonePath = "Network/NetworkConcaveStone";
    }
    #region CreateStone    
    public void CreateStone()
    {
        PhotonNetwork.Instantiate(stonePath,_vector,Quaternion.identity);
    }

    void SyncPlayer(int m_r, int m_c)
    {
        pv.RPC("RpcSyncPlayer",RpcTarget.All,m_r,m_c);
    }
    [PunRPC]
    void RpcSyncPlayer(int m_r, int m_c)
    {
        GameManager.SetBoardValue(m_r,m_c,m_turn);
    }
    #endregion

    #region NextTurn
    public void NextTurn()
    {
        photonView.RPC("RpcNextTurn",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcNextTurn()
    {
        GameManager.NextTurn();
        //Debug.LogError($"manager turn = {GameManager.turn}");
    }
    #endregion

    #region GameStop
    public void GameOver()
    {
        Debug.LogError("player Gameover");
        pv.RPC("RpcGameOver",RpcTarget.AllViaServer);
    }
    [PunRPC]
    void RpcGameOver() // 여기서 한번
    {
        GameManager.GameOver(); // DestroyStones 여기에 넣어서 그냥 manager만 false하여 자동으로 하면 될 것 같은데..?
    }
    #endregion
}
