using UnityEngine;

public class SecondaryObjectiveTrigger : SecondaryObjective
{
    [SerializeField]
    private Takeable    myObjectToBeInside = null;

    private void OnTriggerEnter(Collider other)
    {
        Takeable takeable = other.GetComponentInParent<Takeable>();
        if (takeable != null)
        {
            if (myObjectToBeInside == takeable)
            {
                if (!myObjectiveComplete)
                {
                    myStateChanged = true;
                }

                myObjectiveComplete = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Takeable takeable = other.GetComponentInParent<Takeable>();
        if (takeable != null)
        {
            if (myObjectToBeInside == takeable)
            {
                if (myObjectiveComplete)
                {
                    myStateChanged = true;
                }

                myObjectiveComplete = false;
            }
        }
    }
}
