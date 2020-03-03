using Photon.Pun;
using UnityEngine;

public class Takeable : MonoBehaviour
{
    private Transform   myPlaceToGo = null;

    private Rigidbody   myBody = null;

    private PhotonView  myPhotonView = null;

    [SerializeField]
    private int         myPoints = 10;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
    }

    public void Take()
    {
        GetComponent<Rigidbody>().useGravity = false;
        myPhotonView.RequestOwnership();
        myPhotonView.RPC("Rpc_Take", RpcTarget.Others);
    }

    [PunRPC]
    private void Rpc_Take()
    {
        GetComponent<Rigidbody>().useGravity = false;
    }

    public void Release()
    {
        GetComponent<Rigidbody>().useGravity = true;
        myPhotonView.TransferOwnership(0);
        myPhotonView.RPC("Rpc_Release", RpcTarget.Others);
    }

    [PunRPC]
    private void Rpc_Release()
    {
        GetComponent<Rigidbody>().useGravity = true;
    }

    public int GetPoints()
    {
        return myPoints;
    }
}
