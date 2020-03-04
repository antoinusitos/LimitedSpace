using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingTrigger : MonoBehaviour
{
    private int                     myCurrentPlayerNumber = 0;
    private List<PlayerMovement>    myCurrentPlayers = new List<PlayerMovement>();
    private bool                    myGameFinished = false;
    private float                   myCurrentTimeToFinish = 0;
    private const float             myTimeToFinish = 2;

    [SerializeField]
    private Text                    myTextFinish = null;

    [SerializeField]
    private HouseCheck              myHouseCheck = null;

    private PhotonView              myPhotonView = null;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if(playerMovement != null && !myCurrentPlayers.Contains(playerMovement))
        {
            myCurrentPlayers.Add(playerMovement);
            myCurrentPlayerNumber++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
        if (playerMovement != null && myCurrentPlayers.Contains(playerMovement))
        {
            myCurrentPlayers.Remove(playerMovement);
            myCurrentTimeToFinish = 0;
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (myGameFinished)
            return;

        if(myCurrentPlayerNumber == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            myCurrentTimeToFinish += Time.deltaTime;
            if(myCurrentTimeToFinish >= myTimeToFinish)
            {
                myGameFinished = true;
                myPhotonView.RPC("Rpc_StopPlayers", RpcTarget.AllViaServer);
            }
        }
    }

    [PunRPC]
    private void Rpc_StopPlayers()
    {
        myTextFinish.gameObject.SetActive(true);
        myTextFinish.text = "TOTAL SCORE " + myHouseCheck.GetTotalPoints().ToString();
        SoundManager.GetInstance().PlayEndSound();

        for (int i = 0; i < myCurrentPlayers.Count; i++)
        {
            myCurrentPlayers[i].SetCanMove(false);
        }
    }
}
