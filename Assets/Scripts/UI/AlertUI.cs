using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AlertUI : MonoBehaviour
{    
    public enum EAlertKind
    {
        Success,
        Fail,
        Wait

    }
    Text m_Text;
    Animator animator;
    public EAlertKind alert;    
    float timer = 0f;
    string coroutineName;
    private void Awake() {        
        m_Text = transform.GetChild(0).GetComponent<Text>();
        animator = GetComponent<Animator>();
        Debug.Log($"animator = {animator.GetCurrentAnimatorStateInfo(0).length}");
    }
    private void Update() {
        timer += Time.deltaTime;
    }
    public void StartAnimation(float limtedTime = 0)
    {
        switch(alert)
        {
            case EAlertKind.Fail :
            m_Text.text = "여기에 돌을 둘 수 없습니다..";
            animator.SetTrigger("Fail");
            coroutineName = "FailAnimationText";
                break;
            case EAlertKind.Wait :
            m_Text.text = "상대방이 돌을 두는 중입니다";
            coroutineName = "WaitAnimationText";
                break;
        }
        StartCoroutine(coroutineName,limtedTime);
    }
    IEnumerator FailAnimationText()
    {
        while(timer < animator.GetCurrentAnimatorStateInfo(0).length)
        {
            m_Text.color = Color.Lerp(Color.white,Color.clear, timer);
            yield return null;
        }
    }
    IEnumerator WaitAnimationText(float limtedTime)
    {
        string body = m_Text.text;
        int length = body.Length;
        Image image = GetComponent<Image>();
        limtedTime /= 2;
        while (timer < limtedTime)
        {
            int _length = body.Length;

            if(length + 3 <= _length)
            {
                m_Text.text = body = body.Substring(0,length);
            }
            else m_Text.text =  body += '.';

            yield return new WaitForSeconds(0.25f);
        }
        timer = 0;
        while (timer < limtedTime)
        {
            m_Text.color = Color.Lerp(Color.white,Color.clear, timer);
            image.color = Color.Lerp(Color.white,Color.clear, timer);
            yield return null;
        }
        // 그리고 절반은 disappear로 처리해야 됨
        DestroyAlert();
    }
    public void DestroyAlert()
    {
        Destroy(gameObject);
    }
}
