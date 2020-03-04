using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private float       myTakeDistance = 100.0f;

    [SerializeField]
    private Transform   myCameraTransform = null;

    [SerializeField]
    private Transform   myHandlePoint = null;

    private Transform   myObjectTaken = null;
    private Rigidbody   myObjectRigidbody;
    private float       myObjectDistanceModifier;

    [SerializeField]
    private float       myObjectRotationSpeed = 20.0f;

    [SerializeField]
    private float myObjectVelocityModifier = 1f;

    [SerializeField]
    private float myMaxDistanceToHandlePoint;

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
                myObjectRigidbody.velocity = (myHandlePoint.position - myObjectRigidbody.position + (myCameraTransform.forward * myObjectDistanceModifier)) * myObjectVelocityModifier; //+myCameraTransform.forward * myObjectDistanceModifier);

                // make object face player on X axis but not Y

                if (Input.GetMouseButton(2) && myObjectTaken != null)
                {
                    //myObjectTaken.transform.Rotate(Vector3.up, Time.fixedDeltaTime * myObjectRotationSpeed);
                    myObjectRigidbody.AddTorque(Vector3.up * myObjectRotationSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
                }

                if (Input.mouseScrollDelta.magnitude > 0.1f)
                {
                    //myObjectTaken.transform.Rotate(Vector3.right, Time.fixedDeltaTime * myObjectRotationSpeed * 5f);
                    myObjectRigidbody.AddTorque(Vector3.right * myObjectRotationSpeed * Time.fixedDeltaTime * 5f, ForceMode.VelocityChange);
                }
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
                myObjectTaken = takeable.transform;
                myObjectRigidbody = takeable.GetRigidbody();
                myObjectDistanceModifier = takeable.GetDistanceModifier();
                takeable.Take();
            }
        }
    }

    private void ReleaseObject()
    {
        myObjectTaken.GetComponent<Takeable>().Release();
        myObjectTaken = null;
        myObjectRigidbody = null;
        myObjectDistanceModifier = 0f;
    }
}
