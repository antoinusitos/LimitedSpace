using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform       myCamera = null;
    [SerializeField]
    private float           myBaseSpeed = 3.5f;
    private float           mySpeed;

    private Vector3         myDir = Vector3.zero;
    private Rigidbody       myBody = null;

    private PhotonView      myPhotonView = null;

    private bool            myCanMove = true;

    private List<float>     mySpeedModifiers = new List<float>();

    private void Start()
    {
        myBody = GetComponent<Rigidbody>();
        myPhotonView = GetComponent<PhotonView>();
        mySpeed = myBaseSpeed;
    }

    private void Update()
    {
        if (!myCanMove)
            return;

        if (!myPhotonView.IsMine)
            return;

        myDir = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))
        {
            myDir += myCamera.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            myDir -= myCamera.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            myDir += myCamera.right;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            myDir -= myCamera.right;
        }
    }

    private void FixedUpdate()
    {
        if (!myCanMove)
            return;

        if (!myPhotonView.IsMine)
            return;

        myDir.y = 0f;
        myBody.MovePosition(myBody.position + myDir.normalized * mySpeed * Time.fixedDeltaTime);
    }
    /*
    private void CalculateSpeedModifier()
    {
        float tmp = 0f;
        for (int i = 0; i < mySpeedModifiers.Count; i++)
        {
            tmp += mySpeedModifiers[i];
        }
        mySpeed = myBaseSpeed * (1f + tmp);
    }
    */
    private void CalculateSpeedModifier()
    {
        mySpeed = myBaseSpeed;
        for (int i = 0; i < mySpeedModifiers.Count; i++)
        {
            mySpeed *= 1f - mySpeedModifiers[i];
        }
    }

    public void SetCanMove(bool aNewState)
    {
        myCanMove = aNewState;
    }

    public void AddMovementSpeedModifier(float modifier)
    {
        mySpeedModifiers.Add(modifier);
        CalculateSpeedModifier();
    }

    public void RemoveMovementSpeedModifier(float modifier)
    {
        mySpeedModifiers.Remove(modifier);
        CalculateSpeedModifier();
    }
}
