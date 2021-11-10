using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player : MonoBehaviourPun
{
    PhotonView pv;
    void Start()
    {
        SetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SetPlayer()
    {
        pv = GetComponent<PhotonView>();
    }
}
