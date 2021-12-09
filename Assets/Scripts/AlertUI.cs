using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlertUI : MonoBehaviour
{
    Text m_Text;
    Animator animator;
    public enum EAlertKind
    {
        Success,
        Fail,

    }
    public EAlertKind alert;

    private void Awake() {        
        m_Text = transform.GetChild(0).GetComponent<Text>();
        animator = GetComponent<Animator>();
    }
    private void Start() {
    
        switch(alert)
        {
            case EAlertKind.Fail : m_Text.text = "여기에 돌을 둘 수 없습니다..";
                break;
        }
        StartCoroutine(SyncAnimationText());
    }
    IEnumerator SyncAnimationText()
    {
        float timer = 0f;
        while(timer < animator.GetCurrentAnimatorStateInfo(0).length)
        {
            timer += Time.deltaTime;
            m_Text.color = Color.Lerp(Color.white,new Color(1,1,1,0),timer);
            yield return null;
        }
    }
    public void DestroyAlert()
    {
        Destroy(gameObject);
    }
}
