using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpawnManager : MonoBehaviourPun
{
    [SerializeField]
    GameObject playerPrefebs;
    public byte customEventcode = 0;
    private void Awake() {
        SpawnPlayer();        
    }
    void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += ReceiveEvent;
    }
    void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= ReceiveEvent;
    }
    #region Spawn Player
    public void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefebs);
        PhotonView pv = player.GetComponent<PhotonView>();

        // if (PhotonNetwork.AllocateViewID(pv))
        // {
            int random = Random.Range(1000,6000);
            pv.ViewID = random;//PhotonNetwork.PlayerList.Length * 1000 + 1;
            Debug.LogError($"랜덤 시발 = {random}");
            Debug.LogError($"Before Player view id = {pv.ViewID}");
            object[] data = new object[]
            {
                player.transform.position, player.transform.rotation, pv.ViewID
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {                
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(customEventcode, data, raiseEventOptions, sendOptions);
        // }
        // else
        // {
        //     Debug.LogError("Failed to allocate a ViewId.");

        //     Destroy(player);
        // }
    }
    
    void ReceiveEvent(EventData photonEvent)
    {
        byte evCode = photonEvent.Code;

        Debug.LogError($"evCode  = {photonEvent.Code}");

        if(evCode == customEventcode)
        {
            Debug.Log("Success");
            object[] data = (object[]) photonEvent.CustomData;

            GameObject player = (GameObject) Instantiate(playerPrefebs, (Vector3) data[0], (Quaternion) data[1]);
            PhotonView photonView = player.GetComponent<PhotonView>();
            photonView.ViewID = (int) data[2];
        }
        else Debug.LogError("not equal code");
    }
    #endregion
}