using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerType{
    white,
    black
}
public abstract class BasePlayer : MonoBehaviour
{
    public EPlayerType playerType;

    [SerializeField]
    protected GameObject stone;
    [SerializeField]
    protected GameObject alertUI;
    protected Transform parent;
    protected Vector3 putPosition; // 원래 protected
    protected int boardNum;
    public int r,c;
    public int m_turn;
    public virtual void IsMouseButtonDown()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GetStonePosition(mousePosition);
    }
    public GameObject CreateAlertUI()
    {
        return Instantiate(alertUI,parent);
    }
    public virtual GameObject CreateStone()
    {
        return Instantiate(stone,putPosition,Quaternion.identity);
    }
    public abstract void GetStonePosition(Vector3 mousePosition);
    public abstract void SetStonePosition();
}
