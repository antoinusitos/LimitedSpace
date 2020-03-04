using Photon.Pun;
using UnityEngine;

public class Takeable : MonoBehaviour
{

    private Transform   myPlaceToGo = null;

    [Tooltip("Will search for a rigidbody on the same GameObject if left empty")]
    [SerializeField]
    private Rigidbody   myBody = null;

    private PhotonView  myPhotonView = null;

    [SerializeField]
    private int         myPoints = 10;

    [SerializeField]
    private float myDistanceModifier = 0f;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        if(myBody==null)
            myBody = GetComponent<Rigidbody>();
    }

    public void Take()
    {
        myBody.useGravity = false;
        myPhotonView.RequestOwnership();
        myPhotonView.RPC("Rpc_Take", RpcTarget.Others);
    }

    [PunRPC]
    private void Rpc_Take()
    {
        myBody.useGravity = false;
    }

    public void Release()
    {
        myBody.useGravity = true;
        myPhotonView.TransferOwnership(0);
        myPhotonView.RPC("Rpc_Release", RpcTarget.Others);
    }

    [PunRPC]
    private void Rpc_Release()
    {
        myBody.useGravity = true;
    }

    public float GetDistanceModifier()
    {
        return myDistanceModifier;
    }

    public int GetPoints()
    {
        return myPoints;
    }

    public Rigidbody GetRigidbody()
    {
        return myBody;
    }
}
