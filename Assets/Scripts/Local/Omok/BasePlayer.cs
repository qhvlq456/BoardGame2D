using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum EPlayerType{
    white,
    black
}
public abstract class BasePlayer : MonoBehaviourPun
{
    public EPlayerType playerType;

    [SerializeField]
    protected GameObject stone;
    [SerializeField]
    protected GameObject alertUI;
    [SerializeField]
    protected Transform parent;
    protected Vector3 putPosition; // 원래 protected
    protected int boardNum;
    public int r,c;
    public int m_turn;
    public virtual void IsMouseButtonDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SetStonePosition(mousePosition);
    }
    public GameObject CreateAlertUI()
    {
        return Instantiate(alertUI,parent);
    }
    public virtual GameObject CreateStone()
    {
        return Instantiate(stone,putPosition,Quaternion.identity);
    }
    public abstract void SetStonePosition(Vector3 mousePosition);
}
