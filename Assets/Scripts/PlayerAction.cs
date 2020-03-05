using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private float       myTakeDistance = 100.0f;

    [SerializeField]
    private Transform   myCameraTransform = null;

    [SerializeField]
    private Transform   myHandlePoint = null;
    private Quaternion  myPreviousHandleRotation;

    private Vector3  myTargetVelocity;
    private Vector3  myTargetAngularVelocity;
    private Quaternion  myTargetRotation;
    private Quaternion  myHandleRotation;

    private Transform   myObjectTaken = null;
    private Rigidbody   myObjectRigidbody;
    private float       myObjectZOffset;
    private float       myObjectYOffset;
    private bool        myObjectIsHeavy;

    private bool myRotateObjectInstead = false;

    [SerializeField]
    private float myThrowForce = 5f;

    //[SerializeField]
    //private float       myObjectRotationSpeed = 20.0f;

    [SerializeField]
    private float myObjectVelocityModifier = 1f;
    [SerializeField]
    private float myObjectHeavyUpModifier = 1f;
    [SerializeField]
    private float myObjectLightForwardModifier = 1f;
    [SerializeField]
    private float myObjectAngularVelocityModifier = 1f;

    [SerializeField]
    private float myMaxDistanceToHandlePoint;

    [SerializeField]
    private float myMinDistanceToPlayer = 0f;

    private CameraPlayer cameraPlayer;
    private float myDistancePointer = 0.25f;

    [SerializeField]
    private float myDistancePointerSpeed = 0.2f;

    [SerializeField]
    private float myMinDistanceModifier = 0f;

    [SerializeField]
    private float myMaxDistanceModifier = 1f;

    private void Start()
    {
        cameraPlayer = GetComponent<CameraPlayer>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && myObjectTaken == null)
        {
            TryToTake();
        }
        else if (Input.GetMouseButtonDown(0) && myObjectTaken != null)
        {
            ReleaseObject();
        }

        if (Input.GetMouseButtonDown(2))
        {
            // block camera rotation
            cameraPlayer.enabled = false;
            // and rotate object around X and Y instead
            myRotateObjectInstead = true;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            // resume camera rotation
            cameraPlayer.enabled = true;
            myRotateObjectInstead = false;
        }

        if (Input.GetButtonDown("Throw"))
        {
            Rigidbody bodyToThrow = myObjectTaken.GetComponent<Takeable>().GetRigidbody();
            ReleaseObject();
            bodyToThrow.velocity = Vector3.zero;
            bodyToThrow.AddForce(myCameraTransform.forward * myThrowForce, ForceMode.Impulse);
        }

        if(Input.mouseScrollDelta.magnitude > 0.1f) // multiply myDistancePointerSpeed by that magnitude?
        {
            if(Input.mouseScrollDelta.y < 0f)
            {
                myDistancePointer = Mathf.Max(myDistancePointer - myDistancePointerSpeed, 0f);
            }
            else
            {
                myDistancePointer = Mathf.Min(myDistancePointer + myDistancePointerSpeed, 1f);
            }
            
        }
    }

    private void FixedUpdate()
    {
        if (myObjectTaken != null)
        {
            //myObjectTaken.transform.position = myCameraTransform.position + myCameraTransform.forward * 2f;
            /*
            myObjectTaken.transform.position = myHandlePoint.position;
            myObjectTaken.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            myObjectTaken.GetComponent<Rigidbody>().velocity = Vector3.zero;
            */

            //myObjectRigidbody.MovePosition(myHandlePoint.position);
            //myObjectRigidbody.MovePosition(myHandlePoint.position + myCameraTransform.forward * myObjectDistanceModifier);
            if(Vector3.Distance(myHandlePoint.position, myObjectRigidbody.position) > myMaxDistanceToHandlePoint)
            {
                ReleaseObject();
            }
            else
            {
                // target velocity
                Vector3 bonusPositionModifier;
                if (myObjectIsHeavy) // scrollwheel moves along Y axis
                {
                    bonusPositionModifier = Mathf.SmoothStep(-myMaxDistanceModifier, myMaxDistanceModifier, myDistancePointer) * Vector3.up * myObjectHeavyUpModifier;
                }
                else // scrollwheel moves along Z axis
                {
                    bonusPositionModifier = Mathf.SmoothStep(myMinDistanceModifier, myMaxDistanceModifier, myDistancePointer) * myCameraTransform.forward * myObjectLightForwardModifier;
                }
                myTargetVelocity = ((myHandlePoint.position + myCameraTransform.forward * myObjectZOffset + Vector3.up * myObjectYOffset) + bonusPositionModifier - myObjectRigidbody.position) * myObjectVelocityModifier;

                // target rotation
                myHandleRotation = Quaternion.RotateTowards(myPreviousHandleRotation, myHandlePoint.rotation, 360f);
                //myHandleRotation = myPreviousHandleRotation * myHandlePoint.rotation;
                //Debug.Log(myHandleRotation.ToString());
                
                //Debug.Log(myPreviousHandleRotation.ToString()+"/"+ myHandlePoint.rotation.ToString());

                //myTargetRotation *= Quaternion.RotateTowards(myPreviousHandleRotation, myHandlePoint.rotation, 360f);
                //myTargetRotation *= Quaternion.RotateTowards(myObjectTaken.rotation, myHandlePoint.rotation, 360f);

                //myHandleRotation.eulerAngles = new Vector3(0f, myHandleRotation.eulerAngles.y, myHandleRotation.eulerAngles.z);
                //myHandleRotation.eulerAngles = new Vector3(0f, myHandleRotation.eulerAngles.y, 0f);
                //myHandleRotation.eulerAngles = Quaternion.FromToRotation(new Vector3(0f, myHandleRotation.eulerAngles.y, myHandleRotation.eulerAngles.z),);

                //myTargetRotation *= myHandleRotation;
                //myTargetAngularVelocity = Quaternion.RotateTowards(myObjectTaken.rotation, myTargetRotation, 360f).eulerAngles;
                //myTargetAngularVelocity = (myObjectTaken.rotation * myHandleRotation).eulerAngles;

                //debug
                myTargetAngularVelocity = Vector3.zero;
                //
                if (myRotateObjectInstead)
                {
                    //myObjectRigidbody.AddTorque(Vector3.up * myObjectRotationSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);

                    myTargetAngularVelocity += new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0f) * myObjectAngularVelocityModifier;
                    
                    // TO ADD : translate target angular velocity from world to local space?
                }

                if (myObjectIsHeavy)
                {
                    // check if the object is going to get out of range
                    if (Input.mouseScrollDelta.magnitude > 0.1f && Vector3.Distance(myHandlePoint.position + myCameraTransform.forward * myObjectZOffset + Vector3.up * myObjectYOffset, myObjectRigidbody.position + myTargetVelocity + Vector3.up * myObjectHeavyUpModifier) < myMaxDistanceToHandlePoint)
                    {
                        myTargetVelocity += Vector3.up * myObjectHeavyUpModifier;
                    }
                    // check if the object is going to get too close to player
                    else if(Input.mouseScrollDelta.magnitude < -0.1f && Vector3.Distance(myHandlePoint.position + myCameraTransform.forward * myObjectZOffset + Vector3.up * myObjectYOffset, myObjectRigidbody.position + myTargetVelocity - Vector3.up * myObjectHeavyUpModifier) < myMaxDistanceToHandlePoint)
                    {
                        myTargetVelocity -= Vector3.up * myObjectHeavyUpModifier;
                    }
                }
                else
                {
                    // check if the object is going to get out of range
                    if (Input.mouseScrollDelta.magnitude > 0.1f && Vector3.Distance(myHandlePoint.position + myCameraTransform.forward * myObjectZOffset + Vector3.up * myObjectYOffset, myObjectRigidbody.position + myTargetVelocity + myHandlePoint.forward * myObjectLightForwardModifier) < myMaxDistanceToHandlePoint)
                    {
                        myTargetVelocity += myHandlePoint.forward * myObjectLightForwardModifier;
                    }
                    // check if the object is going to get out of range
                    else if (Input.mouseScrollDelta.magnitude < -0.1f && Vector3.Distance(myHandlePoint.position + myCameraTransform.forward * myObjectZOffset + Vector3.up * myObjectYOffset, myObjectRigidbody.position + myTargetVelocity - myHandlePoint.forward * myObjectLightForwardModifier) > myMinDistanceToPlayer)
                    {
                        myTargetVelocity -= myHandlePoint.forward * myObjectLightForwardModifier;
                    }
                }

                myObjectRigidbody.velocity = myTargetVelocity;
                //myObjectRigidbody.angularVelocity = myTargetRotation.eulerAngles * myObjectAngularVelocityModifier;
                myObjectRigidbody.angularVelocity = myTargetAngularVelocity;

                myPreviousHandleRotation = myHandlePoint.rotation;
            }
        }
    }

    private void TryToTake()
    {
        RaycastHit hit;

        if(Physics.Raycast(myCameraTransform.position, myCameraTransform.forward, out hit, myTakeDistance))
        {
            Takeable takeable = hit.collider.GetComponentInParent<Takeable>();
            if (takeable != null)
            {
                PlayerAction other = takeable.GetOwner();
                if (other != null)
                {
                    other.ReleaseObject();
                }

                myObjectTaken = takeable.transform;
                myObjectRigidbody = takeable.GetRigidbody();
                myObjectZOffset = takeable.GetZOffset();
                myObjectYOffset = takeable.GetYOffset();
                myObjectIsHeavy = takeable.GetIsHeavy();
                myTargetRotation = myObjectTaken.rotation;
                myTargetVelocity = Vector3.zero;
                myTargetAngularVelocity = Vector3.zero;
                myPreviousHandleRotation = myHandlePoint.rotation;
                myDistancePointer = 0.25f;
                takeable.Take(this);
            }
        }
    }

    private void ReleaseObject()
    {
        myObjectTaken.GetComponent<Takeable>().Release();
        myObjectTaken = null;
        myObjectRigidbody = null;
        //myObjectZOffset = 0f;
        //myObjectYOffset = 0f;
    }
}
