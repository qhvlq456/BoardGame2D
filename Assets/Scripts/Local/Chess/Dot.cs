using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int m_row,m_col;
    private void Start() {
        gameObject.GetComponent<Animator>().SetBool("isCheck",true);
    }
}
