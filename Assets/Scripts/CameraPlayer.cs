using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class CameraPlayer : MonoBehaviour 
{
    #region Public Fields
    public Transform    myTransformToRotate = null;
    public bool         myCanMoveAtStart = true;
    #endregion

    #region Private Fields
    private Transform   myTransform = null;

    private float       myVerticalSpeed;
    private float       myHorizontalSpeed;
    private float       myBaseVerticalSpeed = 150.0f; // was 100f
    private float       myBaseHorizontalSpeed = 150.0f;
    private float       myCurrentAngle = 0f;

    private bool        myInvertMouse = true;
    private bool        myCanMove = true;

    private PhotonView  myPhotonView = null;

    private List<float> mySpeedModifiers = new List<float>();
    #endregion

    #region Unity Methods
    private void Start () 
	{
        myVerticalSpeed = myBaseVerticalSpeed;
        myHorizontalSpeed = myBaseHorizontalSpeed;

        myTransform = transform;
        myCanMove = myCanMoveAtStart;

        Cursor.lockState = CursorLockMode.Locked;
        myPhotonView = GetComponent<PhotonView>();

        if (!myPhotonView.IsMine)
            myTransformToRotate.GetChild(0).gameObject.SetActive(false);
    }
	
	private void Update () 
	{
        if (!myPhotonView.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            myInvertMouse = !myInvertMouse;
        }

        if (!myCanMove) return;

        float x = Input.GetAxis("Mouse X") * Time.deltaTime * myHorizontalSpeed;
        float z = Input.GetAxis("Mouse Y") * Time.deltaTime * myVerticalSpeed;
        if (z > 0f && myCurrentAngle < 89f)
            myCurrentAngle += z;
        else if (z < 0f && myCurrentAngle > -89f)
            myCurrentAngle += z;
        if (myCurrentAngle > 89f) myCurrentAngle = 89f;
        else if (myCurrentAngle < -89f) myCurrentAngle = -89f;

        myTransformToRotate.localRotation = Quaternion.Euler(-myCurrentAngle, 0f, 0f);

        myTransform.Rotate(0f, x, 0f);
    }
    #endregion

    #region Public Methods
    public void SetCanMove(bool newState)
    {
        myCanMove = newState;
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
    #endregion

    #region Private Methods
    private void CalculateSpeedModifier()
    {
        myVerticalSpeed = myBaseVerticalSpeed;
        myHorizontalSpeed = myBaseHorizontalSpeed;
        for (int i = 0; i < mySpeedModifiers.Count; i++)
        {
            myVerticalSpeed *= 1f - mySpeedModifiers[i];
            myHorizontalSpeed *= 1f - mySpeedModifiers[i];
        }
    }
    #endregion
}