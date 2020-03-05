using UnityEngine;

public class SecondaryObjectiveRaycast : SecondaryObjective
{
    [SerializeField]
    private Takeable    myObjectOne = null;

    [SerializeField]
    private Takeable    myObjectTwo = null;

    private void Update()
    {
        RaycastHit hit;
        Debug.DrawLine(myObjectOne.transform.position, myObjectTwo.transform.position);

        if (Physics.Linecast(myObjectOne.transform.position, myObjectTwo.transform.position, out hit))
        {
            if (hit.transform == myObjectTwo.transform)
            {
                if (!myObjectiveComplete)
                {
                    myStateChanged = true;
                }

                myObjectiveComplete = true;
            }
            else
            {
                if (myObjectiveComplete)
                {
                    myStateChanged = true;
                }

                myObjectiveComplete = false;
            }
        }
        else
        {
            if (myObjectiveComplete)
            {
                myStateChanged = true;
            }

            myObjectiveComplete = false;
        }
    }
}
