using Photon.Pun;
using UnityEngine;

public class Takeable : MonoBehaviour
{

    private Transform   myPlaceToGo = null;

    [Tooltip("Will search for a rigidbody on the same GameObject if left empty.")]
    [SerializeField]
    private Rigidbody   myBody = null;

    [Tooltip("Whether to use heavy object controls or not. Heavy = scroll moves up&down ; off = scroll moves closer and away")]
    [SerializeField]
    private bool myIsHeavy = false;

    private PhotonView  myPhotonView = null;

    [SerializeField]
    private int         myPoints = 10;

    [Tooltip("Offset to pivot position for grabbing")]
    [SerializeField]
    private float myZOffset = 0f;
    [Tooltip("Offset to pivot position for grabbing")]
    [SerializeField]
    private float myYOffset = 0f;

    private PlayerAction myOwner = null;

    public static float setHeavyOverThisMass = 4.0f;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        if(myBody==null)
            myBody = GetComponent<Rigidbody>();
    }

    public void Take(PlayerAction owner)
    {
        myOwner = owner;
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
        myOwner = null;
        myBody.useGravity = true;
        myPhotonView.TransferOwnership(0);
        myPhotonView.RPC("Rpc_Release", RpcTarget.Others);
    }

    [PunRPC]
    private void Rpc_Release()
    {
        myBody.useGravity = true;
    }

    public float GetZOffset()
    {
        return myZOffset;
    }

    public float GetYOffset()
    {
        return myYOffset;
    }

    public int GetPoints()
    {
        return myPoints;
    }

    public Rigidbody GetRigidbody()
    {
        return myBody;
    }

    public bool GetIsHeavy()
    {
        return myIsHeavy;
    }

    public PlayerAction GetOwner()
    {
        return myOwner;
    }

    public void SetOwner(PlayerAction newOwner)
    {
        myOwner = newOwner;
    }

    public void SetPoints(int value)
    {
        myPoints = value;
    }

    public void SetMass(float value)
    {
        myBody.mass = value;
        if(value > setHeavyOverThisMass)
        {
            myIsHeavy = true;
        }
        else
        {
            myIsHeavy = false;
        }
    }
}
