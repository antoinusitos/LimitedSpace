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

    [SerializeField]
    private float       myObjectRotationSpeed = 20.0f;

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

        if (myObjectTaken != null)
        {
            myObjectTaken.transform.position = myCameraTransform.position + myCameraTransform.forward * 2;
            myObjectTaken.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            myObjectTaken.GetComponent<Rigidbody>().velocity = Vector3.zero;

            if (Input.GetMouseButton(2) && myObjectTaken != null)
            {
                myObjectTaken.transform.Rotate(Vector3.up, Time.deltaTime * myObjectRotationSpeed);
            }

            if(Input.mouseScrollDelta.magnitude > 0.1)
            {
                myObjectTaken.transform.Rotate(Vector3.right, Time.deltaTime * myObjectRotationSpeed * 5);
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
                takeable.Take();
            }
        }
    }

    private void ReleaseObject()
    {
        myObjectTaken.GetComponent<Takeable>().Release();
        myObjectTaken = null;
    }
}
