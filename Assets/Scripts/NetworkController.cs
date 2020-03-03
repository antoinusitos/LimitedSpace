using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    // Documentation
    // https://doc.photonengine.com/en-us/pun/current/getting-started/pun-intro

    // API
    // https://doc-api.photonengine.com/en/pun/v2/index.html


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Connects to Photon master servers
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (cause == DisconnectCause.ClientTimeout)
        {
            Debug.Log("ClientTimeout : Cannot connect to server :");
        }
        else if (cause == DisconnectCause.ServerTimeout)
        {
            Debug.Log("ServerTimeout: Cannot connect to server :");
        }
        else
        {
            Debug.Log("Cannot connect to server :" + cause.ToString());
        }
    }
}
