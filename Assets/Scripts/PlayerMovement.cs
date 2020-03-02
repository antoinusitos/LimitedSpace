using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform       myCamera = null;
    [SerializeField]
    private float           mySpeed = 2;

    private Vector3         myDir = Vector3.zero;
    private Rigidbody       myBody = null;

    private void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
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
        myBody.MovePosition(myBody.position + myDir.normalized * mySpeed * Time.fixedDeltaTime);
    }
}
