using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlertUI : MonoBehaviour
{
    [SerializeField]
    Text bodyText;
    Animator animator;
    public enum EAlertKind
    {
        Success,
        Fail,

    }
    public EAlertKind alert;

    private void Awake() {
        animator = GetComponent<Animator>();
    }
    private void Start() {
    
        switch(alert)
        {
            case EAlertKind.Fail : bodyText.text = "여기에 돌을 둘 수 없습니다..";
                break;
        }
    }
    public void DestroyAlert()
    {
        Destroy(gameObject);
    }
}
