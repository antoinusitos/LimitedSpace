using UnityEngine;

public class SecondaryObjective : MonoBehaviour
{
    [SerializeField]
    private Takeable    myObjectToBeInside = null;

    [SerializeField]
    private string      myText = "";

    private bool        myObjectiveComplete = false;

    private bool        myStateChanged = false;

    private void OnTriggerEnter(Collider other)
    {
        Takeable takeable = other.GetComponentInParent<Takeable>();
        if(takeable != null)
        {
            if(myObjectToBeInside == takeable)
            {
                if(!myObjectiveComplete)
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

    public bool GetObjectiveComplete()
    {
        return myObjectiveComplete;
    }

    public string GetMyText()
    {
        return myText;
    }

    public bool GetMyStateChanged()
    {
        bool state = myStateChanged;
        myStateChanged = false;
        return state;
    }
}
