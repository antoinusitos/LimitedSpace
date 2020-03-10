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

    [SerializeField]
    private bool garbageCamFollow = false;

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
                // target velocity : 1) object pivot follows handle, 2) add an offset if needed (eg moving big item up to avoid dragging it against the ground), 3) scrollwheel adds an offset to move it closer or further from the player
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

                // target rotation : 1) follow view rotation
                /*
                myHandleRotation = myHandlePoint.rotation * Quaternion.Inverse(myPreviousHandleRotation);
                myTargetRotation *= myHandleRotation;
                */
                if (garbageCamFollow)
                {
                    myHandleRotation = myCameraTransform.rotation * Quaternion.Inverse(myPreviousHandleRotation);
                    myTargetRotation *= myHandleRotation;
                }

                // target rotation : 2) add manipulation rotation
                if (myRotateObjectInstead)
                {
                    //myTargetRotation *= Quaternion.Euler(new Vector3(Input.GetAxis("Mouse Y"), -Input.GetAxis("Mouse X"), 0f));
                    myTargetRotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse Y"), myCameraTransform.right);
                    myTargetRotation *= Quaternion.AngleAxis(Input.GetAxis("Mouse X"), myCameraTransform.up);
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

                // target rotation : 3) add get angular velocity
                // Rotations stack right to left,
                // so first we undo our rotation, then apply the target.
                var delta = myTargetRotation * Quaternion.Inverse(myObjectTaken.rotation);

                float angle; Vector3 axis;
                delta.ToAngleAxis(out angle, out axis);

                // We get an infinite axis in the event that our rotation is already aligned.
                if (float.IsInfinity(axis.x))
                    return;

                if (angle > 180f)
                    angle -= 360f;

                // Here I drop down to 0.9f times the desired movement,
                // since we'd rather undershoot and ease into the correct angle
                // than overshoot and oscillate around it in the event of errors.
                //Vector3 angular = (0.9f * Mathf.Deg2Rad * angle / interval) * axis.normalized;
                Vector3 angular = (0.9f * Mathf.Deg2Rad * angle / Time.fixedDeltaTime) * axis.normalized;

                myObjectRigidbody.angularVelocity = angular;
            }
        }
        //myPreviousHandleRotation = myHandlePoint.rotation;
        myPreviousHandleRotation = myCameraTransform.rotation;
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
                //myPreviousHandleRotation = myHandlePoint.rotation;
                myPreviousHandleRotation = myCameraTransform.rotation;
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
