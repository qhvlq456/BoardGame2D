using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ReadyData : MonoBehaviourPun
{
    public Photon.Realtime.Player player;
    
    [SerializeField]
    Text playerNameText;


    private void Awake() {
    }    
    
    public void SetPlayerText(string name)
    {
        playerNameText.text = name;
    }
}
