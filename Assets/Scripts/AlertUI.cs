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
    void Update() {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) // 1보다 크거나 같으면 에니메이터 시간 종료
            Destroy(gameObject);
    }
}
