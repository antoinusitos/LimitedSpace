using UnityEngine;

public class CameraPlayer : MonoBehaviour 
{
    #region Public Fields
    public Transform    myTransformToRotate = null;
    public bool         myCanMoveAtStart = true;
    #endregion

    #region Private Fields
    private Transform   myTransform = null;

    private float       myVerticalSpeed = 100.0f;
    private float       myHorizontalSpeed = 150.0f;
    private float       myCurrentAngle = 0;

    private bool        myInvertMouse = true;
    private bool        myCanMove = true;
    #endregion

    #region Unity Methods
    private void Start () 
	{
        myTransform = transform;
        myCanMove = myCanMoveAtStart;

        Cursor.lockState = CursorLockMode.Locked;
    }
	
	private void Update () 
	{
        if (Input.GetKeyDown(KeyCode.I))
        {
            myInvertMouse = !myInvertMouse;
        }

        if (!myCanMove) return;

        float x = Input.GetAxis("Mouse X") * Time.deltaTime * myHorizontalSpeed;
        float z = Input.GetAxis("Mouse Y") * Time.deltaTime * myVerticalSpeed;
        if (z > 0 && myCurrentAngle < 89)
            myCurrentAngle += z;
        else if (z < 0 && myCurrentAngle > -89)
            myCurrentAngle += z;
        if (myCurrentAngle > 89) myCurrentAngle = 89;
        else if (myCurrentAngle < -89) myCurrentAngle = -89;

        myTransformToRotate.localRotation = Quaternion.Euler(-myCurrentAngle, 0, 0);

        myTransform.Rotate(0, x, 0);
    }
    #endregion

    #region Public Methods
    public void SetCanMove(bool newState)
    {
        myCanMove = newState;
    }
    #endregion

    #region Private Methods
    #endregion
}