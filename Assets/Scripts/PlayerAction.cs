using UnityEngine;
using TMPro;
using cakeslice;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private float       myTakeDistance = 100.0f;

    [SerializeField]
    private Transform   myCameraTransform = null;

    [SerializeField]
    private Transform   myHandlePoint = null;

    [SerializeField]
    private Transform myRotationTarget;

    private Vector3  myTargetVelocity;
    private Vector3  myTargetAngularVelocity;
    //private Quaternion  myTargetRotation;

    private Transform   myObjectTaken = null;
    private Rigidbody   myObjectRigidbody;
    private float       myObjectZOffset;
    private float       myObjectYOffset;
    private bool        myObjectIsHeavy;

    private Takeable    myObjectInSight;
    private Takeable    myObjectTakenScript;

    private bool myRotateObjectInstead = false;

    [SerializeField]
    private float myThrowForce = 5f;

    //[SerializeField]
    //private float       myObjectRotationSpeed = 20.0f;

    [SerializeField]
    private float myObjectVelocityModifier = 1f;
    [SerializeField]
    //private float myObjectHeavyUpModifier = 1f;
    private float myObjectHeavyMovementModifier = 0.5f;
    [SerializeField]
    private float myObjectHeavyLookModifier = 0.5f;
    [SerializeField]
    private float myObjectLightForwardModifier = 1f;
    [SerializeField]
    private float myObjectAngularVelocityModifier = 1f;

    [SerializeField]
    private float myMaxDistanceToHandlePoint;

    [SerializeField]
    private float myMinDistanceToPlayer = 0f;

    private CameraPlayer cameraPlayer;
    private PlayerMovement playerMovement;
    private float myDistancePointer = 0.25f;

    [SerializeField]
    private float myDistancePointerSpeed = 0.2f;

    [SerializeField]
    private float myMinDistanceModifier = 0f;

    [SerializeField]
    private float myMaxDistanceModifier = 1f;

    [SerializeField]
    private LayerMask myObjectLayers;

    [SerializeField]
    private TextMeshProUGUI myObjectDescriptionText;

    private void Start()
    {
        cameraPlayer = GetComponent<CameraPlayer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(myCameraTransform.position, myCameraTransform.forward, out hit, myTakeDistance, myObjectLayers))
        {
            Takeable takeable = hit.collider.GetComponentInParent<Takeable>();
            if (takeable != null)
            {
                if (!takeable.Equals(myObjectInSight))
                {
                    if(!takeable.Equals(myObjectTakenScript))
                    {
                        myObjectInSight = takeable;
                        // show info
                        myObjectDescriptionText.text = myObjectInSight.GetName() + " - <i>" + myObjectInSight.GetPoints() + " points</i>";
                        // outline
                        myObjectInSight.SetOutline(true);
                    }
                    else
                    {
                        if (myObjectInSight != null)
                        {
                            myObjectInSight.SetOutline(false);
                            myObjectInSight = null;
                            myObjectDescriptionText.text = "";
                        }
                    }
                }
            }
            else
            {
                if (myObjectInSight != null)
                {
                    myObjectInSight.SetOutline(false);
                    myObjectInSight = null;
                    myObjectDescriptionText.text = "";
                }
            }
        }
        else
        {
            if(myObjectInSight != null)
            {
                myObjectInSight.SetOutline(false);
                myObjectInSight = null;
                myObjectDescriptionText.text = "";
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (myObjectTaken == null)
            {
                TryToTake();
            }
            else
            {
                ReleaseObject();
            }
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

        if (Input.GetButtonDown("Throw") && myObjectTaken != null)
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
        // cancel rotation other than y for first part of gimble (so that the held object stays parallel to ground)
        myHandlePoint.eulerAngles = new Vector3(0f, myHandlePoint.eulerAngles.y, 0f);

        if (myObjectTaken != null)
        {
            if(Vector3.Distance(myHandlePoint.position, myObjectRigidbody.position) > myMaxDistanceToHandlePoint)
            {
                ReleaseObject();
            }
            else
            {
                // target velocity : 1) object pivot follows handle, 2) add an offset if needed (eg moving big item up to avoid dragging it against the ground), 3) scrollwheel adds an offset to move it closer or further from the player
                Vector3 bonusPositionModifier;
                bonusPositionModifier = Mathf.SmoothStep(myMinDistanceModifier, myMaxDistanceModifier, myDistancePointer) * myCameraTransform.forward * myObjectLightForwardModifier;
                myTargetVelocity = ((myHandlePoint.position + myCameraTransform.forward * myObjectZOffset + Vector3.up * myObjectYOffset) + bonusPositionModifier - myObjectRigidbody.position) * myObjectVelocityModifier;

                // target rotation : 1) add manipulation rotation
                if (myRotateObjectInstead)
                {
                    myRotationTarget.Rotate(myCameraTransform.right, Input.GetAxis("Mouse Y")*2f, Space.World);
                    myRotationTarget.Rotate(myCameraTransform.up, Input.GetAxis("Mouse X")*2f, Space.World);
                }

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

                myObjectRigidbody.velocity = myTargetVelocity;

                // target rotation : 2) add get angular velocity
                // Rotations stack right to left,
                // so first we undo our rotation, then apply the target.
                //var delta = myTargetRotation * Quaternion.Inverse(myObjectTaken.rotation);
                var delta = myRotationTarget.rotation * Quaternion.Inverse(myObjectTaken.rotation);

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
                //Vector3 angular = (0.9f * Mathf.Deg2Rad * angle / Time.fixedDeltaTime) * axis.normalized;
                myTargetAngularVelocity = (0.9f * Mathf.Deg2Rad * angle / Time.fixedDeltaTime) * axis.normalized;

                //myObjectRigidbody.angularVelocity = angular;
                myObjectRigidbody.angularVelocity = myTargetAngularVelocity;
            }
        }
    }

    private void TryToTake()
    {
        if(myObjectInSight != null)
        {
            myObjectDescriptionText.text = "";
            myObjectInSight.SetOutline(false);

            PlayerAction other = myObjectInSight.GetOwner();
            if (other != null)
            {
                other.ReleaseObject();
            }

            myObjectTaken = myObjectInSight.transform;
            myObjectRigidbody = myObjectInSight.GetRigidbody();
            myObjectZOffset = myObjectInSight.GetZOffset();
            myObjectYOffset = myObjectInSight.GetYOffset();
            myObjectIsHeavy = myObjectInSight.GetIsHeavy();
            myRotationTarget.rotation = myObjectTaken.rotation;
            myTargetVelocity = Vector3.zero;
            myTargetAngularVelocity = Vector3.zero;
            myDistancePointer = 0.25f;
            if (myObjectIsHeavy)
            {
                playerMovement.AddMovementSpeedModifier(myObjectHeavyMovementModifier);
                cameraPlayer.AddMovementSpeedModifier(myObjectHeavyLookModifier);
            }
            myObjectInSight.Take(this);
            myObjectTakenScript = myObjectInSight;
            myObjectInSight = null;
        }
    }

    private void ReleaseObject()
    {
        if (myObjectIsHeavy)
        {
            playerMovement.RemoveMovementSpeedModifier(myObjectHeavyMovementModifier);
            cameraPlayer.RemoveMovementSpeedModifier(myObjectHeavyLookModifier);
        }
        myObjectTaken.GetComponent<Takeable>().Release();
        myObjectTaken = null;
        myObjectRigidbody = null;
        myObjectTakenScript = null;
    }
}
